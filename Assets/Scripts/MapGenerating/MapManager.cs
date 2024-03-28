using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating.PatternScripts;
using Assets.Scripts.Structures;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Assets.Scripts.MapGenerating
{
    public class MapManager : MonoNetworkSingleton<MapManager>
    {
        public CellMap map;
        public IMapPatternGeneration pattern = new DefaultTerrain();
        [SerializeField] private MapBuilder _builder;

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Game")
                return;

            _builder.GenerateMap(pattern, map.size);
            map = _builder.GetMap();
            StartCoroutine(SpawnKings(_builder));
        }

        private IEnumerator SpawnKings(MapBuilder builder)
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

        public static Material GetMaterialByIndex(int index)
            => Singleton._builder.GetMaterialByIndex(index);
    }
}
