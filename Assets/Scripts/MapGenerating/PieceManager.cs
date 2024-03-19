using Assets.Scripts.Structures;
using Assets.Scripts.Units;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.MapGenerating
{
    public class PieceManager : MonoNetworkSingleton<PieceManager>
    {
        private KingPiece _kingPrefab;

        [ServerRpc]
        public void AskForKingsSpawnServerRpc(Vector2Int position, Color color)
            => SpawnKingOnMapClientRpc(position, color);

        [ClientRpc]
        public void SpawnKingOnMapClientRpc(Vector2Int position, Color color)
        {
            if (_kingPrefab is null)
                _kingPrefab = (Resources.Load("Prefabs/King") as GameObject).GetComponent<KingPiece>();

            var instance = GameObject.Instantiate(_kingPrefab);
            instance.currentPositionOnMap = position;
            instance.teamColor = color;
        }
    }
}
