using System;
using UnityEngine.Events;
namespace Assets.Scripts.Structures
{
    [Serializable]
    public sealed class EventValue<T> 
    {
        public T Value
        {
            get
            {
                RefValue<T> refValue = new RefValue<T>(_value);
                onValueGet.Invoke(refValue);
                return refValue.value;
            }
            set
            {
                _value = value;
                onValueChanged.Invoke(_value);
            }
        }
        public UnityEvent<RefValue<T>> onValueGet;
        public UnityEvent<T> onValueChanged;
        private T _value;

        public EventValue(T value = default(T))
        {
            _value = value;
            onValueChanged = new UnityEvent<T>();
            onValueGet = new UnityEvent<RefValue<T>>();
        }

        public void RemoveAllListeners()
        {
            onValueChanged.RemoveAllListeners();
            onValueChanged.RemoveAllListeners();
        }
    }
}
