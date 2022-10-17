using TMPro;
using UnityEngine;

namespace UI
{
    public class GameStateBoard : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void Set(int round, int redWinCnt, int blueWinCnt)
        {
            _text.text = $"Round: {round}\nRed Win: {redWinCnt}\nBlue Win: {blueWinCnt}\n";
        }
    }
}