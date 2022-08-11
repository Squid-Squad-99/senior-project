using System;
using System.Collections;
using System.Collections.Generic;
using Army;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Soldier _mySoldier, _enemy;
    private SoldierAI _mySoldierAI;
    
    private void Start()
    {
        _mySoldier = SoldierFactory.Instance.CreateSoldier("Melee",new Vector2Int(1,1), 1);
        _enemy = SoldierFactory.Instance.CreateSoldier("Melee",new Vector2Int(6,6), 2);
        _mySoldierAI = _mySoldier.GetComponent<SoldierAI>();
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
        var (actionType, payload) = _mySoldierAI.DecideAction();
        switch (actionType)
        {
            case SoldierAI.ActionType.Attack:
                _mySoldier.Attack();
                break;
            case SoldierAI.ActionType.Move:
                SoldierAI.MoveActionPayload movePayLoad = payload as SoldierAI.MoveActionPayload;
                _mySoldier.Move(movePayLoad!.dVec);
                break;
        }
        // _mySoldier.Attack();
    }
}