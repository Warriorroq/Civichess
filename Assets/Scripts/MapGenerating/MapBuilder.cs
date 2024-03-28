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
        public Cell[,] map;
        public List<Vector2Int> startingKingsPositions;
     
        [SerializeField] private List<Material> _mapMaterials;
        [SerializeField] private Vector3 _cellSize;
        [SerializeField] private Cell _cellPrefab;

        public MapBuilder()
            =>startingKingsPositions = new List<Vector2Int>();
        
        public void GenerateMap(IMapPatternGeneration pattern, Vector2Int size)
        {
            this.size = size;
            this.map = new Cell[size.x, size.y];
            int teamsCount = GameManager.Singleton.party.teams.Count;
            var map = pattern.GenerateMap(size);
            startingKingsPositions = pattern.ChooseKingsPositions(teamsCount, size, map).Select(x => x.positionOnMap).ToList();
            GenerateCellsOnScene(size, map);
        }        

        public void GenerateKingsOnScene()
        {
            Queue<Color> teamsColors = new Queue<Color>(GameManager.Singleton.party.teams.Values.Select(x => x.teamColor).ToList());

            foreach (var position in startingKingsPositions)
                PieceManager.Singleton.AskForKingsSpawnServerRpc(position, teamsColors.Dequeue());
        }

        public CellMap GetMap()
            => new CellMap(size, map);

        public void GenerateCellsOnScene(Vector2Int size, CellData[,] map)
        {
            Transform parentTransform = new GameObject("Map").transform;
            Vector2Int position = new Vector2Int();
            for (; position.x < size.x; position.x++)
            {
                for (; position.y < size.y; position.y++)
                {
                    var instance = GameObject.Instantiate(_cellPrefab, new Vector3(_cellSize.x * position.x, 0, _cellSize.z * position.y), Quaternion.identity);
                    instance.data = map[position.x, position.y];
                    instance.transform.parent = parentTransform;
                    this.map[position.x, position.y] = instance;
                }

                position.y = 0;
            }
        }

        public Material GetMaterialByIndex(int index)
            => _mapMaterials[index % _mapMaterials.Count];


    }
}
