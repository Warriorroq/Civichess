using Assets.Scripts.MapGenerating.Structures.Generators;
using Assets.Scripts.Structures.MinMax;
using System.Collections.Generic;

namespace Assets.Scripts.MapGenerating.PatternScripts
{
    public class DefaultTerrain : Terrain
    {
        public DefaultTerrain()
        {
            _terrainLayers = new List<TerrainLayer>()
            {
                new TerrainLayer(10_000f, .1f, new IntMinMax(1, 7), new IntMinMax()),
                new TerrainLayer(10_001f, .1f, new IntMinMax(4, 9), new IntMinMax(4, 7))
            };

            _structureGenerations = new List<(IStructureGenerator.Type, IStructureGenerator)>()
            {
                (IStructureGenerator.Type.Forest, new ForestGeneration(.4f, .6f, 10_000f, .1f, new IntMinMax(1, 4))),
            };
        }
    }
}
