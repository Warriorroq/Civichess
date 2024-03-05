using Steamworks.Data;
using Steamworks;
using System;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using Assets.Scripts.Structures;
using System.Collections.Generic;
using Unity.Netcode;
using Assets.Scripts.GameLobby;

namespace Assets.Scripts.UI
{
    public class ChatManager : MonoSingleton<ChatManager>
    {
        [SerializeField] private TMP_InputField _messageInputField;
        [SerializeField] private TextMeshProUGUI _messageTemplate;
        [SerializeField] private GameObject _messageContainer;
        private LinkedList<string> _messageHistory;
        private void Start()
        {
            _messageHistory = new LinkedList<string>();
            _messageTemplate.text = String.Empty;
            SteamMatchmaking.OnChatMessage += ReceiveChatMessage;
        }

        public virtual void OnDestroy()
        {
            //base.OnDestroy();
            SteamMatchmaking.OnChatMessage -= ReceiveChatMessage;
        }

        private void ReceiveChatMessage(Lobby lobby, Friend friend, string msg)
        {
            _messageHistory.AddLast(msg);
            AddMessageToBox(msg);
        }

        public void AddMessageToBox(string msg)
        {
            TextMeshProUGUI message = Instantiate(_messageTemplate.gameObject, _messageContainer.transform).GetComponent<TextMeshProUGUI>();
            message.text = msg;
        }

        public void ToggleChatBox(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            if (!_messageInputField.gameObject.activeSelf)
            {
                _messageInputField.gameObject.SetActive(true);
                EventSystem.current.SetSelectedGameObject(_messageInputField.gameObject);
                return;
            }

            if (string.IsNullOrEmpty(_messageInputField.text))
            {
                _messageInputField.gameObject.SetActive(false);
                return;
            }

            GameNetworkManager.Singleton.currentLobby?.SendChatString($"[{SteamClient.Name}]{_messageInputField.text}");
            _messageInputField.text = string.Empty;
            _messageInputField.gameObject.SetActive(false);
        }

        public void ClearChatMessages()
        {
            foreach (Transform child in _messageContainer.transform)
                Destroy(child.gameObject);
            _messageHistory.Clear();
        }

        [ServerRpc(RequireOwnership = false)]
        public void AskForSyncChatMessagesServerRPC(ulong steamClientId)
        {
            ClientRpcSendParams sendParams = new ClientRpcSendParams();
            sendParams.TargetClientIds = new[] { GameManager.Singleton.party[steamClientId].localId };
            var rpcParams = new ClientRpcParams() { Send = sendParams };

            foreach (var msg in _messageHistory)
                SyncChatMessageClientRPC(steamClientId, msg, rpcParams);
        }

        [ClientRpc]
        public void SyncChatMessageClientRPC(ulong steamClientSyncId, string message, ClientRpcParams rpcParams = default)
        {
            if (SteamClient.SteamId != steamClientSyncId)
                return;

            _messageHistory.AddLast(message);
            AddMessageToBox(message);
            _messageHistory.Clear();
        }
    }
}
