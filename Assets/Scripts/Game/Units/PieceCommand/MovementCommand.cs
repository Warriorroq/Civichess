using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Game.Units.PieceCommand
{
    public struct MovementCommand : IPieceCommand
    {
        public Vector2Int targetPosition;
        public Color pieceTeam;
        public ulong pieceID;

        public MovementCommand(Vector2Int targetPosition, Color pieceTeam, ulong pieceID)
        {
            this.targetPosition = targetPosition;
            this.pieceTeam = pieceTeam;
            this.pieceID = pieceID;
        }

        public void Execute()
        {
            Team team = GameManager.Singleton.party.teams[pieceTeam];
            Piece piece = team.pieces[pieceID];
            MapManager.Singleton.map[piece.currentPositionOnMap].DeOccupy();
            piece.currentPositionOnMap = targetPosition;
            MapManager.Singleton.map[targetPosition].Occupy(piece);
            piece.isAbleToMove = false;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref targetPosition);
            serializer.SerializeValue(ref pieceTeam);
            serializer.SerializeValue(ref pieceID);
        }

        public bool IsApproved()
        {
            Team team = GameManager.Singleton.party.teams[pieceTeam];
            Piece piece = team.pieces[pieceID];
            if (!piece.isAbleToMove)
                return false;

            return piece.movementMap.IsPossibleMoveToSquare(targetPosition);
        }
    }
}
