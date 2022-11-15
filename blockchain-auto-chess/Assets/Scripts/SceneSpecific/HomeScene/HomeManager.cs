using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneSpecific.HomeScene
{
    public class HomeManager : MonoBehaviour
    {
        [SerializeField] private Animator _canvasAnimator;
        [SerializeField] private TMP_Text _waitTimeText;

        private Stopwatch _stopwatch = new Stopwatch();
        
        private static readonly int WaitingAnimID = Animator.StringToHash("WaitMatch");
        private static readonly int CancelAnimID = Animator.StringToHash("Cancel");

        public void OnPressBattleButton()
        {
            _canvasAnimator.SetTrigger(WaitingAnimID);
            _stopwatch.Restart();

            StartCoroutine(LoadGameScene(4));
        }

        private IEnumerator LoadGameScene(float sec)
        {
            yield return new WaitForSeconds(sec);
            SceneManager.LoadScene("GameScene");
        }

        public void OnPressCancelButton()
        {
            _canvasAnimator.SetTrigger(CancelAnimID);
            _stopwatch.Stop();
        }

        private void Update()
        {
            var ts = _stopwatch.Elapsed;
            _waitTimeText.text = $"{ts.Minutes:00}:{ts.Seconds:00}";
        }
    }
}