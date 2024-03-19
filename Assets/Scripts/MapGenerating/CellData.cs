using Assets.Scripts.MapGenerating.Structures;
using Assets.Scripts.Game.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGenerating
{
    [Serializable]
    public class CellData
    {
        public Cell cellRepresentation;
        public Vector2Int positionOnMap;
        public int height;
        public List<IStructure> structures;
        public bool isWalkable;
        public Piece currentPiece;
        public bool IsWalkable
        {
            get
            {
                if (currentPiece is not null)
                    return currentPiece.IsTakeable;
                return isWalkable;
            }
        }

        public CellData(Vector2Int positionOnMap = new Vector2Int())
        {
            structures = new List<IStructure>();
            this.positionOnMap = positionOnMap;
            height = 0;
            isWalkable = true;
            currentPiece = null;
            cellRepresentation = null;
        }

        public int GetMovementPenalty()
        {
            return 1;
        }

        public override string ToString()
            => $"{positionOnMap} {height} {isWalkable}\n {string.Join('\n', structures)} \n {currentPiece}";

        public int HeightDifferenceWithCell(CellData cellData)
            => Mathf.Abs(height - cellData.height);
    }
}
