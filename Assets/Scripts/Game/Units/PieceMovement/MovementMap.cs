using Assets.Scripts.MapGenerating;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceMovement
{
    public class MovementMap
    {
        private List<Movement> _movementDirections;

        public MovementMap(List<Movement> movementDirections)
        {
            _movementDirections = movementDirections;
        }

        public List<Vector2Int> GetPossibleSquares()
        {
            List<Vector2Int> squares = new List<Vector2Int>();
            foreach (var direction in _movementDirections)
                squares.AddRange(direction.GetPossibleSquares());

            return squares;
        }

        public bool IsPossibleMoveToSquare(Vector2Int targetPositionOnMap)
        {
            foreach (var direction in _movementDirections)
            {
                if (direction.IsPossibleToMove(targetPositionOnMap))
                    return true;
            }

            return false;
        }

        public UnityEngine.Mesh GenerateMesh(Vector2Int positionOnMap, Vector3 cellSize)
             => new Mesh(positionOnMap, GetPossibleSquares(), cellSize).mesh;

        private class Mesh
        {
            public UnityEngine.Mesh mesh;
            public Mesh(Vector2Int positionOnMap, List<Vector2Int> cells, Vector3 cellSize)
            {
                UnityEngine.Mesh mesh = new UnityEngine.Mesh();
                Vector3[] vertices = new Vector3[4 * cells.Count];
                List<int> tris = new List<int>();
                CellMap map = MapManager.Singleton.map;
                Vector3 currentPosition = map[positionOnMap].topTransform.position + cellSize / 2f;
                int i = 0;
                foreach (var cell in cells)
                {
                    var delta = map[cell].topTransform.position - currentPosition;
                    vertices[i + i * 3] = delta + Vector3.up * .1f;
                    vertices[i + 1 + i * 3] = delta + Vector3.right * cellSize.x + Vector3.up * .1f;
                    vertices[i + 2 + i * 3] = delta + Vector3.forward * cellSize.z + Vector3.up * .1f;
                    vertices[i + 3 + i * 3] = delta + Vector3.right * cellSize.x + Vector3.forward * cellSize.z + Vector3.up * .1f;

                    tris.AddRange(new int[] { i + i * 3, i + 2 + i * 3, i + 1 + i * 3 });
                    tris.AddRange(new int[] { i + 2 + i * 3, i + 3 + i * 3, i + 1 + i * 3 });
                    i += 1;
                }
                mesh.vertices = vertices;
                mesh.triangles = tris.ToArray();
                this.mesh = mesh;
            }
        }
    }
}
