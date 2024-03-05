using Assets.Scripts.Structures;
using Assets.Scripts.UI;
using AYellowpaper.SerializedCollections;
using Steamworks;
using System;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts
{
    public class GameManager : MonoNetworkSingleton<GameManager>
    {
        public bool connected;
        public bool inGame;
        public bool isHost;
        public ulong myClientId;

        public UnityEvent onConnectAsClient;
        public UnityEvent onDisconnect;

        public Lobby lobby;

        public void HostCreated()
        {
            isHost = NetworkManager.Singleton.IsHost;
            connected = true;
        }

        public void ConnectedAsClient()
        {
            isHost = NetworkManager.Singleton.IsHost;
            connected = true;
            onConnectAsClient.Invoke();
        }

        public void Disconnect()
        {
            isHost = false;
            connected = false;
            myClientId = 0;
            lobby.Clear();
            onDisconnect.Invoke();
        }

        public void Quit()
            =>Application.Quit();

        [ServerRpc(RequireOwnership = false)]
        public void AddMeToDictionaryServerRPC(ulong steamId, string steamName, ulong clientId)
        {
            UpdateClientsPlayerInfoClientRPC(steamId, steamName, clientId);
            foreach (var data in lobby.playersInfo.ToList())
                UpdateClientsPlayerInfoClientRPC(data.Key, data.Value.nickName, data.Value.localId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemoveMeFromDictionaryServerRPC(ulong steamId)
        {
            RemoveClientFromDictionaryClientRPC(steamId);
        }

        [ClientRpc]
        public void RemoveClientFromDictionaryClientRPC(ulong steamId)
            =>lobby.RemovePlayer(steamId);

        [ClientRpc]
        public void UpdateClientsPlayerInfoClientRPC(ulong steamId, string steamName, ulong clientId)
            => lobby.AddPlayer(steamId, steamName, clientId);

        public void DebugLobby()
        {
            ChatManager.Singleton.AddMessageToBox("[Server] Current players:");
            foreach (var player in lobby.playersInfo.Values)
                ChatManager.Singleton.AddMessageToBox($"    {player.localId} {player.nickName}  ready - {player.ready}");
        }

        [ServerRpc(RequireOwnership = false)]
        public void AskToChangeMyReadyResultStateServerRpc(ulong steamClientSyncId, bool readyState)
        {
            SyncPlayersReadyStateClientRpc(steamClientSyncId, readyState);
        }

        [ClientRpc]
        public void SyncPlayersReadyStateClientRpc(ulong steamClientSyncId, bool readyState)
        {
            lobby[steamClientSyncId].ready = readyState;
        }

        [Serializable]
        public class Lobby
        {
            [SerializedDictionary("steam Id", "Player Data")]
            public SerializedDictionary<ulong, PlayerData> playersInfo;

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

            [Serializable]
            public class PlayerData
            {
                public ulong localId;
                public string nickName;
                public bool ready;
                public Color playersColor;
                public PlayerData(ulong localId, string nickName)
                {
                    this.localId = localId;
                    this.nickName = nickName;
                    ready = false;
                    playersColor = Color.black;
                }
            }
        }
    }
}
