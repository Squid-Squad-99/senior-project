using System;
using UnityEngine;
using UnityEngine.Events;

namespace SceneSpecific.MenuScene
{
    public class DetectPressAnyKey : MonoBehaviour
    {
        public UnityEvent OnPressAnyKey;

        private void Update()
        {
            if (Input.anyKey)
            {
                OnPressAnyKey.Invoke();
            }
        }
    }
}