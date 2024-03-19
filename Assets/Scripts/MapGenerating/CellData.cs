using Assets.Scripts.MapGenerating.Structures;
using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.MapGenerating
{
    [Serializable]
    public class CellData
    {
        public Transform cellRepresentation;
        public Vector2Int positionOnMap;
        public int height;
        public List<IStructure> structures;
        public bool isWalkable;
        public Piece currentPiece;

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
    }
}
