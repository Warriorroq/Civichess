using AYellowpaper.SerializedCollections;
using Steamworks;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.GameLobby
{
    [Serializable]
    public class Lobby
    {
        [SerializedDictionary("steam Id", "Player Data")]
        public SerializedDictionary<ulong, PlayerData> playersInfo;
        public PlayerData LocalPlayerData => this[SteamClient.SteamId.Value];
        public PlayerData this[ulong index]
        {
            get => playersInfo[index];
            set => playersInfo[index] = value;
        }

        public void Clear()
            => playersInfo.Clear();

        public void RemovePlayer(ulong steamId)
        {
            if (!playersInfo.ContainsKey(steamId))
                return;
            playersInfo.Remove(steamId);
        }

        public void AddPlayer(ulong steamId, string steamName, ulong clientId)
        {
            if (playersInfo.ContainsKey(steamId))
                return;
            playersInfo.Add(steamId, new PlayerData(clientId, steamName));
        }

        public void SetPlayerColor(ulong steamId, Color newColor)
        {
            if (!playersInfo.ContainsKey(steamId))
                return;
            playersInfo[steamId].playerColor = newColor;
        }

        public bool IsEveryoneIsReady()
        {
            foreach (var playerData in playersInfo.Values)
            {
                if (!playerData.ready)
                    return false;
            }

            return true;
        }

        public bool IsEachColorIsDifferent()
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

        [Serializable]
        public class PlayerData
        {
            public ulong localId;
            public string nickName;
            public bool ready;
            public Color playerColor;
            public PlayerData(ulong localId, string nickName)
            {
                this.localId = localId;
                this.nickName = nickName;
                ready = false;
                playerColor = Color.black;
            }
        }
    }
}
