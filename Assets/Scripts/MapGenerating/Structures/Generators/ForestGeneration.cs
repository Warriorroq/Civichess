using Assets.Scripts.Structures.MinMax;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.MapGenerating.Structures.Generators
{
    public class ForestGeneration : IStructureGenerator
    {
        public float offset;
        public float step;
        public float minNoise;
        public float highIndesityNoise;
        public IntMinMax mask;
        public ForestGeneration()
        {
            mask = new IntMinMax();
        }
        public ForestGeneration(float minNoise, float highIndesityNoise, float offset, float step, IntMinMax mask)
        {
            this.offset = offset;
            this.step = step;
            this.mask = mask;
            this.minNoise = minNoise;
            this.highIndesityNoise = highIndesityNoise;
        }

        public override void ApplyGenerationToCell(CellData cell)
        {
            if (mask is null)
                return;

            if (!mask.IsValueInRange(cell.height))
                return;

            float noise = Mathf.PerlinNoise(cell.positionOnMap.x * step + offset, cell.positionOnMap.y * step + offset);
            if (noise <= minNoise)
                return;

            Forest forest = new Forest();
            forest.forestDencity = noise > highIndesityNoise ? Forest.ForestDencity.high : Forest.ForestDencity.low;
            cell.structures.Add(forest);
        }

        public override void ApplyRandomization()
            => offset *= UnityEngine.Random.value;

        public override object Clone()
            => new ForestGeneration(minNoise, highIndesityNoise, offset, step, new IntMinMax(mask));

        public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
        {
            serializer.SerializeValue(ref offset);
            serializer.SerializeValue(ref step);
            serializer.SerializeValue(ref minNoise);
            serializer.SerializeValue(ref highIndesityNoise);
            serializer.SerializeValue(ref mask);
        }
    }
}
