using JetBrains.Annotations;
using UnityEngine;

namespace Army
{
    public class SoldierModifier : MonoBehaviour
    {
        [CanBeNull]
        public Vector2Int[] AttackPoints => new[]
        {
            Vector2Int.up, Vector2Int.left, Vector2Int.right,
            Vector2Int.one, -Vector2Int.one, new Vector2Int(1, -1), new Vector2Int(-1, 1)
        };
    }
}