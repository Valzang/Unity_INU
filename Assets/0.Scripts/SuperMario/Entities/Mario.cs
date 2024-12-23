using _0.Scripts.Utility;
using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public class Mario : Entity
    {
        [Header("점프 파워")] [SerializeField] [Range(0f,100f)] private float _jumpPower = 10f;
        [Header("최대 이동속도")] [SerializeField] private float _maxSpeed = 4f;

        private Camera _mainCamera;
        private bool _isJumping = false;
        
        
        private int _obstacleLayer = -1;
        private int _enemyLayer    = -1;
        
        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main;

            if (_obstacleLayer == -1)
            {
                _obstacleLayer = LayerMask.NameToLayer(Obstacle);
            }

            if (_enemyLayer == -1)
            {
                _enemyLayer = LayerMask.NameToLayer(Enemy);
            }
        }

        private float _prevSpeedX = 0f;
        private readonly string AnimationMoveKey = "IsMove";
        private readonly string AnimationJumpKey = "IsJump";
        private readonly string AnimationDeadKey = "Dead";
        private void FixedUpdate()
        {
            if (!_isInteractable) return;
            var velocity = _rigidbody.velocity;
            var wasJumping = _isJumping;
            _isJumping = velocity.y is < -float.Epsilon or > float.Epsilon;

            // 점프해서 바닥에 닿는 순간 마찰력으로 인해 속도 감소되는 부분 수정
            if (wasJumping && !_isJumping)
            {
                velocity.x = _prevSpeedX;
                _rigidbody.velocity = velocity;
                _animator.SetBool(AnimationJumpKey, false);
            }
            _prevSpeedX = 0f;

            // 횡이동
            var isMove = HorizontalMove(velocity);
            _animator.SetBool(AnimationMoveKey, isMove && !_isJumping);
            
            // 점프
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
            CheckMoveInCamera();
        }

        private bool HorizontalMove(Vector2 velocity)
        {
            bool isMove = false;
            
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _rigidbody.AddRelativeForce(Vector2.left * _moveSpeed);
                _prevSpeedX = velocity.x = Mathf.Clamp(_rigidbody.velocity.x, -_maxSpeed, _maxSpeed);
                _rigidbody.velocity = velocity;
                if (!_spriteRenderer.flipX)
                {
                    _spriteRenderer.flipX = true;
                }

                isMove = true;
            }
            else if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                //_prevSpeedX = velocity.x = Mathf.Clamp(velocity.normalized.x, -_maxSpeed, _maxSpeed);
                _prevSpeedX = velocity.x = 0f;
                _rigidbody.velocity = velocity;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                _rigidbody.AddRelativeForce(Vector2.right * _moveSpeed);
                _prevSpeedX= velocity.x = Mathf.Clamp(_rigidbody.velocity.x, -_maxSpeed, _maxSpeed);
                _rigidbody.velocity = velocity;
                if (_spriteRenderer.flipX)
                {
                    _spriteRenderer.flipX = false;
                }
                isMove = true;
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                //_prevSpeedX = velocity.x = Mathf.Clamp(velocity.normalized.x, -_maxSpeed, _maxSpeed);
                _prevSpeedX = velocity.x = 0f;
                _rigidbody.velocity = velocity;
            }

            return isMove;
        }

        private void Jump()
        {
            if (_isJumping) return;
            _rigidbody.AddRelativeForce(new Vector2(0f, _jumpPower), ForceMode2D.Impulse);
            SoundManager.Instance.PlayEffect("SuperMario_Jump");
            _animator.SetBool(AnimationJumpKey, true);
            _animator.SetBool(AnimationMoveKey, false);

        }

        private bool _isInteractable = true;
        protected override void GetDamage()
        {
            SoundManager.Instance.PlayEffect("SuperMario_Dead");
            _isInteractable = false;
            _animator.SetBool(AnimationMoveKey, false);
            _animator.SetBool(AnimationJumpKey, false);
            
            _animator.Play("Dead",0,0f);
            
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.simulated = false;
            _collider.enabled = false;
        }

        /// <summary>
        /// 위치 세팅
        /// </summary>
        private void Respawn()
        {
            SuperMarioGameManager.Instance.RespawnMario();
        }

        /// <summary>
        /// 다시 움직일 수 있게끔 세팅
        /// </summary>
        private void SetPlayable()
        {
            _collider.enabled = true;
            _animator.SetBool(AnimationDeadKey, false);
            _isInteractable = true;
            _rigidbody.simulated = true;
        }

        /// <summary>
        /// 화면 밖으로 못나가게 설정
        /// </summary>
        /// <returns></returns>
        private bool CheckMoveInCamera()
        {
            var viewportPoint = _mainCamera.WorldToViewportPoint(_rigidbody.position);

            bool needToReset = false;
        
            if (viewportPoint.x < 0.01f)
            {
                viewportPoint.x = 0.015f;
                needToReset = true;
            }
            if (viewportPoint.x > 0.99f)
            {
                viewportPoint.x = 0.985f;
                needToReset = true;
            }

            if (!needToReset) return false;
            _rigidbody.position = _mainCamera.ViewportToWorldPoint(viewportPoint);
            var prevVelocity = _rigidbody.velocity;
            prevVelocity.x = 0f;
            _rigidbody.velocity = prevVelocity;
            return true;

        }


        protected void OnCollisionEnter2D(Collision2D other)
        {
            if (!_isInteractable) return;
            if(other.gameObject.layer == _obstacleLayer)
            {
                var contactPoint = other.GetContact(0).point;
                if (contactPoint.y < transform.position.y)
                {
                    _animator.SetBool(AnimationJumpKey, false);
                    var velocity = _rigidbody.velocity;

                    // 점프해서 바닥에 닿는 순간 마찰력으로 인해 속도 감소되는 부분 수정
                    velocity.x = _prevSpeedX;
                    _rigidbody.velocity = velocity;
                }

                return;
            }

            if(other.gameObject.layer == _enemyLayer)
            {
                var contactPoint = other.GetContact(0).point;
                // 적이 밟히는 상황이면
                if (_isJumping && contactPoint.y < transform.position.y)
                {
                    _rigidbody.AddRelativeForce(new Vector2(0f, _jumpPower*0.8f), ForceMode2D.Impulse);
                }
                else
                {
                    GetDamage();
                }
            }
        }
    }
}