using Unity.Netcode;

namespace Assets.Scripts.Game.Commands
{
    public interface ICommand : INetworkSerializable
    {
        public bool IsApproved();
        public void Execute();
    }
}
