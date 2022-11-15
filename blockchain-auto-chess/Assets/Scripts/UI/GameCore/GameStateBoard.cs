using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace UI
{
    public class GameStateBoard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _redWinText;
        [SerializeField] private TMP_Text _blueWinText;
        [SerializeField] private TMP_Text _timeText;

        private Stopwatch _stopwatch = new();
        

        public void Set(int round, int redWinCnt, int blueWinCnt)
        {
            _redWinText.text = $"{redWinCnt}";
            _blueWinText.text = $"{blueWinCnt}";
            _stopwatch.Restart();
        }

        private void Update()
        {
            var ts = _stopwatch.Elapsed;
            _timeText.text = $"{ts.Minutes:00}:{ts.Seconds:00}";
        }
    }
}