using System;
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
            Vector2Int disVec = enemy.IndexPos - _soldier.IndexPos;
            int biggerAttr = math.max(math.abs(disVec.x), math.abs(disVec.y));
            Vector2Int dVec = disVec / biggerAttr;
            if (math.abs(dVec.x) == 1 && math.abs(dVec.x) == math.abs(dVec.y))
            {
                // make sure soldier will not run in loop
                if (dVec.x == 1 && dVec.y == 1) dVec.y = 0;
                else if (dVec.x == -1 && dVec.y == -1) dVec.x = 0;
                else
                {
                    if (dVec.x == -1) dVec.x = 0;
                    else if (dVec.y == -1) dVec.y = 0;
                }
            }

            return (ActionTypes.Move, new MoveActionPayload(dVec));
        }
    }
}