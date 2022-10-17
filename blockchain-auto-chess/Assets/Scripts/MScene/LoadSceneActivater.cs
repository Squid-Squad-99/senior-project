using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MScene
{
    public class LoadSceneActivater : MonoBehaviour
    {
        [SerializeField] private string _sceneName;
        public void LoadScene()
        {
            SceneManager.LoadScene(_sceneName);
        }
    }
}