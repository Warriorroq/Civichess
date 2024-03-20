using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.PieceCommand;
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
            if (!FindCellByRayCastFromCamera(out Cell cell))
                return;

            Piece piece = cell.cellData.currentPiece;
            if (piece is null)
                return;

            if(piece.teamColor == GameManager.CurrentPlayer.color)
                Piece = piece;
        }

        public void MovePiece(InputAction.CallbackContext context)
        {
            if (Piece is null)
                return;

            if (!FindCellByRayCastFromCamera(out Cell cell))
                return;

            Vector2Int targetPosition = cell.cellPositionOnMap;
            if (!Piece.movementMap.IsPossibleMoveToSquare(targetPosition))
                return;

            CommandManager.Singleton.AskForApprovingCommandServerRpc(new MovementCommand(targetPosition, Piece.teamColor, Piece.Id));
            Piece = null;
        }

        private void ToggleCurrentCells(bool state)
        {
            foreach (var cellPos in _cells)
                MapManager.Singleton.map[cellPos].cellRepresentation.ToggleCellHighLight(state);
        }

        private bool FindCellByRayCastFromCamera(out Cell cell)
        {
            cell = null;
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 100, _mask))
                return false;

            if (hit.collider.gameObject.TryGetComponent(out cell))
                return true;

            return false;
        }
    }
}
