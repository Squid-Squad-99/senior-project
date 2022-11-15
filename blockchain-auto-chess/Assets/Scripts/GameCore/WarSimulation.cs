using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Army;
using Army.AI;
using Ultility;
using UnityEngine;
using UnityEngine.Events;

namespace GameCore
{
    public class WarSimulation: MonoBehaviour
    {
        public UnityEvent<TeamColorTypes> WarOverUnityEvent; 
    
        public IEnumerator StartSimulation()
        {
            float stepTime = GameManager.Instance.AnimationTimePerRound;
            bool isOver = false;
            TeamColorTypes winnerColor = TeamColorTypes.None;
            while (!isOver)
            {
                StepSimulation();
                yield return null;
                 (isOver, winnerColor) = CheckSimOver();
                if (isOver)
                {
                    WarOverUnityEvent?.Invoke(winnerColor);
                    yield break;
                }
                yield return new WaitForSeconds(stepTime);
            }
        }

        private (bool, TeamColorTypes) CheckSimOver()
        {
            int blueCnt = SoldierManager.Instance.TeamSoldierCnt[TeamColorTypes.Blue];
            int redCnt = SoldierManager.Instance.TeamSoldierCnt[TeamColorTypes.Red];
            if (blueCnt == 0 &&
                redCnt == 0)
            {
                return (true, TeamColorTypes.None);
            }
            else if (blueCnt == 0)
            {
                return (true, TeamColorTypes.Red);
            }
            else if (redCnt == 0)
            {
                return (true, TeamColorTypes.Blue);
            }

            return (false, TeamColorTypes.None);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void StepSimulation()
        {
            Dictionary<Soldier, SoldierAI> soldierAis = new Dictionary<Soldier, SoldierAI>();
            Dictionary<Soldier, (ActionTypes, Payload )> actions =
                new Dictionary<Soldier, (ActionTypes, Payload )>();
            foreach (Soldier soldier in SoldierManager.Instance.Soldiers)
            {
                soldierAis.Add(soldier, soldier.GetComponent<SoldierAI>());
            }

            // get soldier ai decided actions
            foreach (var (soldier, ai) in soldierAis.Select(x => (x.Key, x.Value)))
            {
                actions.Add(soldier, ai.DecideAction());
            }


            // execute actions
            foreach (var (soldier, (actionType, payload)) in actions.Select(x => (x.Key, x.Value)))
            {
                switch (actionType)
                {
                    case ActionTypes.Attack:
                        AttackActionPayload attackPayLoad = payload as AttackActionPayload;
                        StartCoroutine(soldier.Attack(attackPayLoad!.AttackPos));
                        break;
                    case ActionTypes.Move:
                        MoveActionPayload movePayLoad = payload as MoveActionPayload;
                        soldier.Move(movePayLoad!.DVec);
                        break;
                }
            }
        }
    }
}