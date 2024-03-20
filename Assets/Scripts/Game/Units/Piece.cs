using Assets.Scripts.GameLobby;
using Assets.Scripts.Game.Units.PieceMovement;
using UnityEngine;
using Assets.Scripts.MapGenerating;
using UnityEngine.Events;

namespace Assets.Scripts.Game.Units
{
    public class Piece : MonoBehaviour
    {
        public ulong Id;
        public bool HasBeenUsed
        {
            get => _hasBeenUsed;
            set
            {
                _hasBeenUsed = value;
                onUsedStateChange.Invoke(_hasBeenUsed);
            }
        }
        public Color teamColor;
        public Vector2Int currentPositionOnMap;
        public MovementMap movementMap;
        public virtual bool IsTakeable => true;

        public UnityEvent<bool> onUsedStateChange;
        protected bool _hasBeenUsed;

        protected virtual void Start()
        {
            if (GameManager.Singleton.isHost)
                RoundManager.Singleton.onRoundChange.AddListener(ResetPiece);

            SetUpMovementMap();
            PlacePieceOnTheMap();
            HasBeenUsed = true;
        }

        public void Init(Vector2Int position, Color color, ulong pieceID)
        {
            Id = pieceID;
            currentPositionOnMap = position;
            teamColor = color;
        }

        private void PlacePieceOnTheMap()
            =>MapManager.Singleton.map[currentPositionOnMap].Occupy(this);

        private void ResetPiece()
            => HasBeenUsed = true;

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
