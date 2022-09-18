using System;
using Ultility;
using UnityEngine;

namespace UI
{
    public class GameUIController : Singleton<GameUIController>
    {
        public CardGroupPanel CardGroupPanel;
        public GameStateBoard GameStateBoard;

        private Player _showedPlayer;

        public void HookState(Player localPlayer)
        {
            // hook card in hand
            CardGroupPanel.ShowCards(localPlayer.CardInHand);
            localPlayer.HandChangeEvent += () => CardGroupPanel.ShowCards(localPlayer.CardInHand);
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