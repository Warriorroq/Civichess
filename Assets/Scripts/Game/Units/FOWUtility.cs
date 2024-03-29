using Assets.Scripts.MapGenerating;
using Assets.Scripts.Structures;
using System;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    [Serializable]
    public class FOWUtility : IDisposable
    {
        public EventValue<float> visibilityRadius;
        public EventValue<int> maxSubstractiveHeightDifference;
        public const int errorOffset = 0;
        public void FOWAffectCellsFromPiece(Vector2Int position, int difference = 1)
        {
            var map = MapManager.Singleton.map;          
            map[position].data.amountOfViewers.Value += difference;
            foreach (var target in map.GetPointsOfCircle(position, visibilityRadius.Value + errorOffset))
                AffectPointsInLine(position, target, difference);

            AffectDiagonalPoints(position, difference);
        }

        private void AffectDiagonalPoints(Vector2Int position, int difference)
        {
            int distance = (int)(visibilityRadius.Value/2);
            AffectPointsInLine(position, position + Vector2Int.one * distance, difference);
            AffectPointsInLine(position, position - Vector2Int.one * distance, difference);
            AffectPointsInLine(position, position + new Vector2Int(1, -1) * distance, difference);
            AffectPointsInLine(position, position - new Vector2Int(1, -1) * distance, difference);
        }

        private void AffectPointsInLine(Vector2Int position, Vector2Int target, int difference)
        {
            var map = MapManager.Singleton.map;
            float currentVisibility = visibilityRadius.Value;
            CellData cellData = map[position].data;
            int lastHeight = cellData.height;
            int startHeight = cellData.height;
            foreach (var point in map.GetPointsInLine(position, target))
            {
                if (!map.IsPositionIsInBox(point))
                    break;

                CellData pointData = map[point].data;
                currentVisibility -= pointData.GetVisibilityPenalty();
                if (currentVisibility <= 0)
                    break;

                if (lastHeight - pointData.height < -maxSubstractiveHeightDifference.Value)
                    currentVisibility = 0;

                if (lastHeight > startHeight)
                    break;

                lastHeight = pointData.height;
                pointData.amountOfViewers.Value += difference;
                lastHeight = pointData.height;
            }
        }

        public void Dispose()
        {
            visibilityRadius.RemoveAllListeners();
            visibilityRadius = null;
            maxSubstractiveHeightDifference.RemoveAllListeners();
            maxSubstractiveHeightDifference = null;
        }
    }
}
