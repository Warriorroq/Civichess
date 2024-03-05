using Assets.Scripts.GameLobby;
using Assets.Scripts.Structures;
using Assets.Scripts.UI;
using Steamworks;
using System.Linq;
using Unity.Netcode;
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
        public void RemovePlayerFromDictionaryServerRPC(ulong steamId)
            =>RemoveClientFromDictionaryClientRPC(steamId);

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

        public void ChangeLocalPlayersColor(Color color)
            =>AskToChangeMyColorServerRpc(SteamClient.SteamId.Value, color);

        [ServerRpc(RequireOwnership = false)]
        public void AskToChangeMyColorServerRpc(ulong steamID, Color color)
            => SyncPlayersColorsClientRpc(steamID, color);

        [ClientRpc]
        public void SyncPlayersColorsClientRpc(ulong steamID, Color newColor)
            => lobby.SetPlayerColor(steamID, newColor);

        [ServerRpc(RequireOwnership = false)]
        public void AskToChangeMyReadyResultStateServerRpc(ulong steamID, bool readyState)
            =>SyncPlayersReadyStateClientRpc(steamID, readyState);

        [ClientRpc]
        public void SyncPlayersReadyStateClientRpc(ulong steamID, bool readyState)
            =>lobby[steamID].ready = readyState;
    }
}
