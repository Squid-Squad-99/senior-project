using System.Collections;
using System.Collections.Generic;
using Army;
using UI;
using Ultility;
using UnityEngine;


/// <summary>
/// game flow:
/// give 6 card to each player
/// player start 
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Player _playerA, _playerB;

    private IEnumerator Start()
    {
        yield return null;
        StartCoroutine(StartGame());
    }

    public IEnumerator StartGame()
    {
        // player init
        _playerA.Init(TeamColorTypes.Blue);
        _playerB.Init(TeamColorTypes.Red);
        LocalUser.Instance.Init(_playerA);
        AutoPlayer.Instance.StartAutoPlay(_playerB);
        // hook UI
        GameUIController.Instance.HookState(_playerA);

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

    public IEnumerator StartRound()
    {
        // 0. 
        // clean up
        SoldierManager.Instance.DestroyAllSoldier();
        // 1.give card to each player
        var cards = GetRandSoldierCards(5);
        _playerA.FillHand(cards);
        _playerB.FillHand(cards);

        // 2. place card to board
        for (int i = 0; i < 1; i++)
        {
            Coroutine aTurn = StartCoroutine(_playerA.MyTurnToUseCard());
            Coroutine bTurn = StartCoroutine(_playerB.MyTurnToUseCard());
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