using Assets.Scripts.Game.Units.PieceMovement;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Units.PreparedTypes
{
    public class Rook : Piece
    {
        protected override void SetUpMovementMap()
        {
            List<Movement> movementDirections = new List<Movement>
            {
                new DirectionFile(7, 1, true, this)
            };

            movementMap = new MovementMap(movementDirections);
        }
    }
}
