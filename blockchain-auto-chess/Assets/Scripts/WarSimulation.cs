using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Army;
using Army.AI;
using Ultility;
using UnityEngine;
using UnityEngine.Events;

public class WarSimulation : Singleton<WarSimulation>
{
    public UnityEvent<TeamColorTypes> WarOverUnityEvent; 
    
    public IEnumerator StartSimulation()
    {
        int stepCnt = 100;
        float stepTime = 1f;
        while (SoldierManager.Instance.Soldiers.Count > 0 && stepCnt-- > 0)
        {
            StepSimulation();
            var (isOver, winnerColor) = CheckSimOver();
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
        if (SoldierManager.Instance.TeamSoldierCnt[TeamColorTypes.Blue] == 0 &&
            SoldierManager.Instance.TeamSoldierCnt[TeamColorTypes.Red] == 0)
        {
            return (true, TeamColorTypes.None);
        }
        else if (SoldierManager.Instance.TeamSoldierCnt[TeamColorTypes.Blue] == 0)
        {
            return (true, TeamColorTypes.Red);
        }
        else if (SoldierManager.Instance.TeamSoldierCnt[TeamColorTypes.Red] == 0)
        {
            return (true, TeamColorTypes.Blue);
        }

        return (false, TeamColorTypes.None);
    }

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
                    soldier.Attack(attackPayLoad!.AttackPos);
                    break;
                case ActionTypes.Move:
                    MoveActionPayload movePayLoad = payload as MoveActionPayload;
                    soldier.Move(movePayLoad!.DVec);
                    break;
            }
        }
    }
}