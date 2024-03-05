using AYellowpaper.SerializedCollections;
using System.Collections.Generic;

namespace Assets.Scripts.GameLobby.GameRegimes
{
    public interface IGameRegime
    {
        public abstract List<Team> CreateTeams(SerializedDictionary<ulong, Party.Player> playersInfo);
        public abstract bool IsEverythingOkayWithColors(SerializedDictionary<ulong, Party.Player> playersInfo);
    }
}
