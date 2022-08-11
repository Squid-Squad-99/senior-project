using System;
using System.Collections.Generic;
using Ultility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Army
{
    public class SoldierFactory : Singleton<SoldierFactory>
    {
        [Serializable]
        public struct SoldierType
        {
            public string _name;
            public GameObject _prefab;
        }
        [SerializeField] private List<SoldierType> _soldierTypes = new List<SoldierType>();
        private readonly Dictionary<string, SoldierType> _soldierTypeDict = new Dictionary<string, SoldierType>();
        private GameTiles _gameTiles;

        protected override void Awake()
        {
            base.Awake();
            _gameTiles = GameTiles.Instance;
            //
            foreach (SoldierType soldierType in _soldierTypes)
            {
                _soldierTypeDict.Add(soldierType._name, soldierType);
            }
        }

        private SoldierType GetSoldierType(string soldierName)
        {
            bool haveType= _soldierTypeDict.ContainsKey(soldierName);
            if (!haveType) throw new ArgumentException($"dont have soldier type named: {soldierName}");
            return _soldierTypeDict[soldierName];
        }
    
        public Soldier CreateSoldier(string soldierTypeName, Vector2Int pos, int teamId = 0)
        {
            SoldierType soldierType = GetSoldierType(soldierTypeName);
            GameObject soldierPrefab = soldierType._prefab;
            Vector3 tilePos = _gameTiles.data[pos.x, pos.y].position;
            Soldier soldier = Instantiate(soldierPrefab, tilePos, soldierPrefab.transform.rotation).GetComponent<Soldier>();
            soldier.Init(pos, new Vector2Int(0,1), teamId);
            return soldier;
        }
    }
}