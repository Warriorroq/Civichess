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

        private void Start()
        {
            cellData = MapManager.Singleton.Map[cellPositionOnMap.x, cellPositionOnMap.y];
            cellData.cellRepresentation = transform;

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

            cellRenderer.material = MapManager.Singleton.mapBuilder.GetMaterialByIndex(cellPositionOnMap.x + cellPositionOnMap.y);

            foreach (var structure in cellData.structures)
                structure.CenerateStructureOnCell(this);
        }

        public override string ToString()
            => $"{cellData}";
    }
}
