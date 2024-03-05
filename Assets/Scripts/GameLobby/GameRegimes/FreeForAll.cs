using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.GameLobby.GameRegimes
{
    public class FreeForAll : IGameRegime
    {
        public virtual List<Team> CreateTeams(SerializedDictionary<ulong, Party.Player> playersInfo)
        {
            List<Team> teams = new List<Team>();
            foreach(var steamID in playersInfo.Keys)
                teams.Add(new Team(playersInfo[steamID].playerColor, $"{playersInfo[steamID].nickName}'s team", steamID));

            return teams;
        }

        public virtual bool IsEverythingOkayWithColors(SerializedDictionary<ulong, Party.Player> playersInfo)
        {
            var colors = playersInfo.Values.Select(x => x.playerColor).ToList();
            Hashtable table = new();
            foreach (var color in colors)
            {
                if (table.ContainsKey(color))
                    return false;

                table.Add(color, color);
            }
            return true;
        }
    }
}
