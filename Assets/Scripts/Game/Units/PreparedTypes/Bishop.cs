using Assets.Scripts.Game.Units.PieceMovement;
using System.Collections.Generic;
namespace Assets.Scripts.Game.Units.PreparedTypes
{
    public class Bishop : Piece
    {
        protected override void SetUpMovementMap()
        {
            List<Movement> movementDirections = new List<Movement>
            {
                new DirectionDiagonal(10, 1, true, this),
            };

            movementMap = new MovementMap(movementDirections);
        }
    }
}
