﻿using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating.PatternScripts;
using System;
using System.Collections.Generic;
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

        public Material GetMaterialByIndex(int index)
            => _mapSceneConstructor.GetMaterialByIndex(index);
        public void GenerateMap(IMapPatternGeneration pattern)
        {
            int teamsCount = GameLobbyManager.Singleton.party.teams.Count;
            map = pattern.GenerateMap(size);
            //startingKingsPositions = pattern.ChooseKingsPositions(teamsCount, size, map).Select(x => x.positionOnMap).ToList();
        }
        
        public void GenerateCellsOnScene()
        {
            _mapSceneConstructor.GenerateCellsOnScene(size, map);
        }
    }
}
