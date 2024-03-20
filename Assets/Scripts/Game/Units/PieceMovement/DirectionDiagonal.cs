using Assets.Scripts.Extensions;
using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class DirectionDiagonal : Movement
    {
        protected int _distance;
        protected List<Vector2Int> _directions;
        public DirectionDiagonal(int distance, int maxHeightDifference, bool isAttackable, Piece owner)  : base(maxHeightDifference, isAttackable, owner)
        {
            _distance = distance;
            _directions = new List<Vector2Int>() { Vector2Int.one, -Vector2Int.one, new Vector2Int(1, -1), new Vector2Int(-1, 1) };
        }

        public override List<Vector2Int> GetPossibleSquares()
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();
            foreach (var direction in _directions)
                possibleSquares.AddRange(GetPossibleSquaresInDirection(direction));

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

                if (!_isAttackable && cell.currentPiece is not null)
                    break;

                possibleSteps -= cell.GetMovementPenalty();
                lastSquare = newSquare;
                possibleSquares.Add(lastSquare);
            }

            return possibleSquares;
        }

        public override bool IsPossibleToMove(Vector2Int targetCellPosition)
        {
            Vector2Int direction = (targetCellPosition - _owner.currentPositionOnMap).ToOneVector();
            if(!_directions.Contains(direction))
                return false;

            if (GetPossibleSquaresInDirection(direction).Contains(targetCellPosition))
                return true;

            return false;
        }
    }
}
