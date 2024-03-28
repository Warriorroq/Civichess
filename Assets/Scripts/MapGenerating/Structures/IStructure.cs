namespace Assets.Scripts.MapGenerating.Structures
{
    public interface IStructure
    {
        public abstract float GetVisibilityPenalty();
        public abstract int GetMovementPenalty();
        public abstract void CenerateStructureOnCell(Cell cell);
    }
}
