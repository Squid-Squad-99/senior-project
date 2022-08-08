using UnityEngine;

namespace Army
{
    public class MeleeSoldier : Soldier
    {
        public override Vector2Int[] AttackPoints => _attackPoints;

        private readonly Vector2Int[] _attackPoints = new[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            Vector2Int.one, -Vector2Int.one, new Vector2Int(1, -1), new Vector2Int(-1, 1)
        };
    }
}