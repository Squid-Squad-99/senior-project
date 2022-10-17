using System.Collections;
using System.Collections.Generic;
using Army;
using GameCore;
using UI;
using Ultility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;


/// <summary>
/// game flow:
/// give 6 card to each player
/// player start 
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [Header("setting")] 
    [SerializeField]private int _roundCntToWin = 3;
    [Header("reference")]
    [SerializeField] private GamePlayer gamePlayerA;
    [SerializeField] private GamePlayer gamePlayerB;

    public UnityEvent<TeamColorTypes> gameOverEvent;

    public IEnumerator StartGameAsync()
    {
        // update game state on round end
        WarSimulation.Instance.WarOverUnityEvent.AddListener((winnerColor) =>
        {
            if (winnerColor == TeamColorTypes.None)
            {
                GameState.Instance.Round++;
                Debug.Log("it a tie");
            }
            else
            {
                GameState.Instance.Round++;
                if (winnerColor == TeamColorTypes.Blue)
                {
                    GameState.Instance.BlueWinCnt++;
                }
                else
                {
                    GameState.Instance.RedWinCnt++;
                }

                Debug.Log($"Round Over, winner {winnerColor}");
            }
        });

        while (GameState.Instance.RedWinCnt < _roundCntToWin && GameState.Instance.BlueWinCnt < _roundCntToWin)
        {
            // start  round
            yield return StartCoroutine(StartRound());
            // wait till round over
            yield return StartCoroutine(
                UltiFunc.WaitUntilEvent(WarSimulation.Instance.WarOverUnityEvent));
        }

        if (GameState.Instance.RedWinCnt == _roundCntToWin)
        {
            Debug.Log("red win Game");  
            gameOverEvent.Invoke(TeamColorTypes.Red);
        }
        else
        {
            Debug.Log("blue win Game");
            gameOverEvent.Invoke(TeamColorTypes.Blue);
        }
    }

    private IEnumerator StartRound()
    {
        // 0. 
        // clean up
        SoldierManager.Instance.DestroyAllSoldier();
        // 1.give card to each player
        var cards = GetRandSoldierCards(5);
        gamePlayerA.FillHand(cards);
        gamePlayerB.FillHand(cards);

        // 2. place card to board
        for (int i = 0; i < 2; i++)
        {
            Coroutine aTurn = StartCoroutine(gamePlayerA.MyTurnToUseCard());
            Coroutine bTurn = StartCoroutine(gamePlayerB.MyTurnToUseCard());
            yield return aTurn;
            yield return bTurn;
        }

        // 3. start simulation
        StartCoroutine(WarSimulation.Instance.StartSimulation());
    }

    private List<SoldierFactory.SoldierType> GetRandSoldierCards(int num)
    {
        List<SoldierFactory.SoldierType> cards = new List<SoldierFactory.SoldierType>();
        for (int i = 0; i < num; i++)
        {
            cards.Add(SoldierFactory.Instance.soldierTypeDict[SoldierFactory.SoldierNameEnum.Base]);
        }

        return cards;
    }
}