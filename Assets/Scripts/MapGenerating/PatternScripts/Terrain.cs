using Assets.Scripts.Structures.MinMax;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.MapGenerating.PatternScripts
{
    public class Terrain : Plane
    {
        private List<TerrainLayer> _terrainLayers;
        public Terrain(){}
        public Terrain(List<TerrainLayer> layers)
        {
            _terrainLayers = layers;
        }

        public void ApplyRandomizedOffset()
        {
            foreach (var layer in _terrainLayers)
                layer.maxOffset *= Random.value;
        }
        protected override MapGenerator.CellData GenerateCell(Vector2Int position)
        {
            var cell = base.GenerateCell(position);
            foreach (var terrain in _terrainLayers)
                terrain.ApplyGenerationToCell(cell);

            return cell;
        }

        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            int length = 0;
            TerrainLayer[] readList = null;

            if (serializer.IsWriter)
            {
                length = _terrainLayers.Count;
                readList = _terrainLayers.ToArray();
            }

            serializer.SerializeValue(ref length);

            if(readList is null)
            {
                readList = new TerrainLayer[length];
                for (int i = 0; i < length; i++)
                    readList[i] = new TerrainLayer();
            }

            for (int n = 0; n < length; n++)
                serializer.SerializeValue(ref readList[n]);

            if (serializer.IsReader)
                _terrainLayers = new List<TerrainLayer>(readList);
        }

        public class TerrainLayer : INetworkSerializable
        {
            public float maxOffset;
            public float step;
            public IntMinMax minMaxHeight;
            public IntMinMax mask;

            public TerrainLayer() { 
                minMaxHeight = new IntMinMax();
                mask = new IntMinMax(); 
            }

            public TerrainLayer(float offset, float step, IntMinMax minMaxHeight, IntMinMax mask)
            {
                this.maxOffset = offset;
                this.step = step;
                this.minMaxHeight = minMaxHeight;
                this.mask = mask;
            }

            public void ApplyGenerationToCell(MapGenerator.CellData cell)
            {
                if (mask is null)
                    return;

                if (!mask.IsValueInRange(cell.height))
                    return;

                var heightPercent = Mathf.PerlinNoise(cell.positionOnMap.x * step + maxOffset, cell.positionOnMap.y * step + maxOffset);
                cell.height = minMaxHeight.Clamp((int)(heightPercent * minMaxHeight.max));
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref maxOffset);
                serializer.SerializeValue(ref step);
                serializer.SerializeValue(ref minMaxHeight);
                serializer.SerializeValue(ref mask);
            }
        }
    }
}
