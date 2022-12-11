using System;
using System.Collections.Generic;
using TileMap;
using Ultility;
using UnityEngine;
using UnityEngine.Rendering;

namespace Army
{
    public class SoldierFactory : Singleton<SoldierFactory>
    {
        public enum SoldierNameEnum
        {
            Base,
            Melee,
        }

        [Serializable]
        public struct SoldierType
        {
            public SoldierNameEnum _name;
            public GameObject _prefab;
            public GameObject _cardFrame;
        }

        [SerializeField] private List<SoldierType> _soldierTypes = new List<SoldierType>();

        public readonly Dictionary<SoldierNameEnum, SoldierType> soldierTypeDict =
            new Dictionary<SoldierNameEnum, SoldierType>();

        [Serializable]
        public struct TeamMaterial
        {
            public TeamColorTypes _teamColor;
            public Material _material;
        }

        [SerializeField] private List<TeamMaterial> _teamMaterials = new List<TeamMaterial>();

        private readonly Dictionary<TeamColorTypes, TeamMaterial> _teamMaterialDict =
            new Dictionary<TeamColorTypes, TeamMaterial>();

        private GameTiles _gameTiles;

        protected override void Awake()
        {
            base.Awake();
            _gameTiles = GameTiles.Instance;
            //
            foreach (SoldierType soldierType in _soldierTypes)
            {
                soldierTypeDict.Add(soldierType._name, soldierType);
            }

            foreach (TeamMaterial teamMaterial in _teamMaterials)
            {
                _teamMaterialDict.Add(teamMaterial._teamColor, teamMaterial);
            }
        }

        private SoldierType GetSoldierType(SoldierNameEnum soldierName)
        {
            bool haveType = soldierTypeDict.ContainsKey(soldierName);
            if (!haveType) throw new ArgumentException($"dont have soldier type named: {soldierName}");
            return soldierTypeDict[soldierName];
        }

        public Soldier CreateSoldier(SoldierNameEnum soldierTypeName, Vector2Int pos, TeamColorTypes teamColor, Vector2Int direction)
        {
            SoldierType soldierType = GetSoldierType(soldierTypeName);
            // get soldier  prefab
            GameObject soldierPrefab = soldierType._prefab;
            Vector3 tilePos = _gameTiles.Data[pos.x, pos.y].Position;
            Soldier soldier = Instantiate(soldierPrefab, tilePos, soldierPrefab.transform.rotation)
                .GetComponent<Soldier>();
            soldier.Init(pos, direction, teamColor);
            // set team material
            soldier.Renderer.material = _teamMaterialDict[teamColor]._material;

            return soldier;
        }
    }
}