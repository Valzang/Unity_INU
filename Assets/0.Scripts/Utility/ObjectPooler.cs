using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _0.Scripts.Utility
{
    public abstract class ObjectPooler<T> : Singleton<ObjectPooler<T>> where T : MonoBehaviour
    {
        [Header("프리팹")] [SerializeField] protected T _prefab;
        [Header("생성될 상위 부모")] [SerializeField] protected Transform _parent;
        [Header("생성 한계수")] [SerializeField] [Range(2, 50000)] protected uint _limitCount;

        protected Queue<T> _poolQueue;
        protected List<T> _activePools;
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
            _activePools ??= new((int)_limitCount);
            
            var currentChildCount = _parent.GetComponentsInChildren<T>().Length;
            if (currentChildCount >= _limitCount) return;
            
            var needCount = _limitCount - currentChildCount;
            for (int i = 0; i < needCount; ++i)
            {
                CreateItem();
            }
        }

        public virtual void DisposeAll()
        {
            if (_activePools is not { Count: > 0 }) return;
            for (int i = _activePools.Count - 1; i >= 0; --i)
            {
                var activeItem = _activePools[i];
                activeItem.gameObject.SetActive(false);
                _poolQueue.Enqueue(activeItem);
                _activePools.Remove(activeItem);
            }
        }

        protected virtual void CreateItem()
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

        /// <summary>
        /// 등록된 풀에서 아이템을 가져올 수 있는지 체크하고 가능하면 아이템을 가져옵니다.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="needExpand">부족 시 풀 확장할 지</param>
        /// <returns></returns>
        public virtual bool TryGetItem(out T item, bool needExpand = false)
        {
            item = null;
            if (_poolQueue is { Count: > 0 })
            {
                item = _poolQueue.Dequeue();
                _activePools.Add(item);
            }
            else
            {
                if (needExpand)
                {
                    CreateItem();
                    item = _poolQueue.Dequeue();
                    _activePools.Add(item);
                }
                else
                {
                    var activeItem = _activePools.FirstOrDefault();
                    _activePools.Remove(activeItem);
                    item = activeItem;
                }
            }
            return item != null;
        }

        /// <summary>
        /// 등록된 풀에서 아이템을 가져올 수 있는지 체크하고 가능하면 아이템을 가져옵니다.
        /// </summary>
        /// <param name="count">개수</param>
        /// <param name="items">아이템들</param>
        /// <param name="needExpand">확장 여부</param>
        /// <returns></returns>
        public virtual bool TryGetMultipleItem(int count, out List<T> items, bool needExpand = false)
        {
            items = null;
            if (_poolQueue != null && _poolQueue.Count >= count)
            {
                items = new(count);
                for (int i = 0; i < count; ++i)
                {
                    var item = _poolQueue.Dequeue();
                    items.Add(item);
                    _activePools.Add(item);
                }
            }
            else
            {
                var needCount = count - _poolQueue.Count;
                items = new(needCount);
                if (needExpand)
                {
                    for (int i = 0; i < count; ++i)
                    {
                        CreateItem();
                        var item = _poolQueue.Dequeue();
                        items.Add(item);
                        _activePools.Add(item);
                    }
                }
                else
                {
                    var maxCount = Math.Min(_activePools.Count, needCount);
                    for (int i = 0; i < maxCount; ++i)
                    {
                        var item = _activePools.FirstOrDefault();
                        _activePools.Remove(item);
                        items.Add(item);
                    }
                }
            }
            return items != null;
        }

        /// <summary>
        /// 등록된 풀에 아이템을 반환합니다.
        /// </summary>
        /// <param name="item"></param>
        public virtual void ReleaseItem(T item)
        {
            if (item == null) return;
            _activePools.Remove(item);
            item.gameObject.SetActive(false);
            _poolQueue.Enqueue(item);
        }
        
        /// <summary>
        /// 등록된 풀에 아이템을 반환합니다.
        /// </summary>
        /// <param name="items"></param>
        public virtual void ReleaseMultipleItem(List<T> items)
        {
            if (items == null) return;
            foreach (var item  in items)
            {
                item.gameObject.SetActive(false);
                _poolQueue.Enqueue(item);
                _activePools.Remove(item);
            }
        }
    }
}