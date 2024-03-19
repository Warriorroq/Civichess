using Assets.Scripts.MapGenerating;
using Assets.Scripts.Game.Units.PieceMovement;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class KingPiece : Piece
    {
        protected override void Start()
        {
            base.Start();
            CellData cellData = MapManager.Singleton.map[currentPositionOnMap];
            transform.position = cellData.cellRepresentation.GetComponent<Cell>().topTransform.position;
            var meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshRenderer.material = new Material(meshRenderer.material);
            meshRenderer.material.color = teamColor;
            cellData.currentPiece = this;
        }

        protected override void SetUpMovementMap()
        {
            List<MovementDirection> movementDirections = new List<MovementDirection>
            {
                new MovementDirectionDiagonal(10, 1, true),
                new MovementDirectionFile(14, 1, true),
            };

            movementMap = new MovementMap(movementDirections);
        }
    }
}
