using Unity.Netcode;

namespace Assets.Scripts.MapGenerating.Structures
{
    public interface IStructure : INetworkSerializable
    {
        public abstract void CenerateStructureOnCell(Cell cell);
        public enum Type : int{}
    }
}
