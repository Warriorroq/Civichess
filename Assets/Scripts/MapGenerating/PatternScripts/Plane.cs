using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.MapGenerating.PatternScripts
{
    public class Plane : IMapPatternGeneration
    {
        public Plane(){}

        public virtual List<CellData> ChooseKingsPositions(int amount, Vector2Int size, CellData[,] map)
        {
            float angle = 0;
            float worldPartAngleSize = 360f / amount;
            float delta = 360f / (amount * 1.5f);
            List<CellData> cells = new List<CellData>();
            while(amount > 0)
            {
                CellData cell = null;

                while(cell is null)
                    cell = ChoosePosition(angle, delta/2f, size, map);

                cells.Add(cell);
                angle += worldPartAngleSize;
                amount--;
            }

            return cells;
        }

        protected virtual CellData ChoosePosition(float angle, float delta, Vector2Int size, CellData[,] map)
        {
            Vector2Int half = size / 2;
            float radians = Random.Range(angle - delta, angle + delta) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
            direction *= Random.Range(0.3f, 1f);
            Vector2Int cellPosition = new Vector2Int((int)(direction.x * half.x) + half.x, (int)(direction.y * half.y) + half.y);
            return map[cellPosition.x, cellPosition.y];
        }

        public virtual CellData[,] GenerateMap(Vector2Int size)
        {
            CellData[,] map = new CellData[size.x, size.y];
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                    map[i, j] = GenerateCell(new Vector2Int(i, j));
            }

            return map;
        }
        protected virtual CellData GenerateCell(Vector2Int position)
            => new CellData(position);

        public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {}
    }
}
