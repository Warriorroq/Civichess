using Assets.Scripts.GameLobby;
using Unity.Netcode;

namespace Assets.Scripts.Game.Commands
{
    public struct RoundChangeCommand : ICommand
    {
        public int round;
        private float currentTime;

        public RoundChangeCommand(int round, float currentTime)
        {
            this.round = round;
            this.currentTime = currentTime;
        }

        public void Execute()
        {
            if(GameManager.Singleton.isHost)
                GameManager.Singleton.ResetReadyResultStateServerRpc();
            RoundManager.Singleton.ChangeRound(round, currentTime);
        }

        public bool IsApproved()
            => RoundManager.Singleton.currentTime < 0 || RoundManager.Singleton.IsEveryOneIsReady();

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref round);
            serializer.SerializeValue(ref currentTime);

        }
    }
}
