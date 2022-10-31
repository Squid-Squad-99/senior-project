using System.Collections;
using System.Collections.Generic;
using Army;
using Ultility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace GameCore
{
    /// <summary>
    /// game flow:
    /// give 6 card to each player
    /// player start 
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        [Header("setting")] [SerializeField] private int _roundCntToWin = 3;
        [SerializeField] private int _initCardCnt = 5;
        [SerializeField] private float _animationTimePerRound = 1.0f;
        public float AnimationTimePerRound => _animationTimePerRound;
        public UnityEvent<TeamColorTypes> GameOverEvent;
        public UnityEvent RoundStartEvent;
        public UnityEvent WarSimStartEvent;

        [FormerlySerializedAs("gamePlayerA")] [Header("reference")] [SerializeField] private GamePlayer LocalPlayer;
        [FormerlySerializedAs("gamePlayerB")] [SerializeField] private GamePlayer EnemyPlayer;
        // reference
        private WarSimulation _warSimulation;
        
        protected override void Awake()
        {
            base.Awake();
            _warSimulation = GetComponent<WarSimulation>();
        }

        public IEnumerator StartGameAsync()
        {
            // update game state on round end
            _warSimulation.WarOverUnityEvent.AddListener((winnerColor) =>
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
                    UltiFunc.WaitUntilEvent(_warSimulation.WarOverUnityEvent));
            }

            if (GameState.Instance.RedWinCnt == _roundCntToWin)
            {
                Debug.Log("red win Game");
                GameOverEvent.Invoke(TeamColorTypes.Red);
            }
            else
            {
                Debug.Log("blue win Game");
                GameOverEvent.Invoke(TeamColorTypes.Blue);
            }
        }

        private IEnumerator StartRound()
        {
            // 0.
            RoundStartEvent.Invoke();
            // 0. 
            // clean up
            SoldierManager.Instance.DestroyAllSoldier();
            // 1.give card to each player
            var cards = GetRandSoldierCards(5);
            LocalPlayer.FillHand(cards);
            EnemyPlayer.FillHand(cards);

            // 2. place card to board
            for (int i = 0; i < _initCardCnt; i++)
            {
                Coroutine aTurn = StartCoroutine(LocalPlayer.MyTurnToUseCard());
                Coroutine bTurn = StartCoroutine(EnemyPlayer.MyTurnToUseCard());
                yield return aTurn;
                yield return bTurn;
            }

            // 3. start simulation
            WarSimStartEvent.Invoke();
            yield return new WaitForSeconds(1);
            StartCoroutine(_warSimulation.StartSimulation());
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
}