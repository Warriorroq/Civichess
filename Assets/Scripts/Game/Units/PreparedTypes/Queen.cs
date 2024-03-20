using Assets.Scripts.Game.Units.PieceMovement;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Units.PreparedTypes
{
    public class Queen : Piece
    {
        protected override void SetUpMovementMap()
        {
            List<Movement> movementDirections = new List<Movement>
            {
                new DirectionDiagonal(10, 1, true, this),
                new DirectionFile(14, 1, true, this),
            };

            movementMap = new MovementMap(movementDirections);
        }
    }
}
