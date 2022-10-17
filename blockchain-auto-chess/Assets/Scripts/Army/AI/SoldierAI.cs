using System;
using System.Linq;
using TileMap;
using Unity.Mathematics;
using UnityEngine;

namespace Army.AI
{
    [RequireComponent(typeof(Soldier))]
    public class SoldierAI : MonoBehaviour
    {
        private Soldier _soldier;

        protected void Awake()
        {
            _soldier = GetComponent<Soldier>();
        }

        public (ActionTypes, Payload) DecideAction()
        {
            var enemy = GetNearestEnemy();
            // check have enemy
            if (enemy == null)
            {
                return (ActionTypes.DoNothing, new Payload());
            }

            // check if enemy is in range
            bool enemyInRange = _soldier.IsInAttackRange(enemy.IndexPos);
            if (enemyInRange)
            {
                return (ActionTypes.Attack, new AttackActionPayload(enemy.IndexPos));
            }

            // move to nearest enemy
            Vector2Int moveVec = GetMoveVecTo(enemy.IndexPos);
            if (moveVec != Vector2Int.zero)
            {
                return (ActionTypes.Move, new MoveActionPayload(moveVec));
            }

            return (ActionTypes.DoNothing, new Payload());
        }

        private static int RandomSeed => _randomSeed++;
        private static int _randomSeed = 5;

        private Vector2Int GetMoveVecTo(Vector2Int targetPos)
        {
            Vector2Int disVec = targetPos - _soldier.IndexPos;
            Vector2Int moveVec = Vector2Int.zero;

            if (disVec == Vector2Int.zero)
            {
            }
            else if (disVec.x == 0)
            {
                moveVec.y = disVec.y > 0 ? 1 : -1;
            }
            else if (disVec.y == 0)
            {
                moveVec.x = disVec.x > 0 ? 1 : -1;
            }
            else if (RandomSeed % 2 == 0)
            {
                moveVec.x = disVec.x > 0 ? 1 : -1;
            }
            else
            {
                moveVec.y = disVec.y > 0 ? 1 : -1;
            }

            return moveVec;
        }

        private Soldier GetNearestEnemy()
        {
            Soldier nearestSoldier = null;
            int nearestDistance = 1000;
            foreach (Soldier other in SoldierManager.Instance.Soldiers)
            {
                // check  is enemy
                if (!_soldier.IsEnemy(other)) continue;
                // check is nearest
                Vector2Int disVec = other.IndexPos - _soldier.IndexPos;
                int distance = math.abs(disVec.x) + math.abs(disVec.y);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestSoldier = other;
                }
            }

            return nearestSoldier;
        }
    }
}