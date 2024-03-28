using Assets.Scripts.MapGenerating;
using Assets.Scripts.Structures;
using System;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    [Serializable]
    public class FOWUtility
    {
        public Piece piece;
        public EventValue<float> visibilityRadius;
        public EventValue<int> maxSubstractiveHeightDifference;
        public const int errorOffset = 0;
        public void FOWAffectCellsFromPiece(int difference = 1)
        {
            var map = MapManager.Singleton.map;
            Vector2Int position = piece.currentPositionOnMap;
            map[position].data.amountOfViewers.Value += difference;
            foreach (var target in map.GetPointsOfCircle(position, visibilityRadius.Value + errorOffset))
                AffectPointsInLine(position, target, difference);

            AffectDiagonalPoints(difference);
        }

        private void AffectDiagonalPoints(int difference)
        {
            Vector2Int position = piece.currentPositionOnMap;
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
            CellData cellData = map[piece.currentPositionOnMap].data;
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
    }
}
