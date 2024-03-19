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
        private Pawn _pawnPrefab;

        [ServerRpc]
        public void AskForKingsSpawnServerRpc(Vector2Int position, Color color)
        {
            SpawnKingOnMapClientRpc(position, color, _lastID++);
            SpawnPawnOnMapClientRpc(position + Vector2Int.one, color, _lastID++);
        }

        [ClientRpc]
        public void SpawnKingOnMapClientRpc(Vector2Int position, Color color, ulong pieceID)
        {
            if (_kingPrefab is null)
                _kingPrefab = (Resources.Load("Prefabs/King") as GameObject).GetComponent<KingPiece>();

            var instance = GameObject.Instantiate(_kingPrefab);
            instance.Init(position, color, pieceID);
            AddPieceToTeam(instance);
        }

        [ClientRpc]
        public void SpawnPawnOnMapClientRpc(Vector2Int position, Color color, ulong pieceID)
        {
            if (_pawnPrefab is null)
                _pawnPrefab = (Resources.Load("Prefabs/Pawn") as GameObject).GetComponent<Pawn>();

            var instance = GameObject.Instantiate(_pawnPrefab);
            instance.Init(position, color, pieceID);
            AddPieceToTeam(instance);
        }

        private void AddPieceToTeam(Piece piece)
        {
            Team team = GameManager.Singleton.party.teams[piece.teamColor];
            team.pieces.Add(piece.Id, piece);
        }
    }
}
