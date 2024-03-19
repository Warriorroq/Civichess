using Assets.Scripts.Game.Units;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLobby
{
    [Serializable]
    public class Team
    {
        public int score;
        public string name;
        public Color teamColor;
        public List<ulong> playersSteamIds;
        public SerializedDictionary<ulong, Piece> pieces;

        public Team(Color teamColor, string name, params ulong[] players)
        {
            score = 0;
            this.name = name;
            this.teamColor = teamColor;
            playersSteamIds = new List<ulong>(players);
        }

        public bool ContainsPlayer(ulong steamId)
        {
            foreach (ulong currentSteamId in playersSteamIds)
            {
                if (currentSteamId == steamId)
                    return true;
            }

            return false;
        }
    }
}
