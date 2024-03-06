using System.Collections.Generic;
using UnityEngine;
namespace Assets.Scripts.MapGenerating.PatternScripts
{
    public interface IMapPatternGeneration
    {
        public abstract MapGenerator.CellData[,] GenerateMap(Vector2Int size);
        public abstract List<MapGenerator.CellData> ChooseKingsPositions(int amount, Vector2Int size, MapGenerator.CellData[,] map);       
    }
}
