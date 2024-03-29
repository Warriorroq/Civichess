using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class Movement
    {
        protected int _maxHeigthDifference;
        protected bool _isAttackable;
        protected Piece _owner;
        protected CellMap Map => MapManager.Singleton.map;

        public Movement(int maxHeigthDifference, bool isAttackable, Piece owner)
        {
            _maxHeigthDifference = maxHeigthDifference;
            _isAttackable = isAttackable;   
            _owner = owner;
        }

        public virtual List<Vector2Int> GetPossibleSquares()
            => new List<Vector2Int>();

        public virtual bool IsPossibleToMove(Vector2Int targetCellPosition)
            => Map[targetCellPosition].data.isWalkable && Mathf.Abs(Map[_owner.positionOnMap].data.height - Map[targetCellPosition].data.height) <= _maxHeigthDifference;
    }
}
