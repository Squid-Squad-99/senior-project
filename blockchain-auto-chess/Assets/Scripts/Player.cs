using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Army;
using Army.AI;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    private void Start()
    {
        SoldierFactory.Instance.CreateSoldier("Base", new Vector2Int(5, 6), 1);
        SoldierFactory.Instance.CreateSoldier("Base", new Vector2Int(2, 1), 2);
    }

    public void OnMove(InputValue value)
    {
        // var vFloat = value.Get<Vector2>();
        // var vInt = Vector2Int.RoundToInt(vFloat);
        // _mySoldier.Move(vInt);
        // _mySoldier.Turn(vInt);
    }

    private void OnFire()
    {
        Dictionary<Soldier, SoldierAI> soldierAis = new Dictionary<Soldier, SoldierAI>();
        Dictionary<Soldier, (ActionTypes, Payload )> actions =
            new Dictionary<Soldier, (ActionTypes, Payload )>();
        foreach (Soldier soldier in SoldierManager.Instance.Soldiers)
        {
            soldierAis.Add(soldier, soldier.GetComponent<SoldierAI>());
        }

        foreach (var (soldier, ai) in soldierAis.Select(x => (x.Key, x.Value)))
        {
            actions.Add(soldier, ai.DecideAction());
        }

        foreach (var (soldier, (actionType, payload) ) in actions.Select(x => (x.Key, x.Value)))
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