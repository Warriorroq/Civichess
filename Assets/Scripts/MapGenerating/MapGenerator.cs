using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating.PatternScripts;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
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
            this.map = new CellData[size.x, size.y];
            this.startingKingsPositions = new List<Vector2Int>();
            _mapSceneConstructor = mapSceneConstructor;
        }

        public Material GetMaterialByIndex(int index)
            => _mapSceneConstructor.GetMaterialByIndex(index);
        public void GenerateMap(IMapPatternGeneration pattern)
        {
            int teamsCount = GameLobbyManager.Singleton.party.teams.Count;
            map = pattern.GenerateMap(size);
            startingKingsPositions = pattern.ChooseKingsPositions(teamsCount, size, map).Select(x => x.positionOnMap).ToList();
        }
        
        public void GenerateCellsOnScene()
            =>_mapSceneConstructor.GenerateCellsOnScene(size, map);

        public void GenerateKingsOnScene()
        {
            KingPiece kingPrefab = (Resources.Load("Prefabs/King") as GameObject).GetComponent<KingPiece>();
            Queue<Color> teamsColors = new Queue<Color>(GameLobbyManager.Singleton.party.teams.Values.Select(x => x.teamColor).ToList());

            foreach (var position in startingKingsPositions)
            {
                var instance = GameObject.Instantiate(kingPrefab);
                var instanceNetworkObject = instance.GetComponent<NetworkObject>();
                instanceNetworkObject.Spawn();

                instance.SetCellServerRpc(position);
                instance.teamColor.Value = teamsColors.Dequeue();
            }
        }

        public CellMap GetMap()
            => new CellMap(size, map);
    }
}
