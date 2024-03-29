using Assets.Scripts.Game.Units;

namespace Assets.Scripts.MapGenerating.Structures
{
    public interface IStructure
    {
        public abstract void OnOccupy(Piece piece);
        public abstract float GetVisibilityPenalty();
        public abstract int GetMovementPenalty();
        public abstract void CenerateStructureOnCell(Cell cell);
    }
}
