using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Assets.Scripts.Units.PieceMovement
{
    [Serializable]
    public class MovementDirection
    {
        public Type type;
        public int maxHeigthDifference;
        protected CellMap Map => MapManager.Singleton.map;

        public virtual List<Vector2Int> GetPossibleSquares(Vector2Int positionOnMap)
            => new List<Vector2Int>();

        public virtual bool IsPossibleToMove(Vector2Int currentCellPosition, Vector2Int targetCellPosition)
            => Map[targetCellPosition].isWalkable && Mathf.Abs(Map[currentCellPosition].height - Map[targetCellPosition].height) <= maxHeigthDifference;

        public enum Type : short
        {
            Movement,
            Jump,
        }
    }
}
