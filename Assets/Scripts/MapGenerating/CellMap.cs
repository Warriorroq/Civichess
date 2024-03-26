using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.MapGenerating
{
    [Serializable]
    public class CellMap
    {
        public Vector2Int size;
        public CellData[,] map;

        public CellMap(Vector2Int size, CellData[,] map)
        {
            this.size = size;
            this.map = map;
        }

        public CellData this[int x, int y]
        {
            set => map[x, y] = value;
            get => map[x, y];
        }

        public CellData this[Vector2Int position]
        {
            set => map[position.x, position.y] = value;
            get => map[position.x, position.y];
        }

        public bool IsPositionIsInBox(Vector2Int position)
            => position.x >= 0 && position.y >= 0 && position.x < size.x && position.y < size.y;

        public List<Vector2Int> GetPointsOfCircle(Vector2Int center, float radius)
        {
            HashSet<Vector2Int> points = new HashSet<Vector2Int>((int)(radius * 6.64f)); //6.64f is average increase per step
            int x = (int)radius;
            int y = 0;
            int p = 1 - x;

            while (y <= x)
            {
                points.Add(new Vector2Int(x + center.x, y + center.y)); // Octant 1
                points.Add(new Vector2Int(y + center.x, x + center.y)); // Octant 2
                points.Add(new Vector2Int(-x + center.x, y + center.y)); // Octant 4
                points.Add(new Vector2Int(-y + center.x, x + center.y)); // Octant 3
                points.Add(new Vector2Int(-x + center.x, -y + center.y)); // Octant 5
                points.Add(new Vector2Int(-y + center.x, -x + center.y)); // Octant 6
                points.Add(new Vector2Int(x + center.x, -y + center.y)); // Octant 8
                points.Add(new Vector2Int(y + center.x, -x + center.y)); // Octant 7
                y++;
                if (p <= 0)
                    p += 2 * y + 1;                
                else
                {
                    x--;
                    p += 2 * (y - x) + 1;
                }
            }

            return points.ToList();
        }

        public List<Vector2Int> GetPointsInLine(Vector2Int center, Vector2Int target)
        {
            List<Vector2Int> points = new List<Vector2Int>();

            int dx = Mathf.Abs(target.x - center.x);
            int dy = Mathf.Abs(target.y - center.y);
            int sx = center.x < target.x ? 1 : -1;
            int sy = center.y < target.y ? 1 : -1;
            int err = dx - dy;

            int x = center.x;
            int y = center.y;

            while (true)
            {
                points.Add(new Vector2Int(x, y));

                if (x == target.x && y == target.y)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y += sy;
                }
            }

            return points;
        }
    }
}
