using System;
using UnityEngine.Events;
namespace Assets.Scripts.Structures
{
    [Serializable]
    public sealed class EventValue<T> 
    {
        public T Value
        {
            get => _value;
            set
            {
                beforeValueChanged.Invoke(value);
                _value = value;
                onValueChanged.Invoke(_value);
            }
        }

        public UnityEvent<T> beforeValueChanged;
        public UnityEvent<T> onValueChanged;
        private T _value;

        public EventValue(T value = default(T))
            =>_value = value;

        public void RemoveAllListeners()
        {
            beforeValueChanged.RemoveAllListeners();
            onValueChanged.RemoveAllListeners();
        }
    }
}
