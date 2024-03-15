namespace Assets.Scripts.MapGenerating.Structures
{
    public class Forest : IStructure
    {
        public ForestDencity forestDencity;
        public void CenerateStructureOnCell(Cell cell)
        {
            
        }

        public enum ForestDencity
        {
            low = 0,
            high = 2,
        }

        public override string ToString()
            => $"Forest: {forestDencity}";
    }
}
