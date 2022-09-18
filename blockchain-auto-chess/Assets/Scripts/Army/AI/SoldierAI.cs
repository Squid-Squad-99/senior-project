using System;
using System.Linq;
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
            // 1. find most significant direction
            Vector2Int disVec = enemy.IndexPos - _soldier.IndexPos;
            int signicAttr = -1;
            if (math.abs(disVec.x) == math.abs(disVec.y))
            {
                // blue horizontal move first, red vertical move first
                signicAttr = (_soldier.TeamColor == TeamColorTypes.Blue) ? 0 : 1;
            }
            else
            {
                signicAttr = math.abs(disVec.x) > math.abs(disVec.y) ? 0 : 1;
            }
            Vector2Int dVec = signicAttr == 0
                ? (disVec.x > 0 ? Vector2Int.right : Vector2Int.left) // x
                : (disVec.y > 0 ? Vector2Int.up : Vector2Int.down); // y
            
            // find available move direction
            // 1. to nearest direction vector
            if (!GameTiles.Instance.IsIndexOccupied(_soldier.IndexPos + dVec))
            {
                return (ActionTypes.Move, new MoveActionPayload(dVec));
            }
            // find available alternative direction
            Vector2Int[] horizontalDir = {Vector2Int.left, Vector2Int.right};
            Vector2Int[] dirs;
            // if dVec is horizontal => find vertical alternative first
            if (horizontalDir.Contains(dVec))
            {
                dirs = new [] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right};
            }
            else
            {
                dirs = new [] {Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down};
            }
            foreach (Vector2Int dir in dirs)
            {
                if (!GameTiles.Instance.IsIndexOutOfBound(_soldier.IndexPos + dir) && !GameTiles.Instance.IsIndexOccupied(_soldier.IndexPos + dir))
                {
                    return (ActionTypes.Move, new MoveActionPayload(dir));
                }
            }

            return (ActionTypes.DoNothing, new Payload());
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