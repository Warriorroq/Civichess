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

        public void GenerateCellsOnScene(Vector2Int size, MapGenerator.CellData[,] map)
        {
            Vector2Int position = new Vector2Int();
            for (; position.x < size.x; position.x++)
            {
                for (; position.y < size.y; position.y++)
                {
                    var instance = GameObject.Instantiate(_cellPrefab, new Vector3(_cellSize.x * position.x, 0, _cellSize.z * position.y), Quaternion.identity);
                    var instanceNetworkObject = instance.GetComponent<NetworkObject>();
                    instanceNetworkObject.Spawn();
                    instance.SetCellPositionOnMapClientRpc(position);
                }

                position.y = 0;
            }
        }

        public Material GetMaterialByIndex(int index)
            => _mapMaterials[index % _mapMaterials.Count];
    }
}
