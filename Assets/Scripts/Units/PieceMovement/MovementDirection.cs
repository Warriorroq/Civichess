using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Units.PieceMovement
{
    public class MovementDirection
    {
        protected int _maxHeigthDifference;

        public MovementDirection(int maxHeigthDifference)
        {
            _maxHeigthDifference = maxHeigthDifference;
        }

        protected CellMap Map => MapManager.Singleton.map;

        public virtual List<Vector2Int> GetPossibleSquares(Vector2Int positionOnMap)
            => new List<Vector2Int>();

        public virtual bool IsPossibleToMove(Vector2Int currentCellPosition, Vector2Int targetCellPosition)
            => Map[targetCellPosition].isWalkable && Mathf.Abs(Map[currentCellPosition].height - Map[targetCellPosition].height) <= _maxHeigthDifference;
    }
}
