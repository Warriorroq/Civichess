﻿using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating.PatternScripts;
using Assets.Scripts.Structures;
using Assets.Scripts.Structures.MinMax;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.MapGenerating.Structures.Generators;
using System.Collections;
using TMPro;
namespace Assets.Scripts.MapGenerating
{
    public class MapManager : MonoNetworkSingleton<MapManager>
    {
        public TMP_InputField size;
        public CellMap map;

        public IMapPatternGeneration pattern = new PatternScripts.Terrain(
            new List<PatternScripts.Terrain.TerrainLayer>()
            {
                new PatternScripts.Terrain.TerrainLayer(10_000f, .1f, new IntMinMax(1, 7), new IntMinMax()),
                new PatternScripts.Terrain.TerrainLayer(10_001f, .1f, new IntMinMax(4, 9), new IntMinMax(4, 7))
            }, 
            new List<(IStructureGenerator.Type, IStructureGenerator)>()
            {
                (IStructureGenerator.Type.Forest, new ForestGeneration(.4f, .6f, 10_000f, .1f, new IntMinMax(1, 4))),
            });

        [SerializeField] private MapSceneConstructor _mapSceneConstructor;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Game")
                return;

            var mapBuilder = new MapGenerator(map.size, _mapSceneConstructor);
            mapBuilder.GenerateMap(pattern);
            mapBuilder.GenerateCellsOnScene();
            map = mapBuilder.GetMap();
            if (!GameManager.Singleton.isHost)
                return;

            StartCoroutine(SpawnKings(mapBuilder));
        }

        private IEnumerator SpawnKings(MapGenerator builder)
        {
            yield return new WaitForSeconds(0.1f);
            builder.GenerateKingsOnScene();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            pattern = null;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SyncMapServerRpc()
        {
            try
            {
                var text = size.text.Split('x');
                int x = int.Parse(text[0]);
                int y = int.Parse(text[1]);
                map.size = new Vector2Int(x, y);
            }
            catch{}
            SyncMapSizeWithPlayersClientRpc(map.size);
            SyncPattern();
        }

        private void SyncPattern()
        {
            switch (pattern)
            {
                case PatternScripts.Terrain terrain:
                    terrain.ApplyRandomizedOffset();
                    SyncGeneratingTerrainPatternClientRpc(terrain);
                    break;

                case PatternScripts.Plane plane:
                    SyncGeneratingPlanePatternClientRpc(plane);
                    break;
            }
        }

        [ClientRpc]
        private void SyncMapSizeWithPlayersClientRpc(Vector2Int size)
        {
            if (GameManager.Singleton.isHost)
                return;

            map.size = size;
        }

        [ClientRpc]
        private void SyncGeneratingTerrainPatternClientRpc(PatternScripts.Terrain pattern)
        {
            if (GameManager.Singleton.isHost)
                return;

            this.pattern = pattern;
        }

        [ClientRpc]
        private void SyncGeneratingPlanePatternClientRpc(PatternScripts.Plane pattern)
        {
            if (GameManager.Singleton.isHost)
                return;

            this.pattern = pattern;
        }
    }
}
