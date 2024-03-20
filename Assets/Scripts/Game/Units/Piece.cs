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
                RoundManager.Singleton.onRoundChange.AddListener(ResetPiece);

            SetUpMovementMap();
            PlacePieceOnTheMap();
            ApplyMaterial();
        }

        public void Init(Vector2Int position, Color color, ulong pieceID)
        {
            Id = pieceID;
            currentPositionOnMap = position;
            teamColor = color;
        }

        private void PlacePieceOnTheMap()
            =>MapManager.Singleton.map[currentPositionOnMap].Occupy(this);

        private void ApplyMaterial()
        {
            var meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshRenderer.material = new Material(meshRenderer.material) { color = teamColor };
        }

        private void ResetPiece()
            => isAbleToMove = true;

        public virtual void OnDestroy()
        {
            if (GameManager.Singleton.isHost)
                RoundManager.Singleton.onRoundChange.RemoveListener(ResetPiece);

            Team team = GameManager.Singleton.party.teams[teamColor];
            team.pieces.Remove(Id);
        }

        protected virtual void SetUpMovementMap() { }

        public override string ToString()
            => $"{GetType().Name} {Id}";
    }
}
