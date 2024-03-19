using Assets.Scripts.Game.Units.PieceMovement;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Units
{
    public class KingPiece : Piece
    {
        protected override void SetUpMovementMap()
        {
            List<Movement> movementDirections = new List<Movement>
            {
                new DirectionDiagonal(1, 1, true, this),
                new DirectionFile(1, 1, true, this),
            };

            movementMap = new MovementMap(movementDirections);
        }
    }
}
