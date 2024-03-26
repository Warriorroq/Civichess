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
    public class MapBuilder
    {
        public Vector2Int size;
        public CellData[,] map;
        public List<Vector2Int> startingKingsPositions;
        private MapSceneConstructor _mapSceneConstructor;

        public MapBuilder(Vector2Int size, MapSceneConstructor mapSceneConstructor)
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

        [Serializable]
        public class MapSceneConstructor
        {
            [SerializeField] private List<Material> _mapMaterials;
            [SerializeField] private Vector3 _cellSize;
            [SerializeField] private Cell _cellPrefab;

            public void GenerateCellsOnScene(Vector2Int size, CellData[,] map)
            {
                Transform parentTransform = new GameObject("Map").transform;
                Vector2Int position = new Vector2Int();
                for (; position.x < size.x; position.x++)
                {
                    for (; position.y < size.y; position.y++)
                    {
                        var instance = GameObject.Instantiate(_cellPrefab, new Vector3(_cellSize.x * position.x, 0, _cellSize.z * position.y), Quaternion.identity);
                        instance.SetCellPosition(position);
                        instance.transform.parent = parentTransform;
                        instance.cellRenderer.material = GetMaterialByIndex((position.x + position.y) % 2);
                    }

                    position.y = 0;
                }
            }

            public Material GetMaterialByIndex(int index)
                => _mapMaterials[index % _mapMaterials.Count];
        }
    }
}
