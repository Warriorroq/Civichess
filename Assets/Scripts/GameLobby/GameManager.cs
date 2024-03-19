using Assets.Scripts.MapGenerating;
using Assets.Scripts.Structures;
using Assets.Scripts.UI;
using Steamworks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.GameLobby
{
    public class GameManager : MonoNetworkSingleton<GameManager>
    {
        public bool connected;
        public bool inGame;
        public bool isHost;
        public ulong myClientId;

        public UnityEvent onConnectAsClient;
        public UnityEvent onDisconnect;

        public Party party;

        public static Party.Player CurrentPlayer => Singleton.party.LocalPlayerData;
        public static Team CurrentTeam => Singleton.party.GetTeamByPlayerId(SteamClient.SteamId);

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
            party.Clear();
            onDisconnect.Invoke();
        }

        public void Quit()
        {
            GameNetworkManager.Singleton.Disconnect();
            Application.Quit();
        }

        public void RunGame()
        {
            if (!isHost)
                return;

            if (!party.IsReadyForGame())
                return;

            party.PrepareTeams();
            GameNetworkManager.Singleton.currentLobby?.SetJoinable(false);
            GameNetworkManager.Singleton.currentLobby?.SendChatString($"[Server] Starting game...");
            MapManager.Singleton.SyncMapServerRpc();
            ResetReadyResultStateServerRpc();
            NetworkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            inGame = true;
        }        

        [ClientRpc]
        public void SyncTeamClientRpc(Color teamColor, string name, ulong[] players)
        {
            if (party.teams.ContainsKey(teamColor))
                return;
            
            party.teams.Add(teamColor, new Team(teamColor, name, players));
        }

        [ServerRpc(RequireOwnership = false)]
        public void AddPlayerToDictionaryServerRPC(ulong steamId, string steamName, ulong clientId, Color color)
        {            
            UpdateClientsPlayerInfoClientRPC(new Party.Player(clientId, steamId, steamName, color));
            foreach (var player in party.playersInfo.Values)
                UpdateClientsPlayerInfoClientRPC(player);
        }

        [ServerRpc(RequireOwnership = false)]
        public void RemovePlayerFromDictionaryServerRPC(ulong steamId)
        {
            RemoveClientFromDictionaryClientRPC(steamId);
        }            

        [ClientRpc]
        public void RemoveClientFromDictionaryClientRPC(ulong steamId)
            =>party.RemovePlayer(steamId);

        [ClientRpc]
        public void UpdateClientsPlayerInfoClientRPC(Party.Player player)
            => party.AddPlayer(player);

        public void DebugLobby()
        {
            ChatManager.Singleton.AddMessageToBox("[Server] Current players:");
            foreach (var player in party.playersInfo.Values)
            {
                ChatManager.Singleton.AddMessageToBox($"    {player.localId} {player.nickName}:");
                ChatManager.Singleton.AddMessageToBox($"        ready - {player.ready} ");
                ChatManager.Singleton.AddMessageToBox($"        color - {player.color} ");
            }
        }

        public void ChangeLocalPlayersColor(Color color)
            =>AskToChangeMyColorServerRpc(SteamClient.SteamId.Value, color);

        [ServerRpc(RequireOwnership = false)]
        public void AskToChangeMyColorServerRpc(ulong steamID, Color color)
            => SyncPlayersColorsClientRpc(steamID, color);

        [ClientRpc]
        public void SyncPlayersColorsClientRpc(ulong steamID, Color newColor)
            => party.SetPlayerColor(steamID, newColor);

        [ServerRpc(RequireOwnership = false)]
        public void AskToChangeMyReadyResultStateServerRpc(ulong steamID, bool readyState)
            =>SyncPlayersReadyStateClientRpc(steamID, readyState);

        [ClientRpc]
        public void SyncPlayersReadyStateClientRpc(ulong steamID, bool readyState)
            =>party[steamID].ready = readyState;

        [ServerRpc]
        public void ResetReadyResultStateServerRpc()
        {
            foreach(var key in party.playersInfo.Keys)
                SyncPlayersReadyStateClientRpc(key, false);
        }
    }
}
