using System;
using GameCore;
using Ultility;
using UnityEngine;

namespace UI.GameCore
{
    public class GameUIController : MonoBehaviour
    {
        public CardGroupPanel CardGroupPanel;
        public GameStateBoard GameStateBoard;

        private GamePlayer _showedGamePlayer;

        public void HookState(GamePlayer localGamePlayer)
        {
            // hook card in hand
            CardGroupPanel.ShowCards(localGamePlayer.CardInHand);
            localGamePlayer.HandChangeEvent += () => CardGroupPanel.ShowCards(localGamePlayer.CardInHand);
            // hook game state
            Action hookGameState = () =>
            {
                GameState gs = GameState.Instance;
                GameStateBoard.Set(gs.Round, gs.RedWinCnt, gs.BlueWinCnt);
            };
            GameState.Instance.GameStateChangeEvent += hookGameState;
            hookGameState();
        }

    }
}