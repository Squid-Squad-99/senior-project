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
        Vector2Int[] blueTeam = new[]
        {
            new Vector2Int(1, 7),
            new Vector2Int(2, 5),
            new Vector2Int(1, 3),
            
        };
        Vector2Int[] redTeam = new[]
        {
            new Vector2Int(7, 7),
            new Vector2Int(7, 6),
            new Vector2Int(7, 5)
            
        };
        foreach (var pos in blueTeam)
        {
            SoldierFactory.Instance.CreateSoldier("Base", pos, TeamIDTypes.Blue);
        }
        foreach (var pos in redTeam)
        {
            SoldierFactory.Instance.CreateSoldier("Base", pos, TeamIDTypes.Red);
        }
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