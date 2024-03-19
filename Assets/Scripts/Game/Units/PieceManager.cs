using Assets.Scripts.GameLobby;
using Assets.Scripts.Structures;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class PieceManager : MonoNetworkSingleton<PieceManager>
    {
        private static ulong _lastID = 0;
        private KingPiece _kingPrefab;

        [ServerRpc]
        public void AskForKingsSpawnServerRpc(Vector2Int position, Color color)
            => SpawnKingOnMapClientRpc(position, color, _lastID++);

        [ClientRpc]
        public void SpawnKingOnMapClientRpc(Vector2Int position, Color color, ulong pieceID)
        {
            if (_kingPrefab is null)
                _kingPrefab = (Resources.Load("Prefabs/King") as GameObject).GetComponent<KingPiece>();

            var instance = GameObject.Instantiate(_kingPrefab);
            instance.Id = pieceID;
            instance.currentPositionOnMap = position;
            instance.teamColor = color;
            AddPieceToTeam(instance);
        }

        private void AddPieceToTeam(Piece piece)
        {
            Team team = GameManager.Singleton.party.teams[piece.teamColor];
            team.pieces.Add(piece.Id, piece);
        }
    }
}
