using UnityEngine;

namespace Assets.Scripts.MapGenerating.Structures
{
    public class Forest : IStructure
    {
        public static GameObject objPrefab = Resources.Load("Prefabs/TreePrefab") as GameObject;
        public ForestDencity forestDencity;
        public void CenerateStructureOnCell(Cell cell)
        {
            if (forestDencity == ForestDencity.low)
                InstantiateTrees(cell, 2);
            else
                InstantiateTrees(cell, 6);
        }

        private void InstantiateTrees(Cell cell, int amount)
        {
            for(int i = 0; i < amount; i++) {
                GameObject tree = GameObject.Instantiate(objPrefab) as GameObject;
                tree.transform.parent = cell.topTransform;
                tree.transform.localPosition = new Vector3(-0.46f + Random.value * .92f, 0, -0.46f + Random.value * .92f);
            }
        }
        public enum ForestDencity
        {
            low = 0,
            high = 2,
        }

        public override string ToString()
            => $"Forest: {forestDencity}";

        public virtual int GetPenalty()
            => forestDencity == ForestDencity.low ? 1 : 7;
    }
}
