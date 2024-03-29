using Assets.Scripts.Game.Units;
using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating;
using Assets.Scripts.MapGenerating.Structures;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour, IStructure
{
    public Color teamColor;
    public Cell cell;
    public FOWUtility fOWUtility;
    [SerializeField] private List<MeshRenderer> _meshRenderers;

    public const float visibilityPenalty = 0f;
    public const int cityDistanceRadius = 5;
    public static City objPrefab;

    public static void LoadPrefab()
    {
        if(objPrefab is null)
            objPrefab = (Resources.Load("Prefabs/City") as GameObject).GetComponent<City>();
    }

    public static bool IsPossibleToPlaceCityOnCell(Cell cell)
    {
        Vector2Int offset = Vector2Int.one * cityDistanceRadius;
        Vector2Int point = cell.data.positionOnMap;
        Vector2Int currentPoint = point - offset;
        Vector2Int endPosition = point + offset;
        CellMap map = MapManager.Singleton.map;

        for (;currentPoint.x <= endPosition.x ; currentPoint.x++)
        {
            int y = currentPoint.y;
            for (; currentPoint.y <= endPosition.y; currentPoint.y++)
            {
                if (!IsWithinCircle(currentPoint, point))
                    continue;

                if (!map.IsPositionIsInBox(currentPoint))
                    continue;

                if (map[currentPoint].data.ContainsStructureOfType<City>())
                    return false;
            }
            currentPoint.y = y;
        }

        return true;
    }

    private static bool IsWithinCircle(Vector2Int point, Vector2Int center)
    {
        float distanceSquared = (point - center).sqrMagnitude;
        return distanceSquared <= cityDistanceRadius * cityDistanceRadius;
    }

    public void Init(Color teamColor, Cell cell)
    {
        this.teamColor = teamColor; 
        this.cell = cell;
        CenerateStructureOnCell(cell);

        if(teamColor == GameManager.CurrentTeam.teamColor)
            fOWUtility.FOWAffectCellsFromPiece(cell.data.positionOnMap);
    }

    public void CenerateStructureOnCell(Cell cell)
    {
        transform.position = cell.topTransform.position;
        var structures = cell.data.structures;

        foreach(var render in _meshRenderers)
            render.material.color = teamColor;
        
        if (!structures.Contains(this))
            structures.Add(this);
    }

    public int GetMovementPenalty()
        => 0;

    public float GetVisibilityPenalty()
        => visibilityPenalty;

    public void OnOccupy(Piece piece)
    {
        if (piece.teamColor == teamColor)
            return;

        cell.data.structures.Remove(this);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (teamColor == GameManager.CurrentTeam.teamColor)
            fOWUtility.FOWAffectCellsFromPiece(cell.data.positionOnMap, -1);
        fOWUtility.Dispose();
    }
}
