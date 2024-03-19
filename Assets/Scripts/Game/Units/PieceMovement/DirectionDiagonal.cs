using Assets.Scripts.Extensions;
using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class DirectionDiagonal : Movement
    {
        protected int _distance;

        public DirectionDiagonal(int distance, int maxHeightDifference, bool isAttackable, Piece owner)  : base(maxHeightDifference, isAttackable, owner)
        {
            _distance = distance;
        }

        public override List<Vector2Int> GetPossibleSquares()
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();
            possibleSquares.AddRange(GetPossibleSquaresInDirection(Vector2Int.one));
            possibleSquares.AddRange(GetPossibleSquaresInDirection( -Vector2Int.one));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(new Vector2Int(1, -1)));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(new Vector2Int(-1, 1)));

            return possibleSquares;
        }

        protected List<Vector2Int> GetPossibleSquaresInDirection(Vector2Int direction)
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();
            int possibleSteps = _distance;
            Vector2Int lastSquare = _owner.currentPositionOnMap;
            while (possibleSteps > 0)
            {
                Vector2Int newSquare = direction + lastSquare;
                if (!Map.IsPositionIsInBox(newSquare))
                    break;

                CellData cell = Map[newSquare];

                if (!cell.CouldBeOccupiedByPiece(_owner))
                    break;

                if (cell.HeightDifferenceWithCell(Map[lastSquare]) > _maxHeigthDifference)
                    break;

                if (_isAttackable && cell.currentPiece is not null)
                {
                    possibleSquares.Add(newSquare);
                    break;
                }

                possibleSteps -= cell.GetMovementPenalty();
                lastSquare = newSquare;
                possibleSquares.Add(lastSquare);
            }

            return possibleSquares;
        }

        public override bool IsPossibleToMove(Vector2Int targetCellPosition)
        {
            int possibleSteps = _distance;
            Vector2Int lastSquare = _owner.currentPositionOnMap;
            Vector2Int direction = (targetCellPosition - lastSquare).ToOneVector();
            while(possibleSteps > 0)
            {
                Vector2Int newSquare = direction + lastSquare;
                if (!Map.IsPositionIsInBox(newSquare))
                    return false;

                CellData cell = Map[newSquare];
                if (!cell.CouldBeOccupiedByPiece(_owner))
                    return false;

                if (cell.HeightDifferenceWithCell(Map[lastSquare]) > _maxHeigthDifference)
                    return false;

                if (_isAttackable && cell.currentPiece is not null)
                    return newSquare == targetCellPosition;

                possibleSteps -= cell.GetMovementPenalty();
                lastSquare = newSquare;

                if(lastSquare == targetCellPosition)
                    return true;
            }

            return true;
        }
    }
}
