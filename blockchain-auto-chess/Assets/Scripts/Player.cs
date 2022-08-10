using System;
using System.Collections;
using System.Collections.Generic;
using Army;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Soldier _mySoldier;
    private void Start()
    {
        _mySoldier = SoldierFactory.Instance.CreateSoldier(new Vector2Int(3,3));
    }

    public void OnMove(InputValue value)
    {
        var vFloat = value.Get<Vector2>();
        var vInt = Vector2Int.RoundToInt(vFloat);
        _mySoldier.Move(vInt);
        _mySoldier.Turn(vInt);
    }

    private void OnFire()
    {
        _mySoldier.Attack();
    }
}