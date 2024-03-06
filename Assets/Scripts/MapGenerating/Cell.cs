using Assets.Scripts.GameLobby;
using System;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.MapGenerating
{
    [Serializable]
    public class Cell : NetworkBehaviour
    {
        public MeshRenderer cellRenderer;
        public MapGenerator.CellData mapCellData;
        public Vector2Int cellPositionOnMap;

        private void Start()
        {
            mapCellData = MapManager.Singleton.Map[cellPositionOnMap.x, cellPositionOnMap.y];
            mapCellData.cellRepresentation = transform;
            cellRenderer.material = MapManager.Singleton.mapBuilder.GetMaterialByIndex(cellPositionOnMap.x + cellPositionOnMap.y);
        }

        [ClientRpc]
        public void SetCellPositionOnMapClientRpc(Vector2Int cellPositionOnMap)
        {
            this.cellPositionOnMap = cellPositionOnMap;
        }
    }
}
