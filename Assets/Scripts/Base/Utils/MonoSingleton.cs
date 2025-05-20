using UnityEngine;

namespace Base.Utils
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance => _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                Debug.LogError("Instance already exists. Destroying duplicate.");
                return;
            }

            _instance = (T)this;
            DontDestroyOnLoad(gameObject);
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }
    }
}