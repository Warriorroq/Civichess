using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.MapGenerating.PatternScripts
{
    public class Plane : IMapPatternGeneration
    {
        private float _offset;
        private float _step;
        private bool _isToUsePerlinNoise;
        private int _maxHeight;
        public Plane()
        {
            _isToUsePerlinNoise = false;
            _maxHeight = 0;
            _offset = 0;
            _step = 0;
        }

        public Plane(bool usePerlinNoise, int maxHeight, float step, float maxOffSet) { 
            _isToUsePerlinNoise = usePerlinNoise;
            _maxHeight = maxHeight;
            _offset = maxOffSet;
            _step = step;
        }

        public void ApplyOffset()
            => _offset*= Random.value;

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
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                    map[i, j] = GenerateCell(new Vector2Int(i, j));
            }
            return map;
        }
        private MapGenerator.CellData GenerateCell(Vector2Int position)
        {
            MapGenerator.CellData cell = new MapGenerator.CellData(position);
            if (_isToUsePerlinNoise)
                cell.height = (int)(Mathf.PerlinNoise(position.x * _step + _offset, position.y * _step + _offset) * _maxHeight);
            return cell;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _offset);
            serializer.SerializeValue(ref _step);
            serializer.SerializeValue(ref _isToUsePerlinNoise);
            serializer.SerializeValue(ref _maxHeight);
        }
    }
}
