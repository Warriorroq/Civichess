using Unity.Netcode;

namespace Assets.Scripts.Structures.MinMax
{
    public class FloatMinMax : NumericMinMax<float>, INetworkSerializable
    {
        public virtual void NetworkSerialize<T1>(BufferSerializer<T1> serializer) where T1 : IReaderWriter {
            serializer.SerializeValue(ref min);
            serializer.SerializeValue(ref max);
        }
    }
}
