using Ultility;
using UnityEngine;

namespace Army
{
    public class SoldierFactory : Singleton<SoldierFactory>
    {
    
        [SerializeField] private GameObject _soldierPrefab;
        private GameTiles _gameTiles;

        protected override void Awake()
        {
            base.Awake();
            _gameTiles = GameTiles.Instance;
        }
    
        public Soldier CreateSoldier(Vector2Int pos)
        {
            Vector3 tilePos = _gameTiles.data[pos.x, pos.y].position;
            Soldier soldier = Instantiate(_soldierPrefab, tilePos, _soldierPrefab.transform.rotation).GetComponent<Soldier>();
            soldier.Init(pos, new Vector2Int(0,1));
            return soldier;
        }
    }
}