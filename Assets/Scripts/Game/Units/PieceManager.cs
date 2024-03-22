using Assets.Scripts.Extensions;
using Assets.Scripts.Game.Units.PreparedTypes;
using Assets.Scripts.GameLobby;
using Assets.Scripts.Structures;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class PieceManager : MonoNetworkSingleton<PieceManager>
    {
        public static ulong LastID => _lastID++;
        private static ulong _lastID = 0;
        private King _kingPrefab;
        private Queen _queenPrefab;
        private Rook _rookPrefab;
        private Bishop _bishopPrefab;
        private Knight _knightPrefab;
        private Pawn _pawnPrefab;

        [ServerRpc]
        public void AskForKingsSpawnServerRpc(Vector2Int position, Color color)
        {
            SpawnKingOnMapClientRpc(position, color, LastID);
            SpawnQueenOnMapClientRpc(position + Vector2Int.right, color, LastID);
            SpawnBishopOnMapClientRpc(position + Vector2Int.left + Vector2Int.up, color, LastID);
            SpawnBishopOnMapClientRpc(position + Vector2Int.left, color, LastID);
            SpawnKnightOnMapClientRpc(position + Vector2Int.down, color, LastID);
            SpawnKnightOnMapClientRpc(position + Vector2Int.right - Vector2Int.up, color, LastID);
            SpawnRookOnMapClientRpc(position + Vector2Int.one, color, LastID);
            SpawnRookOnMapClientRpc(position - Vector2Int.one, color, LastID);
            Vector2Int pawnSpawn = new Vector2Int(2, 1);
            for (int i = 0; i < 4; i++)
            {
                SpawnPawnOnMapClientRpc(pawnSpawn + position, color, LastID);
                pawnSpawn = pawnSpawn.Rotate90();
            }
            pawnSpawn = new Vector2Int(-2, 1);
            for (int i = 0; i < 4; i++)
            {
                SpawnPawnOnMapClientRpc(pawnSpawn + position, color, LastID);
                pawnSpawn = pawnSpawn.Rotate90();
            }
        }

        [ClientRpc]
        public void SpawnKingOnMapClientRpc(Vector2Int position, Color color, ulong pieceID)
        {
            if (_kingPrefab is null)
                _kingPrefab = (Resources.Load("Prefabs/King") as GameObject).GetComponent<King>();

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

        [ClientRpc]
        public void SpawnRookOnMapClientRpc(Vector2Int position, Color color, ulong pieceID)
        {
            if (_rookPrefab is null)
                _rookPrefab = (Resources.Load("Prefabs/Rook") as GameObject).GetComponent<Rook>();

            var instance = GameObject.Instantiate(_rookPrefab);
            instance.Init(position, color, pieceID);
            AddPieceToTeam(instance);
        }

        [ClientRpc]
        public void SpawnBishopOnMapClientRpc(Vector2Int position, Color color, ulong pieceID)
        {
            if (_bishopPrefab is null)
                _bishopPrefab = (Resources.Load("Prefabs/Bishop") as GameObject).GetComponent<Bishop>();

            var instance = GameObject.Instantiate(_bishopPrefab);
            instance.Init(position, color, pieceID);
            AddPieceToTeam(instance);
        }

        [ClientRpc]
        public void SpawnKnightOnMapClientRpc(Vector2Int position, Color color, ulong pieceID)
        {
            if (_knightPrefab is null)
                _knightPrefab = (Resources.Load("Prefabs/Knight") as GameObject).GetComponent<Knight>();

            var instance = GameObject.Instantiate(_knightPrefab);
            instance.Init(position, color, pieceID);
            AddPieceToTeam(instance);
        }

        [ClientRpc]
        public void SpawnQueenOnMapClientRpc(Vector2Int position, Color color, ulong pieceID)
        {
            if (_queenPrefab is null)
                _queenPrefab = (Resources.Load("Prefabs/Queen") as GameObject).GetComponent<Queen>();

            var instance = GameObject.Instantiate(_queenPrefab);
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
