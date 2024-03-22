using UnityEngine;

namespace Assets.Scripts.Structures
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public bool dontDestroyOnLoad;
        public static T Singleton
        {
            get
            {
                if (_instance is null)
                    SetSingleton();
                return _instance;
            }
        }

        private static T _instance;

        protected void CheckAndDestroyIfAdditionalInstanceExist()
        {
            if (Singleton == this)
                return;
            Destroy(this);
        }

        private void Awake()
        {
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(Singleton.gameObject);
            CheckAndDestroyIfAdditionalInstanceExist();
        }

        private static void SetSingleton()
        {
            FindAndSetSingleton();
            if (_instance is null)
                CreateAndSetSingleton();
        }

        private static void FindAndSetSingleton()
            =>_instance = FindObjectOfType<T>();

        private static void CreateAndSetSingleton()
            =>_instance = new GameObject($"MonoSingleton<{typeof(T).Name}>").AddComponent<T>();
    }
}
