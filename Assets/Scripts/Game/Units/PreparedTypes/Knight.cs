using Assets.Scripts.Game.Units.PieceMovement;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PreparedTypes
{
    public class Knight : Piece
    {
        protected override void SetUpMovementMap()
        {
            List<Movement> movementDirections = new List<Movement>
            {
                new JumpDirection(new Vector2Int(2, 1), 6, true, this)
            };

            movementMap = new MovementMap(movementDirections);
        }
    }
}
