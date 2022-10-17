using System.Collections;
using MScene;
using UnityEngine;

namespace App
{
    /// <summary>
    /// the first thing to run when app start
    /// </summary>
    public class Bootstraper : MonoBehaviour
    {
        private IEnumerator Start()
        {
            Debug.Log($"App start...");
            // Load Game menu
            yield return MSceneManager.Instance.LoadSceneAsync(1, true);
            Destroy(gameObject);
        }
    }
}