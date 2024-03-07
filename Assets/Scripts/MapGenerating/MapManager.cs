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

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
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

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SyncMapServerRpc()
        {
            SyncMapSizeWithPlayersClientRpc(mapBuilder.size);
            if (pattern is PatternScripts.Plane)
            {
                (pattern as PatternScripts.Plane).ApplyOffset();
                SyncGeneratingPlanePatternClientRpc((PatternScripts.Plane)pattern);
            }
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
        public void SyncGeneratingPlanePatternClientRpc(PatternScripts.Plane pattern)
        {
            if (GameManager.Singleton.isHost)
                return;

            this.pattern = pattern;
        }
    }
}
