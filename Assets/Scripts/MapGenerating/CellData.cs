﻿using Assets.Scripts.MapGenerating.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public CellData()
        {
            structures = new List<IStructure>();
            positionOnMap = new Vector2Int();
            height = 0;
        }

        public CellData(Vector2Int positionOnMap)
        {
            structures = new List<IStructure>();
            this.positionOnMap = positionOnMap;
            height = 0;
        }

        public override string ToString()
            => $"{positionOnMap} {height} \n {string.Join('\n', structures)}";
    }
}
