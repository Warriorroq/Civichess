using Assets.Scripts.Units;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.MapGenerating
{
    [Serializable]
    public class MapSceneConstructor
    {
        [SerializeField] private List<Material> _mapMaterials;
        [SerializeField] private Vector3 _cellSize;
        [SerializeField] private Cell _cellPrefab;

        public void GenerateCellsOnScene(Vector2Int size, CellData[,] map)
        {
            Transform parentTransform = new GameObject("Map").transform;
            Vector2Int position = new Vector2Int();
            for (; position.x < size.x; position.x++)
            {
                for (; position.y < size.y; position.y++)
                {
                    var instance = GameObject.Instantiate(_cellPrefab, new Vector3(_cellSize.x * position.x, 0, _cellSize.z * position.y), Quaternion.identity);
                    instance.SetCellPosition(position);
                    instance.transform.parent = parentTransform;
                    instance.cellRenderer.material = GetMaterialByIndex(position.x + position.y);
                }

                position.y = 0;
            }
        }

        public Material GetMaterialByIndex(int index)
            => _mapMaterials[index % _mapMaterials.Count];
    }
}
