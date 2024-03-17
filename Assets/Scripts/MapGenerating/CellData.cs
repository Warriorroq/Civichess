using Assets.Scripts.MapGenerating.Structures;
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

        public CellData(Vector2Int positionOnMap = new Vector2Int())
        {
            structures = new List<IStructure>();
            this.positionOnMap = positionOnMap;
            height = 0;
            isWalkable = true;
        }

        public override string ToString()
            => $"{positionOnMap} {height} \n {string.Join('\n', structures)}";
    }
}
