using Assets.Scripts.MapGenerating;
using Assets.Scripts.Units.PieceMovement;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Units
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
                new MovementDirectionDiagonal(1, 1, true)
            };

            _movementMap = new MovementMap(movementDirections);
        }
    }
}
