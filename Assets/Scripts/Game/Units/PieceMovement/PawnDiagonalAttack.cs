﻿
using Assets.Scripts.Extensions;
using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class PawnDiagonalAttack : Movement
    {
        public PawnDiagonalAttack(int maxHeigthDifference, Piece owner) : base(maxHeigthDifference, true, owner)
        {

        }
        public override List<Vector2Int> GetPossibleSquares()
        {
            List<Vector2Int> possibleSquares = new List<Vector2Int>();

            if (IsPossibleToMove(_owner.currentPositionOnMap + Vector2Int.one))
                possibleSquares.Add(_owner.currentPositionOnMap + Vector2Int.one);
            if (IsPossibleToMove(_owner.currentPositionOnMap - Vector2Int.one))
                possibleSquares.Add(_owner.currentPositionOnMap - Vector2Int.one);
            if (IsPossibleToMove(_owner.currentPositionOnMap + new Vector2Int(1, -1)))
                possibleSquares.Add(_owner.currentPositionOnMap + new Vector2Int(1, -1));
            if (IsPossibleToMove(_owner.currentPositionOnMap + new Vector2Int(-1, 1)))
                possibleSquares.Add(_owner.currentPositionOnMap + new Vector2Int(-1, 1));

            return possibleSquares;
        }

        public override bool IsPossibleToMove(Vector2Int targetCellPosition)
        {
            int possibleSteps = 1;
            Vector2Int lastSquare = _owner.currentPositionOnMap;
            Vector2Int direction = (targetCellPosition - lastSquare).ToOneVector();
            while (possibleSteps > 0)
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

                if (lastSquare == targetCellPosition)
                    return true;
            }

            return true;
        }
    }
}
