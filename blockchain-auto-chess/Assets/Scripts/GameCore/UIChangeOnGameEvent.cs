using System;
using UI.GameCore;
using UnityEngine;

namespace GameCore
{
    public class UIChangeOnGameEvent : MonoBehaviour
    {
        [SerializeField] private GameUIController _gameUIController;
        private void Start()
        {
            GameManager.Instance.RoundStartEvent.AddListener(()=>_gameUIController.ShowCardGroup(true));
            GameManager.Instance.WarSimStartEvent.AddListener(()=>_gameUIController.ShowCardGroup(false));
        }
    }
}