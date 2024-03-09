using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating.PatternScripts;
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
        public MapGenerator.CellData[,] map;
        public List<Vector2Int> startingKingsPositions;

        [SerializeField] private MapSceneConstructor _mapSceneConstructor;

        public Material GetMaterialByIndex(int index)
            => _mapSceneConstructor.GetMaterialByIndex(index);
        public void GenerateMap(IMapPatternGeneration pattern)
        {
            int teamsCount = GameManager.Singleton.party.teams.Count;
            map = pattern.GenerateMap(size);
            //startingKingsPositions = pattern.ChooseKingsPositions(teamsCount, size, map).Select(x => x.positionOnMap).ToList();
        }
        
        public void GenerateCellsOnScene()
        {
            _mapSceneConstructor.GenerateCellsOnScene(size, map);
        }

        [Serializable]
        public class CellData : INetworkSerializable
        {
            public Transform cellRepresentation;
            public Vector2Int positionOnMap;
            public int height;

            public CellData()
            {
                this.positionOnMap = new Vector2Int();
                height = 0;
            }

            public CellData(Vector2Int positionOnMap)
            {
                this.positionOnMap = positionOnMap;
                height = 0;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref positionOnMap);
                serializer.SerializeValue(ref height);
            }

            public override string ToString()
                => $"{positionOnMap} {height}";
        }
    }
}
