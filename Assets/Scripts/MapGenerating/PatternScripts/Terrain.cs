using Assets.Scripts.Extensions;
using Assets.Scripts.MapGenerating.Structures.Generators;
using Assets.Scripts.Structures.MinMax;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.MapGenerating.PatternScripts
{
    public class Terrain : Plane
    {
        protected List<(IStructureGenerator.Type, IStructureGenerator)> _structureGenerations;
        protected List<TerrainLayer> _terrainLayers;
        public Terrain(){}
        public Terrain(List<TerrainLayer> layers, List<(IStructureGenerator.Type, IStructureGenerator)> structureGenerators)
        {
            _terrainLayers = layers;
            _structureGenerations = structureGenerators;
        }

        public void ApplyRandomizedOffset()
        {
            foreach (var layer in _terrainLayers)
                layer.maxOffset *= Random.value;

            foreach(var layer in _structureGenerations)
                layer.Item2.ApplyRandomization();
        }
        protected override CellData GenerateCell(Vector2Int position)
        {
            var cell = base.GenerateCell(position);
            foreach (var terrain in _terrainLayers)
                terrain.ApplyGenerationToCell(cell);

            foreach (var generator in _structureGenerations)
                generator.Item2.ApplyGenerationToCell(cell);

            return cell;
        }

        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            SyncTerrainLayers(serializer);
            SyncStructureLayers(serializer);
        }

        private void SyncTerrainLayers<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            var newList = serializer.SerializeList(_terrainLayers);
            if (newList is not null)
                _terrainLayers = newList;
        }

        private void SyncStructureLayers<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsWriter)
                serializer.SerializeList(_structureGenerations.Select(x => (int)x.Item1).ToList());

            if (serializer.IsWriter)
                serializer.SerializeList(_structureGenerations.Select(x => x.Item2).ToList());

            if (serializer.IsWriter)
                return;

            List<IStructureGenerator.Type> structureGenerationTypes = serializer
                .SerializeList(new List<int>())
                .Select(x => (IStructureGenerator.Type)x)
                .ToList();


            List<(IStructureGenerator.Type, IStructureGenerator)> structureGenerations = new();
            if (serializer.IsReader)
            {
                int length = 0;
                serializer.SerializeValue(ref length);
                for (int i = 0; i < length; i++)
                {
                    IStructureGenerator structureGenerator = IStructureGenerator.dictionaryOfTemporaryGeneratorsAndTypes[structureGenerationTypes[i]];
                    serializer.SerializeValue(ref structureGenerator);
                    structureGenerations.Add((structureGenerationTypes[i], structureGenerator.Clone() as IStructureGenerator));
                }
            }

            _structureGenerations = structureGenerations;
        }

        public sealed class TerrainLayer : INetworkSerializable
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

            public void ApplyGenerationToCell(CellData cell)
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
