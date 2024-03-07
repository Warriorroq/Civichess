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
        public MapGenerator.CellData cellData;
        public Vector2Int cellPositionOnMap;

        private void Start()
        {
            cellData = MapManager.Singleton.Map[cellPositionOnMap.x, cellPositionOnMap.y];
            Vector3 additionalScale = transform.localScale.y * Vector3.up * cellData.height;
            transform.localScale = transform.localScale + additionalScale;
            transform.localPosition += additionalScale / 2;
            cellData.cellRepresentation = transform;
            cellRenderer.material = MapManager.Singleton.mapBuilder.GetMaterialByIndex(cellPositionOnMap.x + cellPositionOnMap.y);
        }

        [ClientRpc]
        public void SetCellPositionOnMapClientRpc(Vector2Int cellPositionOnMap)
        {
            this.cellPositionOnMap = cellPositionOnMap;
        }
    }
}
