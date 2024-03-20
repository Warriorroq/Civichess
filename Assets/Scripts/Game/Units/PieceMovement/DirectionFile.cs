using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class DirectionFile : DirectionDiagonal
    {
        public DirectionFile(int distance, int maxHeightDifference, bool isAttackable, Piece owner) : base(distance, maxHeightDifference, isAttackable, owner)
        {
            _directions = new List<Vector2Int>() {Vector2Int.up, -Vector2Int.up, Vector2Int.right, -Vector2Int.right};
        }
    }
}
