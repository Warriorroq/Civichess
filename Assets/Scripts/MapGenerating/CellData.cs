using Assets.Scripts.MapGenerating.Structures;
using Assets.Scripts.Game.Units;
using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Structures;

namespace Assets.Scripts.MapGenerating
{
    [Serializable]
    public class CellData
    {
        public Vector2Int positionOnMap;
        public int height;
        public List<IStructure> structures;
        public bool isWalkable;
        public Piece currentPiece;
        public EventValue<int> amountOfViewers;
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
            amountOfViewers = new EventValue<int>();
        }

        public bool CouldBeOccupiedByPiece(Piece piece)
        {
            if (currentPiece is not null && currentPiece.teamColor == piece.teamColor)
                return false;

            if (!IsWalkable)
                return false;

            return true;
        }

        public int GetMovementPenalty()
        {
            int penalty = 1;
            foreach (IStructure structure in structures)
                penalty += structure.GetMovementPenalty();
            return penalty;
        }

        public float GetVisibilityPenalty()
        {
            float penalty = 1;
            foreach (IStructure structure in structures)
                penalty += structure.GetVisibilityPenalty();
            return penalty;
        }

        public override string ToString()
            => $"{positionOnMap} {height} {isWalkable}\n {string.Join('\n', structures)} \n {currentPiece}";

        public int HeightDifferenceWithCell(CellData cellData)
            => Mathf.Abs(height - cellData.height);
    }
}
