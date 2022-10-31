using System;
using System.Collections;
using GameCore;
using TileMap;
using Ultility;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;
        private Vector3 _currentVelocity;

        // reference
        private GameTiles _gameTiles;
        private Animator _animator;
        private Renderer _renderer;
        private static readonly int AnimIDSpeed = Animator.StringToHash("speed");
        private static readonly int AnimIDAttack = Animator.StringToHash("attack");
        private static readonly int AnimIDDie = Animator.StringToHash("die");
        private static readonly int AnimIDAttackIndex = Animator.StringToHash("attackIndex");
        private static readonly int AnimIDDieBool = Animator.StringToHash("die_bool");

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _gameTiles = GameTiles.Instance;
            _renderer = GetComponentInChildren<Renderer>();
        }

        private IEnumerator Start()
        {
            StartCoroutine(ToTargetTransformRoutine());
            StartCoroutine(PlayAnimationRoutine());
            yield return null;
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
            _targetPosition = _gameTiles.Data[index.x, index.y].Position;
        }

        private void SetFaceDir(Vector2Int faceDir)
        {
            // check is valid direction
            if (Math.Abs(faceDir.magnitude - 1) > 0.1f) return;
            // face
            FaceDir = faceDir;
            float turnDegree = faceDir.x == 1 ? 0 : faceDir.x == -1 ? 180 : faceDir.y == 1 ? -90 : 90;
            _targetRotation = Quaternion.Euler(0, turnDegree, 0);
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
            if (dVec == Vector2Int.zero) return;
            if (dis != MoveSpeed)
            {
                throw new ArgumentException($"cant move this quick, dis: {dis}");
            }

            // move
            SetIndexPos(new Vector2Int(IndexPos.x + dVec.x, IndexPos.y + dVec.y));
            SetFaceDir(dVec);
        }


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
            // 3. play animation
            _animator.SetTrigger(AnimIDAttack);
            // _animator.SetInteger(AnimIDAttackIndex, Random.Range(0, 2));
        }

        public void TakeDamage(int damage)
        {
            Health = math.max(Health - damage, 0);
            if (Health == 0)
            {
                // play die animation
                _animator.SetTrigger(AnimIDDie);
                _animator.SetBool(AnimIDDieBool, true);
                StartCoroutine(DieAfter(2));
                
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

        public void Show(bool show)
        {
            _renderer.enabled = show;
        }

        private IEnumerator DieAfter(float sec)
        {
            yield return new WaitForSeconds(sec);
            Destroy(gameObject);
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
        

        private IEnumerator ToTargetTransformRoutine()
        {
            StartCoroutine(ToTargetPosition());
            StartCoroutine(ToTargetRotation());
            yield return null;
        }

        private IEnumerator ToTargetPosition()
        {
            _currentVelocity = Vector3.zero;
            while (true)
            {
                if (transform.position == _targetPosition) yield return null;
                float dis = transform.position.ManhattanDistance(_targetPosition);
                float smoothTime = dis / MoveSpeed * GameManager.Instance.AnimationTimePerRound;
                while (transform.position != _targetPosition)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _currentVelocity,
                        smoothTime);
                    yield return null;
                }
            }
        }

        private IEnumerator ToTargetRotation()
        {
            while (true)
            {
                yield return null;
                var transform1 = transform;
                Quaternion rotation = transform1.rotation;
                transform1.rotation = _targetRotation;
            }
        }

        private IEnumerator PlayAnimationRoutine()
        {
            while (true)
            {
                _animator.SetFloat(AnimIDSpeed, _currentVelocity.magnitude);
                yield return null;
            }
        }
    }
}