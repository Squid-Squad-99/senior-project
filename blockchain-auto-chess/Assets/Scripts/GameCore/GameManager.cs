using System.Collections;
using System.Collections.Generic;
using Army;
using GameCore;
using UI;
using Ultility;
using UnityEngine;
using UnityEngine.Serialization;


/// <summary>
/// game flow:
/// give 6 card to each player
/// player start 
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GamePlayer gamePlayerA;
    [SerializeField] private GamePlayer gamePlayerB;

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

        while (GameState.Instance.RedWinCnt < 3 && GameState.Instance.BlueWinCnt < 3)
        {
            // start  round
            yield return StartCoroutine(StartRound());
            // wait till round over
            yield return StartCoroutine(
                UltiFunc.WaitUntilEvent(WarSimulation.Instance.WarOverUnityEvent));
        }

        if (GameState.Instance.RedWinCnt == 3)
        {
            Debug.Log("red win Game");
        }
        else
        {
            Debug.Log("blue win Game");
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
        for (int i = 0; i < 5; i++)
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