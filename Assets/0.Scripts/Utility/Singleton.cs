using UnityEngine;

namespace _0.Scripts.Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Header("씬 전환 시 파괴 여부")] [SerializeField] protected bool needToDestroy = true;
        public static T Instance { get; protected set; }

        /// <summary>
        /// 하나의 Instance만 들어갈 수 있도록 세팅
        /// </summary>
        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Destroy(this.gameObject);
                return;
            }

            Instance = this as T;
            if (!needToDestroy)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}