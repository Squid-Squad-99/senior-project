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
            GameManager.Instance.gameOverEvent.AddListener(OnGameOver);
        }

        private void OnGameOver(TeamColorTypes winnerColor)
        {
            Debug.Log("asdasf");
            bool localwin = winnerColor == _localPlayer.armyTeamColor;
            _gameOverUIController.Display(localwin);
        }
    }
}