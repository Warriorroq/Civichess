﻿using Assets.Scripts.GameLobby.GameRegimes;
using AYellowpaper.SerializedCollections;
using Steamworks;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.GameLobby
{
    [Serializable]
    public class Party
    {
        [SerializedDictionary("steam Id", "Player Data")]
        public SerializedDictionary<ulong, Player> playersInfo;
        [SerializedDictionary("Team Color", "Team Data")]
        public SerializedDictionary<Color, Team> teams;

        public Player LocalPlayerData => this[SteamClient.SteamId.Value];
        private IGameRegime _gameRegime = new FreeForAll();
        public Player this[ulong steamId]
        {
            get => playersInfo[steamId];
            set => playersInfo[steamId] = value;
        }

        public Team GetTeamByPlayerId(ulong id)
        { 
            foreach(var team in teams.Values)
            {
                if(team.ContainsPlayer(id))
                    return team;
            }    

            return null;
        }

        public void ApplyRegime()
        {
            if(_gameRegime is null)
                _gameRegime = new FreeForAll();

            foreach(var team in _gameRegime.CreateTeams(playersInfo))
                teams.Add(team.teamColor, team);
        }

        public void Clear()
            => playersInfo.Clear();

        public void RemovePlayer(ulong steamId)
        {
            if (!playersInfo.ContainsKey(steamId))
                return;

            if(!GameManager.Singleton.inGame)
                TryToRemoveTeam(steamId);

            playersInfo.Remove(steamId);
            Team team = GetTeamByPlayerId(steamId);
            if (team is not null)
                team.king = null;
        }

        private void TryToRemoveTeam(ulong steamId)
        {
            Team team = GetTeamByPlayerId(steamId);
            team?.playersSteamIds.Remove(steamId);
            if (team.playersSteamIds.Count != 0)
                teams.Remove(team.teamColor);
        }

        public void AddPlayer(ulong steamId, string steamName, ulong clientId, Color color)
        {
            if (playersInfo.ContainsKey(steamId))
                return;
            playersInfo.Add(steamId, new Player(clientId, steamId, steamName, color));
        }

        public void AddPlayer(Player player)
        {
            if (playersInfo.ContainsKey(player.steamId))
                return;
            playersInfo.Add(player.steamId, player);
        }

        public void SetPlayerColor(ulong steamId, Color newColor)
        {
            if (!playersInfo.ContainsKey(steamId))
                return;
            playersInfo[steamId].color = newColor;
        }

        public bool IsReadyForGame()
        {
            if (!IsEveryoneIsReady())
            {
                GameNetworkManager.Singleton.currentLobby?.SendChatString($"[Server] Start is not possible, not everyone is ready");
                return false;
            }

            if (!_gameRegime.IsEverythingOkayWithColors(playersInfo))
            {
                GameNetworkManager.Singleton.currentLobby?.SendChatString($"[Server] Start is not possible, player's colors are matching");
                return false;
            }

            return true;
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

        public void SyncTeams()
        {
            foreach (var teamData in teams.Values)
                GameManager.Singleton.SyncTeamClientRpc(teamData.teamColor, teamData.name, teamData.playersSteamIds.ToArray());
        }

        public void PrepareTeams()
        {
            ApplyRegime();
            SyncTeams();
        }

        [Serializable]
        public class Player : INetworkSerializable
        {
            public ulong steamId;
            public ulong localId;
            public string nickName;
            public bool ready;
            public Color color;

            public Player()
            {
                localId = 0;
                steamId = 0;
                ready = false;
                nickName = string.Empty;
                color = Color.black;
            }

            public Player(ulong localId, ulong steamId, string nickName, Color color)
            {
                this.localId = localId;
                this.steamId = steamId;
                ready = false;
                this.nickName = nickName;
                this.color = color;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref steamId);
                serializer.SerializeValue(ref localId);
                serializer.SerializeValue(ref nickName);
                serializer.SerializeValue(ref ready);
                serializer.SerializeValue(ref color);
            }
        }
    }
}
