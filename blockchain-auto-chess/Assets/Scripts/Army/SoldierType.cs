using System;
using UnityEngine;

namespace Army
{
    [Serializable]
    public struct SoldierType
    {
        public SoldierNameEnum _name;
        public GameObject _prefab;
        public GameObject _cardFrame;
    }
}