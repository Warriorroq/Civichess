using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating.PatternScripts;
using Assets.Scripts.Structures;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.MapGenerating
{
    public class MapManager : MonoNetworkSingleton<MapManager>
    {
        public MapGenerator.CellData[,] Map => mapBuilder.map;
        public MapGenerator mapBuilder;
        public IMapPatternGeneration pattern = new PatternScripts.Plane(true, 5, .1f, 10_000f);
        public void GenerateMapData()
        {
            mapBuilder.GenerateMap(pattern);          
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Game")
                return;

            if (!GameManager.Singleton.isHost)
                return;
            
            mapBuilder.GenerateCellsOnScene();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SyncMapServerRpc()
        {
            SyncMapSizeWithPlayersClientRpc(mapBuilder.size);
            foreach(var cell in mapBuilder.map)
                SyncMapCellWithPlayersClientRpc(cell);
        }

        [ClientRpc]
        public void SyncMapSizeWithPlayersClientRpc(Vector2Int size)
        {
            if (GameManager.Singleton.isHost)
                return;

            mapBuilder.size = size;
            mapBuilder.map = new MapGenerator.CellData[size.x, size.y];
        }

        [ClientRpc]
        public void SyncMapCellWithPlayersClientRpc(MapGenerator.CellData cellData)
        {
            if (GameManager.Singleton.isHost)
                return;

            mapBuilder.map[cellData.positionOnMap.x, cellData.positionOnMap.y] = cellData;
            Debug.Log(cellData);
        }
    }
}
