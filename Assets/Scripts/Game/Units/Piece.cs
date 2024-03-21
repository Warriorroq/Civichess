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
        public bool CouldBeenUsed
        {
            get => _couldBeenUsed;
            set
            {
                onUsedStateChange.Invoke(value);
                _couldBeenUsed = value;
            }
        }
        public Color teamColor;
        public Vector2Int currentPositionOnMap;
        public MovementMap movementMap;
        public virtual bool IsTakeable => true;

        public UnityEvent<bool> onUsedStateChange;
        [SerializeField] protected bool _couldBeenUsed;

        protected virtual void Start()
        {
            RoundManager.Singleton.onRoundChange.AddListener(ResetPiece);
            SetUpMovementMap();
            PlacePieceOnTheMap();
            CouldBeenUsed = true;
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
        {
            Party party = GameManager.Singleton.party;
            CouldBeenUsed = party.teams[teamColor].king is not null;
        }

        protected virtual void OnDestroy()
        {
            RoundManager.Singleton.onRoundChange.RemoveListener(ResetPiece);
            Team team = GameManager.Singleton.party.teams[teamColor];
            team.pieces.Remove(Id);
        }

        protected virtual void SetUpMovementMap() { }

        public override string ToString()
            => $"{GetType().Name} {Id}";
    }
}
