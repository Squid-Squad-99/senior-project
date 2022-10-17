using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MScene.LoadingScene
{
    public class LoadingSceneController : MonoBehaviour
    {
        [SerializeField] private Animator _canvasAnimator;
        [SerializeField] private TMP_Text _percentageText;
        [SerializeField] private Slider _slider;
        private MSceneManager _mSceneManager;
        private static readonly int UnloadScene = Animator.StringToHash("unload");

        private void Awake()
        {
            _mSceneManager = MSceneManager.Instance;
            _mSceneManager.ReadyToUnloadLoadingSceneAction += PlayUnloadAnimation;
        }
        
        private IEnumerator Start()
        {
            while (true)
            {
                _percentageText.text = $"{_mSceneManager.LoadingProgressPercentage}%";
                _slider.value = (float)_mSceneManager.LoadingProgressPercentage / 100.0f;
                yield return null;
            }
        }
        
        private void OnDestroy()
        {
            _mSceneManager.ReadyToUnloadLoadingSceneAction -= PlayUnloadAnimation;
        }
        
        private void PlayUnloadAnimation()
        {
            _canvasAnimator.SetBool(UnloadScene, true);
        }

        public void UnloadAnimationFinish()
        {
            // can unload loading scene
            MSceneManager.Instance.CanUnloadLoadingScene.Invoke();
        }
    }
}