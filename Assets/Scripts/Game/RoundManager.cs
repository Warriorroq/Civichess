using Assets.Scripts.GameLobby;
using Assets.Scripts.Structures;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoNetworkSingleton<RoundManager>
{
    public NetworkVariable<int> round = new NetworkVariable<int>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<float> currentTime = new NetworkVariable<float>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public UnityEvent onRoundChange;
    public float startRoundTimer;
    public float additionalTimePerRound;

    private void Update()
    {
        if (!GameManager.Singleton.isHost)
            return;

        if (!GameManager.Singleton.inGame)
            return;

        if (currentTime.Value < 0 || IsEveryOneIsReady())
            ChangeRound();

        currentTime.Value -= Time.deltaTime;
    }
    private bool IsEveryOneIsReady()
    {
        Party party = GameManager.Singleton.party;
        foreach(var player in party.playersInfo.Values)
        {
            if (!player.ready)
                return false;
        }

        return true;
    }
    private void ChangeRound()
    {
        GameManager.Singleton.ResetReadyResultStateServerRpc();
        round.Value++;
        currentTime.Value = round.Value * additionalTimePerRound + startRoundTimer;
        onRoundChange.Invoke();
    }
}