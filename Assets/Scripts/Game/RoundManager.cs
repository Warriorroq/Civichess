using Assets.Scripts.Game.Units;
using Assets.Scripts.Game.Units.PieceCommand;
using Assets.Scripts.GameLobby;
using Assets.Scripts.Structures;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoSingleton<RoundManager>
{
    public int round;
    public float currentTime;

    public UnityEvent onRoundChange;
    public float startRoundTimer;
    public float additionalTimePerRound;

    private void Update()
    {
        if (!GameManager.Singleton.inGame)
            return;

        currentTime -= Time.deltaTime;
        if (!GameManager.Singleton.isHost)
            return;
        
        if (currentTime < 0 || IsEveryOneIsReady())
            CommandManager.Singleton.AskForApprovingCommandServerRpc(new RoundChangeCommand(round+1, round * additionalTimePerRound + startRoundTimer));
    }

    public bool IsEveryOneIsReady()
    {
        Party party = GameManager.Singleton.party;
        foreach(var player in party.playersInfo.Values)
        {
            if (party.GetTeamByPlayerId(player.steamId).king is null)
                continue;

            if (!player.ready)
                return false;
        }

        return true;
    }

    public void ChangeRound(int round, float time)
    {
        this.round = round;
        currentTime = time;
        onRoundChange.Invoke();
    }
}