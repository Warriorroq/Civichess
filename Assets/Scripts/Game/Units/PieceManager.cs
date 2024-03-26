using Assets.Scripts.Extensions;
using Assets.Scripts.GameLobby;
using Assets.Scripts.Structures;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Game.Units
{
    public class PieceManager : MonoNetworkSingleton<PieceManager>
    {
        public static ulong LastID => _lastID++;
        private static ulong _lastID = 0;
        private Dictionary<string, GameObject> _piecesPrefabs;

        protected override void Awake()
        {
           base.Awake();
            _piecesPrefabs = new Dictionary<string, GameObject>();
        }

        [ServerRpc]
        public void AskForKingsSpawnServerRpc(Vector2Int position, Color color)
        {
            SpawnPieceOnMapClientRpc("Prefabs/King", position, color, LastID);
            SpawnPieceOnMapClientRpc("Prefabs/Queen", position + Vector2Int.right, color, LastID);
            SpawnPieceOnMapClientRpc("Prefabs/Bishop", position + Vector2Int.left + Vector2Int.up, color, LastID);
            SpawnPieceOnMapClientRpc("Prefabs/Bishop", position + Vector2Int.left, color, LastID);
            SpawnPieceOnMapClientRpc("Prefabs/Knight", position + Vector2Int.down, color, LastID);
            SpawnPieceOnMapClientRpc("Prefabs/Knight", position + Vector2Int.right - Vector2Int.up, color, LastID);
            SpawnPieceOnMapClientRpc("Prefabs/Rook", position + Vector2Int.one, color, LastID);
            SpawnPieceOnMapClientRpc("Prefabs/Rook", position - Vector2Int.one, color, LastID);
            Vector2Int pawnSpawn = new Vector2Int(2, 1);
            for (int i = 0; i < 4; i++)
            {
                SpawnPieceOnMapClientRpc("Prefabs/Pawn", pawnSpawn + position, color, LastID);
                pawnSpawn = pawnSpawn.Rotate90();
            }
            pawnSpawn = new Vector2Int(-2, 1);
            for (int i = 0; i < 4; i++)
            {
                SpawnPieceOnMapClientRpc("Prefabs/Pawn", pawnSpawn + position, color, LastID);
                pawnSpawn = pawnSpawn.Rotate90();
            }
        }

        [ClientRpc]
        public void SpawnPieceOnMapClientRpc(string key, Vector2Int position, Color color, ulong pieceID)
        {
            SpawnPiece<Piece>(key, position, color, pieceID);
        }

        private void AddPieceToTeam(Piece piece)
        {
            Team team = GameManager.Singleton.party.teams[piece.teamColor];
            team.pieces.Add(piece.Id, piece);
        }

        private void CheckAvalability(string key)
        {
            if (_piecesPrefabs.ContainsKey(key))
                return;

            _piecesPrefabs.Add(key, Resources.Load(key) as GameObject);
        }

        private void SpawnPiece<T>(string key, Vector2Int position, Color color, ulong pieceID) where T : Piece
        {
            CheckAvalability(key);
            var instance = GameObject.Instantiate(_piecesPrefabs[key].GetComponent<T>());
            instance.Init(position, color, pieceID);
            AddPieceToTeam(instance);
        }
    }
}
