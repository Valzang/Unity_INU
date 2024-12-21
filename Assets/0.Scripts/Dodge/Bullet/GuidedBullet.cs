using System;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace _0.Scripts.Dodge
{
    public class GuidedBullet : Bullet
    {
        [Header("회전 속도")] 
        [SerializeField] [Range(0.01f, 10f)] private float _rotationSpeed = 1f;

        private static Camera _mainCamera = null;
        
        private void Awake()
        {
            _mainCamera ??= Camera.main;
        }
        
        public override void Init(Vector2 startPos, Vector2 endPos, float speed)
        {
            transform.position = startPos;
            _speed = speed;
            _endPosition = DodgePlayer.Instance.transform.position;
            isStart = false;
            gameObject.SetActive(true);
        }

        private bool isStart = false;

        protected override void FixedUpdate()
        {
            if (!isStart)
            {
                base.FixedUpdate();
                if (IsInCamera(transform.position))
                {
                    isStart = true;
                }
                return;
            }
            var curBulletPos = transform.position;
            var curPlayerPos = DodgePlayer.Instance.transform.position;
    
            var targetGuidedVec = (Vector2)(curPlayerPos - curBulletPos).normalized;
            
            var deltaTime = Time.fixedDeltaTime;
            var nextDir = Vector2.Lerp(transform.right, targetGuidedVec, _rotationSpeed * deltaTime).normalized;
            Vector3 nextPos = transform.position += (Vector3)nextDir * (_speed * deltaTime);
            float angle = Mathf.Atan2(nextDir.y, nextDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            if (IsInCamera(nextPos) && Vector2.SqrMagnitude(curPlayerPos - nextPos) > 0.01f)
            {
                return;
            }

            gameObject.SetActive(false);
            DodgeGameManager.Instance.Recycle(this);
        }

        private bool IsInCamera(Vector3 nextPos)
        {
            var viewportPoint = _mainCamera.WorldToViewportPoint(nextPos);
            return viewportPoint is { x: >= 0f, y: >= 0f } and { x: <= 1f, y: <= 1f };
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<DodgePlayer>(out var player)) return;
            if(player.GetDamage(1))
                GuidedBulletPool.Instance.ReleaseItem(this);
            else
            {
                DodgeGameManager.Instance.Recycle(this);
            }
        }
    }
}