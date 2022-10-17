using UnityEngine;

namespace MScene.LoadingScene
{
    public class AnimationEventFuncCaller : MonoBehaviour
    {
        [SerializeField] private LoadingSceneController _loadingSceneController;
        public void UnloadAnimationFinish()
        {
            _loadingSceneController.UnloadAnimationFinish();
        } 
    }
}