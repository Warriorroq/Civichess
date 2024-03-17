﻿using Assets.Scripts.Structures;
using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Assets.Scripts.MapGenerating;
namespace Assets.Scripts.GameLobby
{
    [RequireComponent (typeof (FacepunchTransport))]
    public class GameNetworkManager : MonoSingleton<GameNetworkManager>
    {
        public Lobby? currentLobby;
        public ulong hostId;

        private FacepunchTransport _transport;

        private void Start()
        {
            _transport = GetComponent<FacepunchTransport>();

            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
            SteamFriends.OnGameLobbyJoinRequested += OnGameLobbyJoinRequested;
        }

        private void OnDestroy()
        {
            SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined -= OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite -= OnLobbyInvite;
            SteamMatchmaking.OnLobbyGameCreated -= OnLobbyGameCreated;
            SteamFriends.OnGameLobbyJoinRequested -= OnGameLobbyJoinRequested;

            if (NetworkManager.Singleton is null)
                return;
            RemoveCallbacksOnNetworkManager();
        }

        public async void StartHost(int maxMembers = 1)
        {
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.StartHost();
            GameLobbyManager.Singleton.myClientId = NetworkManager.Singleton.LocalClientId;
            currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);
        }

        public void StartClient(SteamId sId)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            _transport.targetSteamId = sId;
            GameLobbyManager.Singleton.myClientId = NetworkManager.Singleton.LocalClientId;
            if (NetworkManager.Singleton.StartClient())
                Debug.Log("Client started");
        }

        public void Disconnect()
        {
            currentLobby?.Leave();
            if (NetworkManager.Singleton is null)
                return;
            RemoveCallbacksOnNetworkManager();
            NetworkManager.Singleton.Shutdown(true);
            GameLobbyManager.Singleton.Disconnect();
        }

        public async void JoinLobbyWithID(TMP_InputField lobbyIDInputField)
        {
            if (!ulong.TryParse(lobbyIDInputField.text, out var ID))
                return;

            Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();
            foreach (Lobby lobby in lobbies)
            {
                if (lobby.Id == ID)
                {
                    await lobby.Join();
                    return;
                }
            }
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        private void RemoveCallbacksOnNetworkManager()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            }
            else
            {
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
                NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            }
        }

        private async void OnGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
        {
            RoomEnter joinedRoom = await lobby.Join();
            if (joinedRoom != RoomEnter.Success)
                return;
            currentLobby = lobby;
            GameLobbyManager.Singleton.ConnectedAsClient();
        }

        private void OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId steamId)
        {            
            currentLobby?.SendChatString($"[Server] Lobby was created id - {lobby.Id}");
            GameLobbyManager.Singleton.AddPlayerToDictionaryServerRPC(SteamClient.SteamId, SteamClient.Name, NetworkManager.Singleton.LocalClientId, UnityEngine.Color.black);
        }

        private void OnLobbyInvite(Friend friend, Lobby lobby)
        {
            
        }

        private void OnLobbyMemberLeave(Lobby lobby, Friend friend)
        {
            currentLobby?.SendChatString($"[Server] Member leaved {friend.Name}");
            GameLobbyManager.Singleton.RemovePlayerFromDictionaryServerRPC(friend.Id);
        }

        private void OnLobbyMemberJoined(Lobby lobby, Friend friend)
        {
            currentLobby?.SendChatString($"[Server] Member joined {friend.Name}");
        }

        private void OnLobbyEntered(Lobby lobby)
        {
            if (NetworkManager.Singleton.IsHost)
                return;
            StartClient(currentLobby.Value.Owner.Id);
        }

        private void OnLobbyCreated(Result result, Lobby lobby)
        {
            if (result != Result.OK)
                return;

            lobby.SetPublic();
            lobby.SetJoinable(true);
            lobby.SetGameServer(lobby.Owner.Id);
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            if(clientId == 0)
                Disconnect();
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            GameLobbyManager.Singleton.AddPlayerToDictionaryServerRPC(SteamClient.SteamId, SteamClient.Name, clientId, UnityEngine.Color.black);
            GameLobbyManager.Singleton.myClientId = clientId;
        }

        private void OnServerStarted()
        {
            GameLobbyManager.Singleton.HostCreated();
        }

    }
}
