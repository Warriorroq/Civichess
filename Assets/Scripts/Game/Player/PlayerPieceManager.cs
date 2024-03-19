using Assets.Scripts.Game.Units;
using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Game.Player
{
    public class PlayerPieceManager : MonoBehaviour
    {
        public Piece Piece
        {
            get => _piece;
            set
            {
                ToggleCurrentCells(false);
                if(value is null)
                {
                    _piece = null;
                    _cells.Clear();
                    return;
                }

                _cells = value.movementMap.GetPossibleSquares();
                ToggleCurrentCells(true);
                _piece = value;
            }
        }

        [SerializeField] private List<Vector2Int> _cells = new List<Vector2Int>();
        [SerializeField] private Piece _piece;
        [SerializeField] protected LayerMask _mask;
        public void SelectPiece(InputAction.CallbackContext context)
        {
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 100, _mask))
                return;

            if (!hit.collider.gameObject.TryGetComponent(out Cell cell))
                return;

            Piece piece = cell.cellData.currentPiece;
            if (piece is null)
                return;

            if(piece.teamColor == GameManager.CurrentPlayer.color)
                Piece = piece;
        }

        public void MovePiece(InputAction.CallbackContext context)
        {
            //Movement
            //Piece = null;
        }

        private void ToggleCurrentCells(bool state)
        {
            foreach (var cellPos in _cells)
                MapManager.Singleton.map[cellPos].cellRepresentation.ToggleCellHighLight(state);
        }
    }
}
