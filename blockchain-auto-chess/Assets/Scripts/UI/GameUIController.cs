using System;
using Ultility;
using UnityEngine;

namespace UI
{
    public class GameUIController : Singleton<GameUIController>
    {
        [SerializeField] private CardGroupPanel _cardGroupPanel;

        private Player _showedPlayer;
        
        public void ShowPlayerStatus(Player player)
        {
            _showedPlayer = player;
            _showedPlayer.HandChangeEvent += OnPlayerStatusChange;
            _cardGroupPanel.ShowCards(_showedPlayer.CardInHand);
        }

        private void OnPlayerStatusChange()
        {
            _cardGroupPanel.ShowCards(_showedPlayer.CardInHand);
        }

        private void OnDestroy()
        {
            if (_showedPlayer != null) _showedPlayer.HandChangeEvent -= OnPlayerStatusChange;
        }
    }
}