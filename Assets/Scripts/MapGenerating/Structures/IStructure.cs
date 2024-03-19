using Unity.Netcode;

namespace Assets.Scripts.MapGenerating.Structures
{
    public interface IStructure
    {
        public abstract int GetPenalty();
        public abstract void CenerateStructureOnCell(Cell cell);
    }
}
