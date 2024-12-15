using System.Collections.Generic;
using UnityEngine;

namespace _0.Scripts.Utility
{
    public abstract class ObjectPooler<T> : Singleton<T> where T : MonoBehaviour
    {
        [Header("프리팹")] [SerializeField] private T _prefab;
        [Header("생성될 상위 부모")] [SerializeField] private Transform _parent;
        [Header("생성 한계수")] [SerializeField] [Range(2, 50000)] private uint _limitCount;

        protected Queue<T> _poolQueue;
        protected void Start()
        {
            CreatePools();
        }

        protected virtual void CreatePools()
        {
            _parent ??= transform;
            if (_prefab == null)
            {
                Debug.LogError($"Prefab이 비어있습니다 !");
                return;
            }

            _poolQueue ??= new((int)_limitCount);
            
            var currentChildCount = _parent.GetComponentsInChildren<T>().Length;
            if (currentChildCount >= _limitCount) return;
            
            var needCount = _limitCount - currentChildCount;
            for (int i = 0; i < needCount; ++i)
            {
                var obj = Instantiate(_prefab.gameObject, _parent);
                obj.SetActive(false);
                if (obj.TryGetComponent<T>(out var inst))
                {
                    _poolQueue.Enqueue(inst);                    
                }
                else
                {
                    Debug.LogError($"{_prefab.gameObject.name} 프리팹에 {typeof(T)}이 없었습니다");
                }
            }
        }

        /// <summary>
        /// 등록된 풀에서 아이템을 가져올 수 있는지 체크하고 가능하면 아이템을 가져옵니다.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual bool TryGetItem(out T item)
        {
            item = null;
            if (_poolQueue is { Count: > 0 })
            {
                item = _poolQueue.Dequeue();
            }
            return item != null;
        }

        /// <summary>
        /// 등록된 풀에 아이템을 반환합니다.
        /// </summary>
        /// <param name="item"></param>
        public virtual void ReleaseItem(T item)
        {
            if (item == null) return;
            _poolQueue.Enqueue(item);
        }
    }
}