using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Army;
using Army.AI;
using Ultility;
using UnityEngine;

public class WarSimulation : Singleton<WarSimulation>
{
    public IEnumerator StartSimulation()
    {
        int stepCnt = 100;
        float stepTime = 1f;
        while (SoldierManager.Instance.Soldiers.Count > 0 && stepCnt-- > 0)
        {
            StepSimulation();
            yield return new WaitForSeconds(stepTime);
        }
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