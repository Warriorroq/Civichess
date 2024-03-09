using System;
namespace Assets.Scripts.Structures.MinMax
{
    public class NumericMinMax<T> where T : notnull, IComparable<T>
    {
        public T min;
        public T max;

        public NumericMinMax(T min = default(T), T max = default(T))
        {
            this.min = min;
            this.max = max;
        }

        public T Clamp(T value)
        {
            if (value.CompareTo(max) > 0)
                return max;

            if(value.CompareTo(min) < 0)
                return min;

            return value;
        }

        public bool IsValueInRange(T value)
            => value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0;
    }
}
