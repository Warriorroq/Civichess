using Assets.Scripts.Game.Commands;
using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.PreparedTypes;
using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating;
using Assets.Scripts.Structures;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Game.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private EventValue<Cell> _currentCell;

        protected virtual void Awake()
        {
            if (_meshFilter is not null)
            {
                _meshFilter.GetComponent<MeshRenderer>().material.color = GameManager.CurrentTeam.teamColor;
                return;
            }

            _meshFilter = new GameObject("Movement Mesh").AddComponent<MeshFilter>();
            _meshFilter.AddComponent<MeshRenderer>().material.color = GameManager.CurrentTeam.teamColor;
        }

        public void ApplyMeshRenderer(Cell cell)
        {
            _meshFilter.mesh = null;
            if (cell is null)
                return;

            Piece piece = cell.data.currentPiece;
            if (piece is null)
                return;
            
            if (piece.teamColor != GameManager.CurrentTeam.teamColor) 
                return;

            _meshFilter.mesh = piece.movementMap.GenerateMesh(piece.positionOnMap, new Vector3(.8f, 0, .8f));
            _meshFilter.transform.position = cell.topTransform.position;
        }

        public void SelectPiece(InputAction.CallbackContext context)
        {
            if (!FindCellByRayCastFromCamera(out Cell cell))
                return;

            _currentCell.Value = cell;
        }

        public void PlaceCity(InputAction.CallbackContext context)
        {
            if (_currentCell.Value is null)
                return;

            if (_currentCell.Value.data.currentPiece is not King king)
                return;

            if (!City.IsPossibleToPlaceCityOnCell(_currentCell.Value))
                return;

            CommandManager.Singleton.AskForApprovingCommandServerRpc(new CityPlacementCommand(king.teamColor, king.positionOnMap));
            _currentCell.Value = null;
        }

        public void MovePiece(InputAction.CallbackContext context)
        {
            if (_currentCell.Value is null)
                return;

            Piece piece = _currentCell.Value.data.currentPiece;
            if (piece is null)
                return;

            if (!FindCellByRayCastFromCamera(out Cell cell))
                return;

            Vector2Int targetPosition = cell.data.positionOnMap;
            if (!piece.movementMap.IsPossibleMoveToSquare(targetPosition))
                return;

            CommandManager.Singleton.AskForApprovingCommandServerRpc(new MovementCommand(targetPosition, piece.teamColor, piece.Id));
            _currentCell.Value = null;
        }

        private bool FindCellByRayCastFromCamera(out Cell cell)
        {
            cell = null;
            Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 100))
                return false;

            if (hit.collider.gameObject.TryGetComponent(out cell))
                return true;

            return false;
        }
    }
}
