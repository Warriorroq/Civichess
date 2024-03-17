using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating.PatternScripts;
using Assets.Scripts.Structures;
using Assets.Scripts.Structures.MinMax;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts.MapGenerating.Structures.Generators;
namespace Assets.Scripts.MapGenerating
{
    public class MapManager : MonoNetworkSingleton<MapManager>
    {
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
            if (!GameLobbyManager.Singleton.isHost)
                return;

            mapBuilder.GenerateKingsOnScene();
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
            if (GameLobbyManager.Singleton.isHost)
                return;

            map.size = size;
        }

        [ClientRpc]
        private void SyncGeneratingTerrainPatternClientRpc(PatternScripts.Terrain pattern)
        {
            if (GameLobbyManager.Singleton.isHost)
                return;

            this.pattern = pattern;
        }

        [ClientRpc]
        private void SyncGeneratingPlanePatternClientRpc(PatternScripts.Plane pattern)
        {
            if (GameLobbyManager.Singleton.isHost)
                return;

            this.pattern = pattern;
        }
    }
}
