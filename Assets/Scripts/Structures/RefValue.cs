namespace Assets.Scripts.Structures
{
    public class RefValue<T>
    {
        public T value;
        public RefValue(T value)
        {
            this.value = value;
        }
    }
}
