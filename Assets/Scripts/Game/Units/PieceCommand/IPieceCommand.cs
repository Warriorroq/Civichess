using Unity.Netcode;

namespace Assets.Scripts.Game.Units.PieceCommand
{
    public interface IPieceCommand : INetworkSerializable
    {
        public bool IsApproved();
        public void Execute();
    }
}
