using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGenerating.PatternScripts
{
    public class Plane : IMapPatternGeneration
    {
        private float _maxOffset;
        private float _step;
        private bool _isToUsePerlinNoise;
        private int _maxHeight;
        public Plane(bool usePerlinNoise, int maxHeight, float step, float maxOffSet) { 
            _isToUsePerlinNoise = usePerlinNoise;
            _maxHeight = maxHeight;
            _maxOffset = maxOffSet;
            _step = step;
        }


        public List<MapGenerator.CellData> ChooseKingsPositions(int amount, Vector2Int size, MapGenerator.CellData[,] map)
        {
            float angle = 0;
            float worldPartAngleSize = 360f / amount;
            float delta = 360f / (amount * 1.5f);
            List<MapGenerator.CellData> cells = new List<MapGenerator.CellData>();
            while(amount > 0)
            {
                MapGenerator.CellData cell = null;

                while(cell is null)
                    cell = ChoosePosition(angle, delta/2f, size, map);

                cells.Add(cell);
                angle += worldPartAngleSize;
                amount--;
            }

            return cells;
        }

        private MapGenerator.CellData ChoosePosition(float angle, float delta, Vector2Int size, MapGenerator.CellData[,] map)
        {
            Vector2Int half = size / 2;
            float radians = Random.Range(angle - delta, angle + delta) * Mathf.Deg2Rad;
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
            direction *= Random.Range(-1f, 1f);
            Vector2Int cellPosition = new Vector2Int((int)(direction.x * half.x) + half.x, (int)(direction.y * half.y) + half.y);
            return map[cellPosition.x, cellPosition.y];
        }

        public virtual MapGenerator.CellData[,] GenerateMap(Vector2Int size)
        {
            MapGenerator.CellData[,] map = new MapGenerator.CellData[size.x, size.y];
            float offset = _maxOffset * Random.value;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                    map[i, j] = GenerateCell(new Vector2Int(i, j), offset);
            }
            return map;
        }
        private MapGenerator.CellData GenerateCell(Vector2Int position, float offset)
        {
            MapGenerator.CellData cell = new MapGenerator.CellData(position);
            if (_isToUsePerlinNoise)
                cell.height = (int)(Mathf.PerlinNoise(position.x * _step + offset, position.y * _step + offset) * _maxHeight);
            return cell;
        }
    }
}
