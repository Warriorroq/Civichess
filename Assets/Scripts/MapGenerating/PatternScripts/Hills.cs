using System.Collections.Generic;
using Assets.Scripts.Structures.MinMax;

namespace Assets.Scripts.MapGenerating.PatternScripts
{
    public class Hills : Terrain
    {

        public Hills()
        {
            _terrainLayers = new List<TerrainLayer>
            {
                new TerrainLayer(10_000f, .1f, new IntMinMax(1, 7), new IntMinMax())
            };
        }
    }
}
