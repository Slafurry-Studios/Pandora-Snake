using UnityEngine;
using UnityEngine.Pool;

namespace Game.Core
{
    public abstract class GenericObjectPool<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private T prefab;
        [SerializeField] private int defaultCapacity = 20;
        [SerializeField] private int maxSize = 100;

        private ObjectPool<T> pool;

        public ObjectPool<T> Pool
        {
            get
            {
                if (pool == null)
                    InitializePool();
                return pool;
            }
        }

        private void Awake()
        {
            if (pool == null)
                InitializePool();
        }

        private void InitializePool()
        {
            pool = new ObjectPool<T>(
                CreatePooledItem,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                true,
                defaultCapacity,
                maxSize
            );
        }

        protected virtual T CreatePooledItem()
        {
            T instance = Instantiate(prefab, transform);
            return instance;
        }

        protected virtual void OnTakeFromPool(T instance)
        {
            instance.gameObject.SetActive(true);
        }

        protected virtual void OnReturnedToPool(T instance)
        {
            instance.gameObject.SetActive(false);
        }

        protected virtual void OnDestroyPoolObject(T instance)
        {
            Destroy(instance.gameObject);
        }
    }
}
