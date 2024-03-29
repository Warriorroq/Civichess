using Assets.Scripts.Game.Units;
using Assets.Scripts.GameLobby;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.MapGenerating
{
    [Serializable]
    public class Cell : MonoBehaviour
    {
        public MeshRenderer cellRenderer;
        public MeshRenderer hiddenMeshRenderer;

        public CellData data = null;
        public Transform topTransform;

        public UnityEvent onCellDisplay;
        public UnityEvent onCellHide;
        private void Start()
        {
            if (data is not null)
                ApplyCellDataOnCell();
        }

        private void ApplyCellDataOnCell()
        {
            Vector3 additionalScale = transform.localScale.y * Vector3.up * data.height;
            transform.localScale = transform.localScale + additionalScale;
            transform.localPosition += additionalScale / 2;

            foreach (var structure in data.structures)
                structure.CenerateStructureOnCell(this);

            data.amountOfViewers.onValueChanged.AddListener(IsCellViewed);
            cellRenderer.material = MapManager.GetMaterialByIndex((data.positionOnMap.x + data.positionOnMap.y) % 2);
            hiddenMeshRenderer.material = MapManager.GetMaterialByIndex((data.positionOnMap.x + data.positionOnMap.y) % 2 + 2);
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
            => $"{data}";

        public void HidePiece()
        {
            if (data.currentPiece is null)
                return;

            if (data.amountOfViewers.Value > 0)
                return;

            foreach (var renderer in data.currentPiece.GetComponentsInChildren<MeshRenderer>())
                renderer.enabled = false;
        }

        public void DisplayPiece()
        {
            if(data.currentPiece is null) 
                return;

            if (data.amountOfViewers.Value == 0)
                return;

            foreach (var renderer in data.currentPiece.GetComponentsInChildren<MeshRenderer>())
                renderer.enabled = true;
        }

        public void Occupy(Piece piece)
        {
            if (data.currentPiece is not null)
                GameObject.Destroy(data.currentPiece.gameObject);

            data.OnOccupyByPiece(piece);
            data.currentPiece = piece;
            piece.transform.position = topTransform.position;
            data.amountOfViewers.Value = data.amountOfViewers.Value;

            if (piece.teamColor == GameManager.CurrentTeam.teamColor)
                piece.fowUtility.FOWAffectCellsFromPiece(piece.positionOnMap);
        }

        public void DeOccupy()
        {
            Piece currentPiece = data.currentPiece;
            if (currentPiece.teamColor == GameManager.CurrentTeam.teamColor)
                currentPiece.fowUtility.FOWAffectCellsFromPiece(currentPiece.positionOnMap, -1);

            data.amountOfViewers.Value = data.amountOfViewers.Value;
            data.currentPiece = null;
        }
    }
}
