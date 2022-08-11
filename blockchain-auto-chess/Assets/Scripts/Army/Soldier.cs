using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Army
{
    public class Soldier : MonoBehaviour
    {
        public Vector2Int IndexPos { get; set; }
        public Vector2Int FaceDir { get; private set; }
        public int TeamID { get; private set; }
        private Vector2Int[] _attackPoints = {Vector2Int.up};

        private class MoveTypeConverter
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
            Front,Back,Left,Right
        }

        // reference
        private GameTiles _gameTiles;

        private void Awake()
        {
            _gameTiles = GameTiles.Instance;
            // apply modifier
            SoldierModifier soldierModifier = GetComponent<SoldierModifier>();
            if (soldierModifier != null) _attackPoints = soldierModifier.AttackPoints ?? _attackPoints;
        }

        private void OnDestroy()
        {
            SoldierManager.Instance.UnRegisterSoldier(this);
        }

        //  will check out of bound and collision
        private void SetIndexPos(Vector2Int index)
        {
            // clamp in valid index
            index.x = Mathf.Clamp(index.x, 0, 7);
            index.y = Mathf.Clamp(index.y, 0, 7);
            // dont move when index is occupied
            if (_gameTiles.data[index.x, index.y].occupier)
            {
                index.x = IndexPos.x;
                index.y = IndexPos.y;
            }
            // move in GameTiles
            _gameTiles.PlaceSoldier(this, index);
            // position
            transform.position = _gameTiles.data[index.x, index.y].position;
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

        public void Init(Vector2Int position, Vector2Int faceDirection, int teamID = 0)
        {
            // register
            SoldierManager.Instance.RegisterSoldier(this);
            //
            SetIndexPos(position);
            SetFaceDir(faceDirection);
            FaceDir = Vector2Int.up;
            TeamID = teamID;
        }
        
        // Actions
        public void Move(Vector2Int dVec)
        {
            SetIndexPos(new Vector2Int(IndexPos.x + dVec.x, IndexPos.y + dVec.y));
        }

        public void Move(MoveType moveType)
        {
            Vector2Int moveVec = MoveTypeConverter.GetVec(moveType);
            Vector2Int rotatedMoveVec = RotateVecByFaceDir(moveVec);
            SetIndexPos(new Vector2Int(IndexPos.x + rotatedMoveVec.x, IndexPos.y + rotatedMoveVec.y));
        }

        public void Turn(Vector2Int faceDirection)
        {
            SetFaceDir(faceDirection);
        }

        [SerializeField] private GameObject _debugSquare;

        public void Attack()
        {
            var attackIndices = GetAttackIndices();

            foreach (Vector2Int index in attackIndices)
            {
                StartCoroutine(Create1Sec(index, _debugSquare));
            }
        }
        
        //
        public bool IsEnemy(Soldier other)
        {
            return other.TeamID != TeamID;
        }

        public Vector2Int[] GetAttackIndices()
        {
            Vector2Int[] attackIndices = new Vector2Int[_attackPoints.Length];
            for (int i = 0; i < _attackPoints.Length; i++)
            {
                Vector2Int point = _attackPoints[i];
                Vector2Int rotatedPoint = RotateVecByFaceDir(point);
                attackIndices[i] = rotatedPoint + IndexPos;
            }

            return attackIndices;
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
            GameObject obj = Instantiate(prefab, _gameTiles.data[index.x, index.y].position, prefab.transform.rotation);
            yield return new WaitForSeconds(1);
            Destroy(obj);
        }
    }
}