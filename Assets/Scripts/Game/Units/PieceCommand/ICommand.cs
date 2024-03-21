using Unity.Netcode;

namespace Assets.Scripts.Game.Units.PieceCommand
{
    public interface ICommand : INetworkSerializable
    {
        public bool IsApproved();
        public void Execute();
    }
}
