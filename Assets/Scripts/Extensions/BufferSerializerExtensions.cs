using System.Collections.Generic;
using Unity.Netcode;

namespace Assets.Scripts.Extensions
{
    public static class BufferSerializerExtensions
    {
        public static List<V> SerializeList<T, V>(this BufferSerializer<T> serializer, List<V> list) 
            where T : IReaderWriter 
            where V : INetworkSerializable, new()
        {
            int length = 0;
            V[] readList = null;

            if (serializer.IsWriter)
            {
                length = list.Count;
                readList = list.ToArray();
            }

            serializer.SerializeValue(ref length);

            if (readList is null)
            {
                readList = new V[length];
                for (int i = 0; i < length; i++)
                    readList[i] = new V();
            }

            for (int n = 0; n < length; n++)
                serializer.SerializeValue(ref readList[n]);

            if (serializer.IsReader)
                return new List<V>(readList);
            return null;
        }

        public static List<int> SerializeList<T>(this BufferSerializer<T> serializer, List<int> list)
            where T : IReaderWriter
        {
            int length = 0;
            int[] readList = null;

            if (serializer.IsWriter)
            {
                length = list.Count;
                readList = list.ToArray();
            }

            serializer.SerializeValue(ref length);

            if (readList is null)
                readList = new int[length];

            for (int n = 0; n < length; n++)
                serializer.SerializeValue(ref readList[n]);

            if (serializer.IsReader)
                return new List<int>(readList);
            return null;
        }
    }
}
