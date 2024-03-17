using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating;
using Assets.Scripts.Units.PieceMovement;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Units
{
    public class Piece : NetworkBehaviour
    {
        public NetworkVariable<bool> isAbleToMove = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public NetworkVariable<Color> teamColor = new NetworkVariable<Color>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        public CellData currentCell;
        protected MovementMap _movementMap;

        protected virtual void Start()
        {
            if(GameLobbyManager.Singleton.isHost)
                RoundManager.Singleton.round.OnValueChanged += ResetPiece;

            SetUpMovementMap();
            transform.position = currentCell.cellRepresentation.GetComponent<Cell>().topTransform.position;
            var meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshRenderer.material = new Material(meshRenderer.material);
            meshRenderer.material.color = teamColor.Value;
        }

        private void ResetPiece(int previousValue, int newValue)
            =>isAbleToMove.Value = true;

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (GameLobbyManager.Singleton.isHost)
                RoundManager.Singleton.round.OnValueChanged -= ResetPiece;
        }

        [ClientRpc]
        public void MoveToCellClientRpc(Vector2Int cellDataPositionOnMap)
        {
            if (!_movementMap.IsPossibleMoveToSquare(currentCell.positionOnMap, cellDataPositionOnMap))
                return;

            currentCell.currentPiece = null;
            currentCell = MapManager.Singleton.map[cellDataPositionOnMap];
            currentCell.currentPiece = this;
            transform.position = currentCell.cellRepresentation.transform.position + Vector3.up * currentCell.height / 2f;
        }

        [ServerRpc]
        public void SetCellServerRpc(Vector2Int cellPositionOnMap)
            => SetCellClientRpc(cellPositionOnMap);

        [ClientRpc]
        public void SetCellClientRpc(Vector2Int cellPositionOnMap)
        {
            currentCell = MapManager.Singleton.map[cellPositionOnMap];
            currentCell.currentPiece = this;
        }

        protected virtual void SetUpMovementMap(){}
    }
}
