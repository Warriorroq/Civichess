using Assets.Scripts.GameLobby;
using Assets.Scripts.Structures;
using Unity.Netcode;
using UnityEngine;

public class RoundManager : MonoNetworkSingleton<RoundManager>
{
    public NetworkVariable<int> round = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<float> _currentTime = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public float _startRoundTimer;
    public float _additionalTimePerRound;

    private void Update()
    {
        if (!GameLobbyManager.Singleton.isHost)
            return;

        if (!GameLobbyManager.Singleton.inGame)
            return;

        if (_currentTime.Value < 0 || IsEveryOneIsReady())
            ChangeRound();

        _currentTime.Value -= Time.deltaTime;
    }
    private bool IsEveryOneIsReady()
    {
        Party party = GameLobbyManager.Singleton.party;
        foreach(var player in party.playersInfo.Values)
        {
            if (!player.ready)
                return false;
        }

        return true;
    }
    private void ChangeRound()
    {
        GameLobbyManager.Singleton.ResetReadyResultStateServerRpc();
        round.Value++;
        _currentTime.Value = round.Value * _additionalTimePerRound + _startRoundTimer;
    }
}