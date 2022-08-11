using System;
using Unity.Mathematics;
using UnityEngine;

namespace Army
{
    [RequireComponent(typeof(Soldier))]
    public class SoldierAI : MonoBehaviour
    {
        public enum ActionType
        {
            DoNothing,
            Attack,
            Move
        }

        public class Payload
        {
        }

        public class MoveActionPayload : Payload
        {
            public Vector2Int dVec;

            public MoveActionPayload(Vector2Int dVec)
            {
                this.dVec = dVec;
            }
        }

        private Soldier _soldier;

        protected void Awake()
        {
            _soldier = GetComponent<Soldier>();
        }

        protected Soldier GetNearestEnemy()
        {
            Soldier nearestSoldier = null;
            int nearestDistance = 1000;
            foreach (Soldier other in SoldierManager.Instance.Soldiers)
            {
                // check  is enemy
                if(!_soldier.IsEnemy(other)) continue;
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

        public (ActionType, Payload) DecideAction()
        {
            Soldier enemy = GetNearestEnemy();
            // check if can attack
            Vector2Int[] attackIndices = _soldier.GetAttackIndices();
            bool canHitEnemy =Array.IndexOf(attackIndices,enemy.IndexPos) != -1;
            if (canHitEnemy)
            {
                return (ActionType.Attack, new Payload());
            }
            // move to nearest enemy
            Vector2Int disVec = enemy.IndexPos - _soldier.IndexPos;
            int biggerAttr = math.max(math.abs(disVec.x), math.abs(disVec.y));
            Vector2Int dVec = disVec / biggerAttr;
            if (dVec.x + dVec.y != 1) dVec.x--;
            return (ActionType.Move, new MoveActionPayload(dVec));
        }
    }
}