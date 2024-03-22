using Unity.Netcode;

namespace Assets.Scripts.Structures.MinMax
{
    public class IntMinMax : NumericMinMax<int>, INetworkSerializable
    {
        public IntMinMax() 
        {
            max = int.MaxValue;
            min = int.MinValue;
        }

        public IntMinMax(IntMinMax minMax)
        {
            max = minMax.max;
            min = minMax.min;
        }

        public IntMinMax(int min, int max) : base(min, max){}
        public virtual void NetworkSerialize<T1>(BufferSerializer<T1> serializer) where T1 : IReaderWriter
        {
            serializer.SerializeValue(ref min);
            serializer.SerializeValue(ref max);
        }
    }
}
