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
        public Vector2Int positionOnMap;
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
            positionOnMap = position;
            teamColor = color;
        }

        private void PlacePieceOnTheMap()
            =>MapManager.Singleton.map[positionOnMap].Occupy(this);

        private void ResetPiece()
        {
            Party party = GameManager.Singleton.party;
            couldBeenUsed.Value = party.teams[teamColor].king is not null;
        }

        protected virtual void OnDestroy()
        {
            RoundManager.Singleton.onRoundChange.RemoveListener(ResetPiece);
            couldBeenUsed.RemoveAllListeners();

            Team team = GameManager.Singleton.party.teams[teamColor];
            team.pieces.Remove(Id);
            if (teamColor == GameManager.CurrentTeam.teamColor)
                fowUtility.FOWAffectCellsFromPiece(positionOnMap, -1);

            fowUtility.Dispose();
        }

        protected virtual void SetUpMovementMap() { }

        public override string ToString()
            => $"{GetType().Name} {Id}";
    }
}
