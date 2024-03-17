using Assets.Scripts.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Units.PieceMovement
{
    public class MovementDirectionDiagonal : MovementDirection
    {
        protected int _distance;

        public MovementDirectionDiagonal(int distance, int maxHeightDifference)  : base(maxHeightDifference)
        {
            _distance = distance;
        }

        public override List<Vector2Int> GetPossibleSquares(Vector2Int positionOnMap)
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();
            possibleSquares.AddRange(GetPossibleSquaresInDirection(positionOnMap, Vector2Int.one));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(positionOnMap, -Vector2Int.one));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(positionOnMap, new Vector2Int(1, -1)));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(positionOnMap, new Vector2Int(-1, 1)));

            return possibleSquares;
        }

        private List<Vector2Int> GetPossibleSquaresInDirection(Vector2Int positionOnMap, Vector2Int direction)
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();
            int possibleSteps = _distance;
            Vector2Int lastSquare = positionOnMap;
            while (possibleSteps > 0)
            {
                Vector2Int newSquare = direction + lastSquare;
                if (!Map.IsPositionIsInBox(newSquare))
                    break;

                if (!Map[newSquare].isWalkable)
                    break;

                if (Mathf.Abs(Map[newSquare].height - Map[lastSquare].height) > _maxHeigthDifference)
                    break;

                possibleSteps -= Map[newSquare].GetMovementPenalty();
                lastSquare = newSquare;
            }

            return possibleSquares;
        }

        public override bool IsPossibleToMove(Vector2Int currentCellPosition, Vector2Int targetCellPosition)
        {
            int possibleSteps = _distance;
            Vector2Int lastSquare = currentCellPosition;
            Vector2Int direction = (targetCellPosition - currentCellPosition).ToOneVector();
            while(lastSquare != targetCellPosition && possibleSteps > 0)
            {
                Vector2Int newSquare = direction + lastSquare;
                if (!Map.IsPositionIsInBox(newSquare))
                    return false;

                if (!Map[newSquare].isWalkable)
                    return false;

                if (Mathf.Abs(Map[newSquare].height - Map[lastSquare].height) > _maxHeigthDifference)
                    return false;

                possibleSteps -= Map[newSquare].GetMovementPenalty();
                lastSquare = newSquare;
            }

            return true;
        }
    }
}
