using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Ultility
{
    public static class UltiFunc
    {
        public static IEnumerator WaitUntilEvent(UnityEvent unityEvent) {
            var trigger = false;
            Action action = () => trigger = true;
            unityEvent.AddListener(action.Invoke);
            yield return new WaitUntil(()=>trigger);
            unityEvent.RemoveListener(action.Invoke);
        }
    }
}