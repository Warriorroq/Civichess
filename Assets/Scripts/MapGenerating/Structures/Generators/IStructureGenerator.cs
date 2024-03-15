using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Assets.Scripts.MapGenerating.Structures.Generators
{
    public class IStructureGenerator : INetworkSerializable, ICloneable
    {
        public static Dictionary<Type, IStructureGenerator> dictionaryOfTemporaryGeneratorsAndTypes = new Dictionary<Type, IStructureGenerator>() {
            { Type.Forest, new ForestGeneration() },
        };

        public virtual void ApplyRandomization() { }
        public virtual void ApplyGenerationToCell(CellData cell) { }
        public virtual void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {}

        public virtual object Clone(){ return new IStructureGenerator(); }

        public enum Type : int
        {
            Forest = 0,
        }
    }
}
