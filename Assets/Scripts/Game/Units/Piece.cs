using Assets.Scripts.GameLobby;
using Assets.Scripts.Game.Units.PieceMovement;
using UnityEngine;
using Assets.Scripts.MapGenerating;
using Assets.Scripts.Structures;

namespace Assets.Scripts.Game.Units
{
    public class Piece : MonoBehaviour
    {
        public ulong Id;
        public EventValue<bool> couldBeenUsed;
        public Color teamColor;
        public Vector2Int currentPositionOnMap;
        public MovementMap movementMap;
        public FOWUtility fowUtility;
        public virtual bool IsTakeable => true;

        protected virtual void Start()
        {
            RoundManager.Singleton.onRoundChange.AddListener(ResetPiece);
            SetUpMovementMap();
            PlacePieceOnTheMap();
            couldBeenUsed.Value = true;
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
            couldBeenUsed.Value = party.teams[teamColor].king is not null;
            if (couldBeenUsed.Value)
                return;

            couldBeenUsed.onValueChanged.RemoveAllListeners();
            enabled = couldBeenUsed.Value;
        }

        protected virtual void OnDestroy()
        {
            RoundManager.Singleton.onRoundChange.RemoveListener(ResetPiece);
            Team team = GameManager.Singleton.party.teams[teamColor];
            team.pieces.Remove(Id);
            couldBeenUsed.onValueChanged.RemoveAllListeners();
            if (teamColor == GameManager.CurrentTeam.teamColor)
                fowUtility.FOWAffectCellsFromPiece();
        }

        protected virtual void SetUpMovementMap() { }

        public override string ToString()
            => $"{GetType().Name} {Id}";
    }
}
