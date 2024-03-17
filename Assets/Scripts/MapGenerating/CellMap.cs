using System;
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
    }
}
