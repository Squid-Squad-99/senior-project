using System;
using Army;
using GameCore;
using UnityEngine;

namespace UI.GameCore
{
    public class ToGameOverUITransitor : MonoBehaviour
    {
        [SerializeField] private GamePlayer _localPlayer;
        [SerializeField] private GameOverUIController _gameOverUIController;

        private void Start()
        {
            GameManager.Instance.GameOverEvent.AddListener(OnGameOver);
        }

        private void OnGameOver(TeamColorTypes winnerColor)
        {
            bool localwin = winnerColor == _localPlayer.armyTeamColor;
            _gameOverUIController.Display(localwin);
        }
    }
}