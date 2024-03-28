using Assets.Scripts.Game.Units.PieceMovement;
using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Units.PreparedTypes
{
    public class King : Piece
    {
        protected override void Start()
        {
            base.Start();
            Party party = GameManager.Singleton.party;
            party.teams[teamColor].king = this;
        }
        protected override void SetUpMovementMap()
        {
            List<Movement> movementDirections = new List<Movement>
            {
                new DirectionDiagonal(1, 1, true, this),
                new DirectionFile(1, 1, true, this),
            };

            movementMap = new MovementMap(movementDirections);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Party party = GameManager.Singleton.party;
            party.teams[teamColor].king = null;
            foreach(var piece in party.teams[teamColor].pieces)
                piece.Value.couldBeenUsed.Value = false;
            party.teams[teamColor].pieces.Clear();
            MapManager.Singleton.map.DisplayAllCellsOnMap();
        }
    }
}
