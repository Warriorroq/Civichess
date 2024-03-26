using Assets.Scripts.Game;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace Assets.Scripts.MapGenerating
{
    [Serializable]
    public class Cell : MonoBehaviour
    {
        public MeshRenderer cellRenderer;
        public MeshRenderer hiddenMeshRenderer;
        public CellData cellData = null;
        public Vector2Int cellPositionOnMap;
        public Transform topTransform;
        public UnityEvent onCellDisplay;
        public UnityEvent onCellHide;
        private void Start()
        {
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

            cellData.amountOfViewers.onValueChanged.AddListener(IsCellViewed);
            hiddenMeshRenderer.material = MapManager.GetMaterialByIndex((cellPositionOnMap.x + cellPositionOnMap.y) % 2 + 2);
            if((cellPositionOnMap.x + cellPositionOnMap.y)%10 < 5)
                IsCellViewed(0);
        }

        private void IsCellViewed(int arg0)
        {
            if (arg0 == 0)
                onCellHide.Invoke();
            else
                onCellDisplay.Invoke();
        }

        public override string ToString()
            => $"{cellData}";
    }
}
