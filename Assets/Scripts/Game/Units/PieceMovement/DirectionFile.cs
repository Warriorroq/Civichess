using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class DirectionFile : DirectionDiagonal
    {
        public DirectionFile(int distance, int maxHeightDifference, bool isAttackable, Piece owner) : base(distance, maxHeightDifference, isAttackable, owner) {}

        public override List<Vector2Int> GetPossibleSquares()
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();
            possibleSquares.AddRange(GetPossibleSquaresInDirection(Vector2Int.up));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(-Vector2Int.up));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(Vector2Int.right));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(-Vector2Int.right));

            return possibleSquares;
        }
    }
}
