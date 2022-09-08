using System;
using Ultility;
using UnityEngine;

namespace UI
{
    public class GameUIController : Singleton<GameUIController>
    {
        public CardGroupPanel CardGroupPanel;

        private Player _showedPlayer;
        
        public void ShowPlayerStatus(Player player)
        {
            _showedPlayer = player;
            _showedPlayer.HandChangeEvent += OnPlayerStatusChange;
            CardGroupPanel.ShowCards(_showedPlayer.CardInHand);
        }

        private void OnPlayerStatusChange()
        {
            CardGroupPanel.ShowCards(_showedPlayer.CardInHand);
        }

        protected override void Awake()
        {
            base.Awake();
            CardGroupPanel.CardTagEvent += (index) =>
            {
                CardGroupPanel.SelectCard(index);
                
            };
        }

        private void OnDestroy()
        {
            if (_showedPlayer != null) _showedPlayer.HandChangeEvent -= OnPlayerStatusChange;
        }
    }
}