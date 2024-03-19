using Assets.Scripts.GameLobby;
using Assets.Scripts.Game.Units.PieceMovement;
using UnityEngine;
using Assets.Scripts.MapGenerating;

namespace Assets.Scripts.Game.Units
{
    public class Piece : MonoBehaviour
    {
        public ulong Id;
        public bool isAbleToMove = true;
        public Color teamColor;
        public Vector2Int currentPositionOnMap;
        public MovementMap movementMap;
        public virtual bool IsTakeable => true;

        protected virtual void Start()
        {
            if (GameManager.Singleton.isHost)
                RoundManager.Singleton.round.OnValueChanged += ResetPiece;

            SetUpMovementMap();
            PlacePieceOnTheMap();
        }

        public void Init(Vector2Int position, Color color, ulong pieceID)
        {
            Id = pieceID;
            currentPositionOnMap = position;
            teamColor = color;
        }

        private void PlacePieceOnTheMap()
        {
            CellData cellData = MapManager.Singleton.map[currentPositionOnMap];
            transform.position = cellData.cellRepresentation.topTransform.position;
            var meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshRenderer.material = new Material(meshRenderer.material) {color = teamColor};
            cellData.currentPiece = this;
        }

        private void ResetPiece(int previousValue, int newValue)
            => isAbleToMove = true;

        public virtual void OnDestroy()
        {
            if (GameManager.Singleton.isHost)
                RoundManager.Singleton.round.OnValueChanged -= ResetPiece;
        }

        protected virtual void SetUpMovementMap() { }

        public override string ToString()
            => $"{GetType().Name} {Id}";
    }
}
