using Assets.Scripts.Game.Units.PieceMovement;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Units.PreparedTypes
{
    public class Pawn : Piece
    {
        protected override void SetUpMovementMap()
        {
            List<Movement> movementDirections = new List<Movement>
            {
                new PawnDiagonalAttack(1, this),
                new DirectionFile(1, 1, false, this),
            };

            movementMap = new MovementMap(movementDirections);
        }
    }
}
