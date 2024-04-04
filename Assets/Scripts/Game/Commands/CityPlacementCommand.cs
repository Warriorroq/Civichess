using Assets.Scripts.GameLobby;
using Assets.Scripts.MapGenerating;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Game.Commands
{
    public struct CityPlacementCommand : ICommand
    {
        public Color teamColor;
        public Vector2Int position;
        public CityPlacementCommand(Color pieceTeam, Vector2Int position)
        {
            this.teamColor = pieceTeam;
            this.position = position;
        }

        public void Execute()
        {
            City.LoadPrefab();
            Cell cell = MapManager.Singleton.map[position];
            City city = GameObject.Instantiate(City.objPrefab);
            city.Init(teamColor, cell);
            cell.data.currentPiece.couldBeenUsed.Value = false;
        }

        public bool IsApproved()
            => City.IsPossibleToPlaceCityOnCell(MapManager.Singleton.map[position]) && GameManager.Singleton.party.teams[teamColor].king.couldBeenUsed.Value;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref teamColor);
            serializer.SerializeValue(ref position);
        }
    }
}
