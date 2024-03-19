using Assets.Scripts.GameLobby;
using System;
using UnityEngine;

namespace Assets.Scripts.MapGenerating
{
    [Serializable]
    public class Cell : MonoBehaviour
    {
        public MeshRenderer cellRenderer;
        public CellData cellData = null;
        public Vector2Int cellPositionOnMap;
        public Transform topTransform;
        [SerializeField] private MeshRenderer _cellHighlight;
        private void Start()
        {
            _cellHighlight.material.color = GameLobbyManager.Singleton.player.color;
            cellData = MapManager.Singleton.map[cellPositionOnMap.x, cellPositionOnMap.y];
            cellData.cellRepresentation = this;

            if (cellData is not null)
                ApplyCellDataOnCell();
        }

        public void SetCellPosition(Vector2Int cellPosition)
            => cellPositionOnMap = cellPosition;

        private void ApplyCellDataOnCell()
        {
            Vector3 additionalScale = transform.localScale.y * Vector3.up * cellData.height;
            transform.localScale = transform.localScale + additionalScale;
            transform.localPosition += additionalScale / 2;

            foreach (var structure in cellData.structures)
                structure.CenerateStructureOnCell(this);
        }

        public void ToggleCellHighLight(bool state)
            =>_cellHighlight.gameObject.SetActive(state);

        public override string ToString()
            => $"{cellData}";
    }
}
