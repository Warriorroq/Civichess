﻿
using Assets.Scripts.Extensions;
using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class PawnDiagonalAttack : Movement
    {
        private static List<Vector2Int> _attackDirections = new List<Vector2Int>() {
            Vector2Int.one, -Vector2Int.one, new Vector2Int(1, -1), new Vector2Int(-1, 1)
        };

        public PawnDiagonalAttack(int maxHeigthDifference, Piece owner) : base(maxHeigthDifference, true, owner)
        {
            
        }

        public override List<Vector2Int> GetPossibleSquares()
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();

            if (IsPossibleToMove(_owner.positionOnMap + Vector2Int.one))
                possibleSquares.Add(_owner.positionOnMap + Vector2Int.one);
            if (IsPossibleToMove(_owner.positionOnMap - Vector2Int.one))
                possibleSquares.Add(_owner.positionOnMap - Vector2Int.one);
            if (IsPossibleToMove(_owner.positionOnMap + new Vector2Int(1, -1)))
                possibleSquares.Add(_owner.positionOnMap + new Vector2Int(1, -1));
            if (IsPossibleToMove(_owner.positionOnMap + new Vector2Int(-1, 1)))
                possibleSquares.Add(_owner.positionOnMap + new Vector2Int(-1, 1));

            return possibleSquares;
        }

        public override bool IsPossibleToMove(Vector2Int targetCellPosition)
        {
            int possibleSteps = 1;
            Vector2Int lastSquare = _owner.positionOnMap;
            Vector2Int direction = (targetCellPosition - lastSquare).ToOneVector();
            if(!_attackDirections.Contains(direction))
                return false;

            while (possibleSteps > 0)
            {
                Vector2Int newSquare = direction + lastSquare;
                if (!Map.IsPositionIsInBox(newSquare))
                    return false;

                CellData data = Map[newSquare].data;

                if (data.HeightDifferenceWithCell(Map[lastSquare].data) > _maxHeigthDifference)
                    return false;

                if (!data.CouldBeOccupiedByPiece(_owner))
                    return false;

                if(data.currentPiece is null)
                    return false;

                possibleSteps -= data.GetMovementPenalty();
                lastSquare = newSquare;

                if (lastSquare == targetCellPosition)
                    return true;
            }

            return false;
        }
    }
}
