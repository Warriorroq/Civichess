using Assets.Scripts.Structures;
using Assets.Scripts.UI;
using Steamworks;
using Steamworks.ServerList;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts
{
    public class GameManager : MonoNetworkSingleton<GameManager>
    {
        public bool connected;
        public bool inGame;
        public bool isHost;
        public ulong myClientId;
        public Dictionary<ulong, PlayerData> playersInfo;

        private void Start()
        {
            playersInfo = new Dictionary<ulong, PlayerData>();
        }

        public void HostCreated()
        {
            isHost = NetworkManager.Singleton.IsHost;
            connected = true;
        }

        public void ConnectedAsClient()
        {
            isHost = NetworkManager.Singleton.IsHost;
            connected = true;
        }

        public void Disconnect()
        {
            isHost = false;
            connected = false;
            myClientId = 0;
            playersInfo.Clear();
            ChatManager.Singleton.ClearChatMessages();
        }

        public class PlayerData
        {
            public ulong localId;
            public string nickName;
            public bool ready;
            public PlayerData(ulong localId, string nickName)
            {
                this.localId = localId;
                this.nickName = nickName;
                ready = false;
            }
        }

        public void Quit()
            =>Application.Quit();

        [ServerRpc(RequireOwnership = false)]
        public void AddMeToDictionaryServerRPC(ulong steamId, string steamName, ulong clientId)
        {
            UpdateClientsPlayerInfoClientRPC(steamId, steamName, clientId);
            foreach (var data in playersInfo.ToList())
                UpdateClientsPlayerInfoClientRPC(data.Key, data.Value.nickName, data.Value.localId);
            ChatManager.Singleton.AskForSyncChatMessagesServerRPC(steamId);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemoveMeFromDictionaryServerRPC(ulong steamId)
        {
            RemoveClientFromDictionaryClientRPC(steamId);
        }

        [ClientRpc]
        public void RemoveClientFromDictionaryClientRPC(ulong steamId)
        {
            if (!playersInfo.ContainsKey(steamId))
                return;
            playersInfo.Remove(steamId);
        }

        [ClientRpc]
        public void UpdateClientsPlayerInfoClientRPC(ulong steamId, string steamName, ulong clientId)
        {
            if (playersInfo.ContainsKey(steamId))
                return;
            playersInfo.Add(steamId, new PlayerData(clientId, steamName));
        }

        public void DebugLobby()
        {
            ChatManager.Singleton.AddMessageToBox("[Server] Current players:");
            foreach (var player in playersInfo.Values)
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
            playersInfo[steamClientSyncId].ready = readyState;
        }

    }
}
