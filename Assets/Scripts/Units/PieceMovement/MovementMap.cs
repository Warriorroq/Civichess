using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Units.PieceMovement
{
    [Serializable]
    public class MovementMap
    {
        private List<MovementDirection> _movementDirections;

        public MovementMap(List<MovementDirection> movementDirections)
        {
            _movementDirections = movementDirections;
        }

        public List<Vector2Int> GetPossibleSquares(Vector2Int positionOnMap)
        {
            List<Vector2Int> squares = new List<Vector2Int>();
            foreach (var direction in _movementDirections)
                squares.AddRange(direction.GetPossibleSquares(positionOnMap));

            return squares;
        }

        public bool IsPossibleMoveToSquare(Vector2Int positionOnMap, Vector2Int targetPositionOnMap)
        {
            foreach (var direction in _movementDirections)
            {
                if (direction.IsPossibleToMove(positionOnMap, targetPositionOnMap))
                    return true;
            }

            return false;
        }
    }
}
