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
    
            //플레이어 방향으로 향하는 정규화된 벡터
            var targetGuidedVec = (Vector2)(curPlayerPos - curBulletPos).normalized;
            
            var deltaTime = Time.fixedDeltaTime;
            
            // x축 단위 벡터와 타겟 정규화 벡터 사이의 벡터로 선형 보간
            var nextDir = Vector2.Lerp(transform.right, targetGuidedVec, _rotationSpeed * deltaTime).normalized;
            
            // 해당 방향에 속도를 곱해서 다음 위치 정해주기
            Vector3 nextPos = transform.position += (Vector3)nextDir * (_speed * deltaTime);
            
            // 아크탄젠트로 각도 구하고 Radian => Degree 변경
            float angle = Mathf.Atan2(nextDir.y, nextDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // 화면 안에 있는지, 플레이어 위치와 동일해졌는지 체크
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