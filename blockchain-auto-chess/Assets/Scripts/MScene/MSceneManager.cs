using System;
using System.Collections;
using Ultility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MScene
{
    public class MSceneManager : Singleton<MSceneManager>
    {
        public int LoadingProgressPercentage { get; private set; } = 0;
        public Action ReadyToUnloadLoadingSceneAction;
        public UnityEvent CanUnloadLoadingScene;

        public IEnumerator LoadSceneAsync(int sceneIndex, bool withLoadingScene)
        {
            LoadingProgressPercentage = 0;
            
            if (!withLoadingScene)
            {
                // load target scene
                AsyncOperation loadSceneAO = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                yield return new WaitUntil(() => loadSceneAO.isDone);
                
                // set target scene active
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
                yield return null;
            }
            else
            {
                // loading scene
                AsyncOperation loadLoadingSceneAO = SceneManager.LoadSceneAsync("LoadingScene", LoadSceneMode.Additive);
                yield return new WaitUntil(() => loadLoadingSceneAO.isDone);
                
                // load target scene & unload current scene
                AsyncOperation loadTargetSceneAO = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
                loadTargetSceneAO.allowSceneActivation = false;

                // track progress
                Debug.Log("loading scene...");
                while (loadTargetSceneAO.isDone != true)
                {
                    // track progress
                    LoadingProgressPercentage = Mathf.CeilToInt(loadTargetSceneAO.progress * 100);

                    // check finish
                    if (loadTargetSceneAO.progress >= 0.9f)
                    {
                        LoadingProgressPercentage = 100;

                        // can activate target scene
                        loadTargetSceneAO.allowSceneActivation = true;
                    }

                    yield return null;
                }

                Debug.Log("scene loaded");

            

                // unload loading scene
                if (ReadyToUnloadLoadingSceneAction.GetInvocationList().Length == 0)
                {
                    Debug.LogError("No object handle unload loading Scene");
                }
                // unload current scene
                AsyncOperation unloadCurrentSceneAO = UnloadCurrentSceneAsync(sceneIndex);
                if (unloadCurrentSceneAO != null) yield return new WaitUntil(() => unloadCurrentSceneAO.isDone);

                Debug.Log("unload loading");
                ReadyToUnloadLoadingSceneAction.Invoke();
                
                yield return UltiFunc.WaitUntilEvent(CanUnloadLoadingScene);
                
                AsyncOperation unloadLoadingAO = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("LoadingScene"));
                yield return new WaitUntil(() => unloadLoadingAO.isDone);
                
                
            }
        }

        private AsyncOperation UnloadCurrentSceneAsync(int newSceneIndex)
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
            {
                Scene preScene = SceneManager.GetActiveScene();
                // SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(newSceneIndex));
                AsyncOperation unLoadSceneAO = SceneManager.UnloadSceneAsync(preScene);
                return unLoadSceneAO;
            }
            else
            {
                SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(newSceneIndex));
            }

            return null;
        }
    }
}