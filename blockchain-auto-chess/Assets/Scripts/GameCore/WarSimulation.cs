using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Army;
using Army.AI;
using Newtonsoft.Json;
using Ultility;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;
using File = System.IO.File;

namespace GameCore
{
    public class WarSimulation : MonoBehaviour
    {
        public UnityEvent<TeamColorTypes> WarOverUnityEvent;

        private WarHistory _warHistory;

        public IEnumerator StartSimulation()
        {
            _warHistory = new WarHistory();
            _warHistory.InitBoardState = new AllSoldierPosInWar();
            _warHistory.ActionInWars = new List<AllSoldierActionInWar>();
            _warHistory.InitBoardState.Player1 = new Dictionary<string, Vector2Int>();
            _warHistory.InitBoardState.Player2 = new Dictionary<string, Vector2Int>();
            foreach (Soldier soldier in SoldierManager.Instance.Soldiers)
            {
                if (soldier.TeamColor == TeamColorTypes.Blue)
                {
                    _warHistory.InitBoardState.Player1.Add(soldier.NameInTeam, soldier.IndexPos);
                }
                else
                {
                    _warHistory.InitBoardState.Player2.Add(soldier.NameInTeam, soldier.IndexPos);
                }
            }

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

            AllSoldierActionInWar actionInWar = new AllSoldierActionInWar();
            actionInWar.Player1 = new Dictionary<string, ActionJson>();
            actionInWar.Player2 = new Dictionary<string, ActionJson>();
            foreach(var( soldier, (action, payload)) in actions)
            {
                ActionJson actionJson = new ActionJson();
                actionJson.ActionName = action.ToString();
                actionJson.Payload = payload;
                if (soldier.TeamColor == TeamColorTypes.Blue)
                {
                    actionInWar.Player1.Add(soldier.NameInTeam, actionJson);
                }
                else
                {
                    actionInWar.Player2.Add(soldier.NameInTeam, actionJson);
                }
            }
            _warHistory.ActionInWars.Add(actionInWar);
            string json = JsonConvert.SerializeObject(_warHistory);
            File.WriteAllText("/Users/Ethan/Developer/Projects/college-projects/senior-project/war-history.txt", json);


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

    [Serializable]
    public class WarHistory
    {
        public AllSoldierPosInWar InitBoardState;
        public List<AllSoldierActionInWar> ActionInWars;
    }

    [Serializable]
    public class AllSoldierPosInWar
    {
        public Dictionary<string, Vector2Int> Player1;
        public Dictionary<string, Vector2Int> Player2;
    }

    [Serializable]
    public class AllSoldierActionInWar
    {
        public Dictionary<string, ActionJson> Player1;
        public Dictionary<string, ActionJson> Player2;
    }

    [Serializable]
    public class ActionJson
    {
        public string ActionName;
        public Payload Payload;
    }
}