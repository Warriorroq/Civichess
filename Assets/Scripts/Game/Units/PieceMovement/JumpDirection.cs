using Assets.Scripts.Extensions;
using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class JumpDirection : Movement
    {
        protected Vector2Int _pattern;

        public JumpDirection(Vector2Int pattern, int maxHeigthDifference, bool isAttackable, Piece owner) : base(maxHeigthDifference, isAttackable, owner)
        {
            _pattern = pattern;
        }

        public override List<Vector2Int> GetPossibleSquares()
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();
            possibleSquares.AddRange(GetPossibleSquaresInDirection(_pattern));
            possibleSquares.AddRange(GetPossibleSquaresInDirection(new Vector2Int(-_pattern.x, _pattern.y)));
            return possibleSquares;
        }

        protected List<Vector2Int> GetPossibleSquaresInDirection(Vector2Int pattern)
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();
            int possibleSteps = 4;
            Vector2Int currentSquare = _owner.positionOnMap;
            for(; possibleSteps > 0; possibleSteps--)
            {
                Vector2Int patternTemp = pattern;
                for (int i = 0; i < possibleSteps; i++)
                    patternTemp = patternTemp.Rotate90();

                Vector2Int newSquare = patternTemp + currentSquare;

                if (!Map.IsPositionIsInBox(newSquare))
                    continue;

                CellData data = Map[newSquare].data;

                if (!data.CouldBeOccupiedByPiece(_owner))
                    continue;

                if (data.HeightDifferenceWithCell(Map[currentSquare].data) > _maxHeigthDifference)
                    continue;

                if (!_isAttackable && data.currentPiece is not null)
                    continue;

                possibleSquares.Add(newSquare);
            }

            return possibleSquares;
        }

        public override bool IsPossibleToMove(Vector2Int targetCellPosition)
        {
            Vector2Int vector = targetCellPosition - _owner.positionOnMap;
            if (Mathf.Abs(vector.y * vector.x) != Mathf.Abs(_pattern.y * _pattern.x))
                return false;

            if (GetPossibleSquaresInDirection(_pattern).Contains(targetCellPosition))
                return true;

            if (GetPossibleSquaresInDirection(new Vector2Int(-_pattern.x, _pattern.y)).Contains(targetCellPosition))
                return true;

            return false;
        }
    }
}
