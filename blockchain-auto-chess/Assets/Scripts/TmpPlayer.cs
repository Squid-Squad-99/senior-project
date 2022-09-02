using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Army;
using Army.AI;
using UnityEngine;
using UnityEngine.InputSystem;


public class TmpPlayer : MonoBehaviour
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
            SoldierFactory.Instance.CreateSoldier(SoldierFactory.SoldierNameEnum.Base, pos, TeamColorTypes.Blue);
        }
        foreach (var pos in redTeam)
        {
            SoldierFactory.Instance.CreateSoldier(SoldierFactory.SoldierNameEnum.Base, pos, TeamColorTypes.Red);
        }
    }
    
    private void OnFire()
    {
        WarSimulation.Instance.StepSimulation();
    }
}