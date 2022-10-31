using UnityEngine;

namespace Ultility
{
    public static class VecUlti
    {
        public static int ManhattanDistance(this Vector2Int a, Vector2Int b)
        {
            checked
            {
                return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
            }
        }

        public static float ManhattanDistance(this Vector3 a, Vector3 b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z);
        }
    }
}