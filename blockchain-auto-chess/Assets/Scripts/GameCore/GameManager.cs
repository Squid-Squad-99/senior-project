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

        /// <summary>
        /// Start game 
        /// </summary>
        public IEnumerator StartGameAsync()
        {

            // run round when no winner yet
            TeamColorTypes? winner;
            while ((winner = GetGameWinner()) == null)
            {
                // run round
                yield return StartCoroutine(StartRoundAsync());
            }

            // handle game over
            OnGameOver(winner.Value);
        }

        /// <summary>
        /// start round
        /// </summary>
        private IEnumerator StartRoundAsync()
        {
            // 1.give card to each player
            var cards = GetRandSoldierCards(_initCardCnt);
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
            yield return _warSimulation.StartSimulation();
            UpdateOnRoundOver(_warSimulation.WinnerColor);
            
            // 4. clean up
            SoldierManager.Instance.DestroyAllSoldier();
        }

        /// <summary>
        /// update game state on round over respect of which color win that round
        /// </summary>
        /// <param name="winnerColor"></param>
        private void UpdateOnRoundOver(TeamColorTypes winnerColor)
        {
            switch (winnerColor)
            {
                // tie
                case TeamColorTypes.None:
                    GameState.Instance.Round++;
                    Debug.Log("it a tie");
                    break;
                // blue win
                case TeamColorTypes.Blue:
                    GameState.Instance.Round++;
                    GameState.Instance.BlueWinCnt++;
                    Debug.Log($"Round Over, winner {winnerColor}");
                    break;
                // red win
                case TeamColorTypes.Red:
                    GameState.Instance.Round++;
                    GameState.Instance.RedWinCnt++;
                    Debug.Log($"Round Over, winner {winnerColor}");
                    break;
            }
        }

        /// <summary>
        /// return game winner, if no winner return null
        /// </summary>
        /// <returns>game winner</returns>
        private TeamColorTypes? GetGameWinner()
        {
            TeamColorTypes? winner = default;
            if (GameState.Instance.RedWinCnt == _roundCntToWin)
            {
                winner = TeamColorTypes.Red;
            }
            else if (GameState.Instance.RedWinCnt == _roundCntToWin)
            {
                winner = TeamColorTypes.Blue;
            }

            return winner;
        }

        /// <summary>
        /// Handle stuff after game over
        /// </summary>
        /// <param name="winner">game winner</param>
        private void OnGameOver(TeamColorTypes winner)
        {
            Debug.Log($"{winner} win Game");
            GameOverEvent.Invoke(winner);
        }

        private List<SoldierType> GetRandSoldierCards(int num)
        {
            List<SoldierType> cards = new List<SoldierType>();
            for (int i = 0; i < num; i++)
            {
                cards.Add(SoldierFactory.Instance.SoldierTypeDict[SoldierNameEnum.Base]);
            }

            return cards;
        }
    }
}