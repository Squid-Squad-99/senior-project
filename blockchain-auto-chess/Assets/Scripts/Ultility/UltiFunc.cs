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
        public static IEnumerator WaitUntilEvent<T>(UnityEvent<T> unityEvent) {
            var trigger = false;
            Action<T> action = _ => trigger = true;
            unityEvent.AddListener(action.Invoke);
            yield return new WaitUntil(()=>trigger);
            unityEvent.RemoveListener(action.Invoke);
        }
        
        public static IEnumerator WaitUntilEvent<T, T1>(UnityEvent<T, T1> unityEvent) {
            var trigger = false;
            Action<T,T1> action = (_,_)=> trigger = true;
            unityEvent.AddListener(action.Invoke);
            yield return new WaitUntil(()=>trigger);
            unityEvent.RemoveListener(action.Invoke);
        }
        
        public static IEnumerator WaitUntilEvent<T, T1, T2>(UnityEvent<T, T1, T2> unityEvent) {
            var trigger = false;
            Action<T,T1, T2> action = (_,_, _)=> trigger = true;
            unityEvent.AddListener(action.Invoke);
            yield return new WaitUntil(()=>trigger);
            unityEvent.RemoveListener(action.Invoke);
        }
    }
}