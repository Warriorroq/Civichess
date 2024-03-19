using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class MovementDirectionFile : MovementDirectionDiagonal
    {
        public MovementDirectionFile(int distance, int maxHeightDifference, bool isAttackable) : base(distance, maxHeightDifference, isAttackable){}

        public override List<Vector2Int> GetPossibleSquares(Vector2Int positionOnMap)
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();
            possibleSquares.AddRange(GetPossibleSquaresInDirection(positionOnMap, Vector2Int.up));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(positionOnMap, -Vector2Int.up));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(positionOnMap, Vector2Int.right));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(positionOnMap, -Vector2Int.right));

            return possibleSquares;
        }
    }
}
