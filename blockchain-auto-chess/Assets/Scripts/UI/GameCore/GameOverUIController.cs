using System;
using GameCore;
using UnityEngine;

namespace UI.GameCore
{
    public class GameOverUIController : MonoBehaviour
    {
        [SerializeField] private GameObject localWinDisplay;
        [SerializeField] private GameObject localLoseDisplay;
        
        private void Awake()
        {
            SetActiveAll(false);
        }

        public void Display(bool localWin)
        {
            gameObject.SetActive(true);
            if (localWin)
            {
                localWinDisplay.SetActive(true);
            }
            else
            {
                localLoseDisplay.SetActive(true);
            }
        }

        private void SetActiveAll(bool active)
        {
            gameObject.SetActive(active);
            localWinDisplay.SetActive(active);
            localLoseDisplay.SetActive(active);
        }
    }
}