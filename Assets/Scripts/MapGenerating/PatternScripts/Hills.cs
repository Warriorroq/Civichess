using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.MapGenerating.PatternScripts
{
    public class Hills : Plane
    {
        private float _offset;
        private float _step;        
        private int _maxHeight;

        public Hills()
        {
            _maxHeight = 0;
            _offset = 0;
            _step = 0;
        }

        public Hills(int maxHeight, float step, float maxOffSet)
        {
            _maxHeight = maxHeight;
            _offset = maxOffSet;
            _step = step;
        }
        public void ApplyRandomizedOffset()
            => _offset *= Random.value;

        protected override MapGenerator.CellData GenerateCell(Vector2Int position)
        {
            var cell = base.GenerateCell(position);
            cell.height = (int)(Mathf.PerlinNoise(position.x * _step + _offset, position.y * _step + _offset) * _maxHeight);
            return cell;
        }

        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref _offset);
            serializer.SerializeValue(ref _step);
            serializer.SerializeValue(ref _maxHeight);
        }
    }
}
