using Assets.Scripts.Game.Units;
using Assets.Scripts.Structures.MinMax;
using UnityEngine;

namespace Assets.Scripts.MapGenerating.Structures
{
    public class Forest : IStructure
    {
        public static FloatMinMax visibilityPenalty = new FloatMinMax(.5f, 7f);
        public static IntMinMax movementPenalty = new IntMinMax(1, 7);

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

        public override string ToString()
            => $"Forest: {forestDencity}";

        public virtual int GetMovementPenalty()
            => forestDencity == ForestDencity.low ? movementPenalty.min : movementPenalty.max;

        public float GetVisibilityPenalty()
            => forestDencity == ForestDencity.low ? visibilityPenalty.min : visibilityPenalty.max;

        public void OnOccupy(Piece piece){}

        public enum ForestDencity
        {
            low = 0,
            high = 2,
        }
    }
}
