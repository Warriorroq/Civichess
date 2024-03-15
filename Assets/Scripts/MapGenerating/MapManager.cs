using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating.PatternScripts;
using Assets.Scripts.Structures;
using Assets.Scripts.Structures.MinMax;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MapGenerating
{
    public class MapManager : MonoNetworkSingleton<MapManager>
    {
        public CellData[,] Map => mapBuilder.map;
        public MapGenerator mapBuilder;
        public IMapPatternGeneration pattern = new Hills();
            /*= new PatternScripts.Terrain(new List<PatternScripts.Terrain.TerrainLayer>()
            {
                new PatternScripts.Terrain.TerrainLayer(10_000f, .1f, new IntMinMax(1, 7), new IntMinMax()),
                new PatternScripts.Terrain.TerrainLayer(10_001f, .1f, new IntMinMax(4, 9), new IntMinMax(4, 7))
            });*/

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Game")
                return;

            mapBuilder.GenerateMap(pattern);
            mapBuilder.GenerateCellsOnScene();
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
            SyncMapSizeWithPlayersClientRpc(mapBuilder.size);
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

            mapBuilder.size = size;
            mapBuilder.map = new CellData[size.x, size.y];
        }

        [ClientRpc]
        private void SyncGeneratingTerrainPatternClientRpc(PatternScripts.Terrain pattern)
        {
            //if (GameManager.Singleton.isHost)
            //    return;

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
