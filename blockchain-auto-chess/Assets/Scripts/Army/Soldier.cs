using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Army
{
    public class Soldier : MonoBehaviour
    {
        public Vector2Int IndexPos { get; set; }
        public Vector2Int FaceDir { get; private set; } // none critical state variable
        public TeamColorTypes TeamColor { get; private set; }
        public int Health { get; private set; }
        public int AttackRange { get; private set; } = 1;
        public int MoveSpeed { get; private set; } = 1;
        public int MaxHealth { get; private set; } = 10;
        public int Strength { get; private set; } = 3;

        private static class MoveTypeConverter
        {
            private static readonly Vector2Int Front = new Vector2Int(0, 1);
            private static readonly Vector2Int Back = new Vector2Int(0, -1);
            private static readonly Vector2Int Left = new Vector2Int(-1, 0);
            private static readonly Vector2Int Right = new Vector2Int(1, 0);

            public static Vector2Int GetVec(MoveType moveType)
            {
                switch (moveType)
                {
                    case MoveType.Front:
                        return Front;
                    case MoveType.Back:
                        return Back;
                    case MoveType.Left:
                        return Left;
                    case MoveType.Right:
                        return Right;
                    default:
                        throw new ArgumentException($"dont have move type {moveType}");
                }
            }
        }

        public enum MoveType
        {
            Front,
            Back,
            Left,
            Right
        }

        // reference
        private GameTiles _gameTiles;

        private void Awake()
        {
            _gameTiles = GameTiles.Instance;
        }

        private void OnDestroy()
        {
            if (SoldierManager.Instance != null) SoldierManager.Instance.UnRegisterSoldier(this);
            
        }

        //  will check out of bound and collision
        private void SetIndexPos(Vector2Int index)
        {
            // clamp in valid index
            index.x = Mathf.Clamp(index.x, 0, 7);
            index.y = Mathf.Clamp(index.y, 0, 7);
            // dont move when index is occupied
            if (_gameTiles.Data[index.x, index.y].Occupier)
            {
                index.x = IndexPos.x;
                index.y = IndexPos.y;
            }

            // move in GameTiles
            _gameTiles.PlaceSoldier(this, index);
            // position
            transform.position = _gameTiles.Data[index.x, index.y].Position;
        }

        private void SetFaceDir(Vector2Int faceDir)
        {
            // check is valid direction
            if (Math.Abs(faceDir.magnitude - 1) > 0.1f) return;
            // face
            FaceDir = faceDir;
            float turnDegree = faceDir.x == 1 ? 0 : faceDir.x == -1 ? 180 : faceDir.y == 1 ? -90 : 90;
            transform.rotation = Quaternion.Euler(0, turnDegree, 0);
        }

        public void Init(Vector2Int position, Vector2Int faceDirection, TeamColorTypes teamColor)
        {
            
            // apply modifier
            SoldierAttrModifier soldierAttrModifier = GetComponent<SoldierAttrModifier>();
            if (soldierAttrModifier != null)
            {
                AttackRange = soldierAttrModifier.AttackRange ?? AttackRange;
            }

            // set init value
            SetIndexPos(position);
            SetFaceDir(faceDirection);
            FaceDir = Vector2Int.up;
            TeamColor = teamColor;
            Health = MaxHealth;
            
            // register
            SoldierManager.Instance.RegisterSoldier(this);
        }

        // Actions
        public void Move(Vector2Int dVec)
        {
            // check can move their
            int dis = math.abs(dVec.x) + math.abs(dVec.y);
            if (dis > MoveSpeed)
            {
                throw new ArgumentException($"cant move this quick, dis: {dis}");
            }

            // move
            SetIndexPos(new Vector2Int(IndexPos.x + dVec.x, IndexPos.y + dVec.y));
            SetFaceDir(dVec);
        }

        public void Move(MoveType moveType)
        {
            Vector2Int moveVec = MoveTypeConverter.GetVec(moveType);
            Vector2Int rotatedMoveVec = RotateVecByFaceDir(moveVec);
            SetIndexPos(new Vector2Int(IndexPos.x + rotatedMoveVec.x, IndexPos.y + rotatedMoveVec.y));
            SetFaceDir(moveVec);
        }

        [SerializeField] private GameObject _debugSquare;


        public void Attack(Vector2Int attackPos)
        {
            // check can attack
            if (!IsInAttackRange(attackPos)) throw new ArgumentException($"can't attack position out of range");
            // attack
            // 1. attack
            Soldier enemy = _gameTiles.Data[attackPos.x, attackPos.y].Occupier;
            if (enemy)
            {
                enemy.TakeDamage(Strength);
            }

            // 2. turn to face enemy
            SetFaceDir(AttackPosFaceDirection(attackPos));
            // 3. animation
            CoroutineManager.Instance.StartCoroutine(Create1Sec(attackPos, _debugSquare));
        }

        public void TakeDamage(int damage)
        {
            Health = math.max(Health - damage, 0);
            if (Health == 0)
            {
                Destroy(gameObject);
            }
        }

        public bool IsInAttackRange(Vector2Int attackPos)
        {
            // check in range
            var dis = math.abs(IndexPos.x - attackPos.x) + math.abs(IndexPos.y - attackPos.y);
            return dis <= AttackRange;
        }

        //
        public bool IsEnemy(Soldier other)
        {
            return other.TeamColor != TeamColor;
        }

        private Vector2Int AttackPosFaceDirection(Vector2Int attackPos)
        {
            Vector2Int dVec = attackPos - IndexPos;
            if (math.abs(dVec.x) > math.abs(dVec.y))
            {
                dVec.y = 0;
                dVec.x /= math.abs(dVec.x);
            }
            else
            {
                dVec.x = 0;
                dVec.y /= math.abs(dVec.y);
            }

            return dVec;
        }


        private Vector2Int RotateVecByFaceDir(Vector2Int vec)
        {
            // rotate matrix 
            int[] rotateMatrix;
            if (FaceDir == Vector2Int.up)
            {
                rotateMatrix = new[] {1, 0, 0, 1};
            }
            else if (FaceDir == Vector2Int.down)
            {
                rotateMatrix = new[] {-1, 0, 0, -1};
            }
            else if (FaceDir == Vector2Int.right)
            {
                rotateMatrix = new[] {0, 1, -1, 0};
            }
            else if (FaceDir == Vector2Int.left)
            {
                rotateMatrix = new[] {0, -1, 1, 0};
            }
            else
            {
                throw new ArgumentException($"cant handle face direction {FaceDir}");
            }

            Vector2Int rotatedVec = new Vector2Int(
                vec.x * rotateMatrix[0] + vec.y * rotateMatrix[1],
                vec.x * rotateMatrix[2] + vec.y * rotateMatrix[3]);
            return rotatedVec;
        }

        private IEnumerator Create1Sec(Vector2Int index, GameObject prefab)
        {
            GameObject obj = Instantiate(prefab, _gameTiles.Data[index.x, index.y].Position, prefab.transform.rotation);
            yield return new WaitForSeconds(1);
            Destroy(obj);
        }
    }
}