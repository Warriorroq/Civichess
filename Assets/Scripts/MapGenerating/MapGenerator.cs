using Assets.Scripts.Game.Units;
using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating.PatternScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.MapGenerating
{
    [Serializable]
    public class MapGenerator
    {
        public Vector2Int size;
        public CellData[,] map;
        public List<Vector2Int> startingKingsPositions;

        [SerializeField] private MapSceneConstructor _mapSceneConstructor;

        public MapGenerator(Vector2Int size, MapSceneConstructor mapSceneConstructor)
        {
            this.size = size;
            map = new CellData[size.x, size.y];
            startingKingsPositions = new List<Vector2Int>();
            _mapSceneConstructor = mapSceneConstructor;
        }

        public Material GetMaterialByIndex(int index)
            => _mapSceneConstructor.GetMaterialByIndex(index);

        public void GenerateMap(IMapPatternGeneration pattern)
        {
            int teamsCount = GameManager.Singleton.party.teams.Count;
            map = pattern.GenerateMap(size);
            startingKingsPositions = pattern.ChooseKingsPositions(teamsCount, size, map).Select(x => x.positionOnMap).ToList();
        }
        
        public void GenerateCellsOnScene()
            =>_mapSceneConstructor.GenerateCellsOnScene(size, map);

        public void GenerateKingsOnScene()
        {
            Queue<Color> teamsColors = new Queue<Color>(GameManager.Singleton.party.teams.Values.Select(x => x.teamColor).ToList());

            foreach (var position in startingKingsPositions)
                PieceManager.Singleton.AskForKingsSpawnServerRpc(position, teamsColors.Dequeue());
        }

        public CellMap GetMap()
            => new CellMap(size, map);
    }
}
