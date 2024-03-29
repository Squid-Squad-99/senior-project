﻿using UnityEngine;

namespace MScene.LoadingScene
{
    public class OnlyRemainNewObject : MonoBehaviour
    {
        private static GameObject _gameObject = null;
        
        private void Awake()
        {
            if (_gameObject == null)
            {
                _gameObject = gameObject;
            }
            else if(_gameObject != gameObject)
            {
                Destroy(_gameObject);
                _gameObject = gameObject;
            }
        }
    }
}