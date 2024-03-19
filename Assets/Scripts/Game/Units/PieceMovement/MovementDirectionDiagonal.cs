using Assets.Scripts.Extensions;
using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class MovementDirectionDiagonal : MovementDirection
    {
        protected int _distance;

        public MovementDirectionDiagonal(int distance, int maxHeightDifference, bool isAttackable)  : base(maxHeightDifference, isAttackable)
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

        protected List<Vector2Int> GetPossibleSquaresInDirection(Vector2Int positionOnMap, Vector2Int direction)
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();
            int possibleSteps = _distance;
            Vector2Int lastSquare = positionOnMap;
            while (possibleSteps > 0)
            {
                Vector2Int newSquare = direction + lastSquare;
                if (!Map.IsPositionIsInBox(newSquare))
                    break;

                CellData cell = Map[newSquare];
                if (!cell.IsWalkable)
                    break;

                if (cell.HeightDifferenceWithCell(Map[lastSquare]) > _maxHeigthDifference)
                    break;

                if (cell.currentPiece is not null && !_isAttackable)
                    break;

                possibleSteps -= cell.GetMovementPenalty();
                lastSquare = newSquare;
                possibleSquares.Add(lastSquare);
                if (cell.currentPiece is not null)
                    break;
            }

            return possibleSquares;
        }

        public override bool IsPossibleToMove(Vector2Int currentCellPosition, Vector2Int targetCellPosition)
        {
            int possibleSteps = _distance;
            Vector2Int lastSquare = currentCellPosition;
            Vector2Int direction = (targetCellPosition - currentCellPosition).ToOneVector();
            while(possibleSteps > 0)
            {
                Vector2Int newSquare = direction + lastSquare;
                if (!Map.IsPositionIsInBox(newSquare))
                    return false;

                CellData cell = Map[newSquare];
                if (!cell.IsWalkable)
                    return false;

                if (cell.HeightDifferenceWithCell(Map[lastSquare]) > _maxHeigthDifference)
                    return false;

                if (cell.currentPiece is not null && !_isAttackable)
                    return false;

                possibleSteps -= cell.GetMovementPenalty();
                lastSquare = newSquare;

                if(lastSquare == targetCellPosition && cell.IsWalkable)
                    return true;
            }

            return true;
        }
    }
}
