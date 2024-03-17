using Assets.Scripts.Units.PieceMovement;
using System.Collections.Generic;

namespace Assets.Scripts.Units
{
    public class KingPiece : Piece
    {
        protected override void SetUpMovementMap()
        {
            List<MovementDirection> movementDirections = new List<MovementDirection>();
            movementDirections.Add(new MovementDirectionDiagonal(1, 1));
            _movementMap = new MovementMap(movementDirections);
        }
    }
}
