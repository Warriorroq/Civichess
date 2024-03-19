using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class MovementMap
    {
        private List<Movement> _movementDirections;

        public MovementMap(List<Movement> movementDirections)
        {
            _movementDirections = movementDirections;
        }

        public List<Vector2Int> GetPossibleSquares()
        {
            List<Vector2Int> squares = new List<Vector2Int>();
            foreach (var direction in _movementDirections)
                squares.AddRange(direction.GetPossibleSquares());

            return squares;
        }

        public bool IsPossibleMoveToSquare(Vector2Int targetPositionOnMap)
        {
            foreach (var direction in _movementDirections)
            {
                if (direction.IsPossibleToMove(targetPositionOnMap))
                    return true;
            }

            return false;
        }
    }
}
