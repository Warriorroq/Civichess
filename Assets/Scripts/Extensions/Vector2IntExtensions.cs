using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class Vector2IntExtensions
    {
        public static Vector2Int ToOneVector(this Vector2Int vector)
        {

            if (vector.x > 0)
                vector.x = 1;
            else if (vector.x < 0)
                vector.x = -1;

            if (vector.y > 0)
                vector.y = 1;
            else if (vector.y < 0)
                vector.y = -1;

            return vector;
        }

        public static Vector2Int Rotate90(this Vector2Int vector)
            => new Vector2Int(-vector.y, vector.x);
    }
}
