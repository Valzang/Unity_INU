using System.Collections;
using _0.Scripts.SuperMario.Blocks;
using _0.Scripts.Utility;
using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public class Mario : Entity
    {
        [Header("점프 파워")] [SerializeField] [Range(0f,100f)] private float _jumpPower = 10f;
        [Header("최대 이동속도")] [SerializeField] private float _maxSpeed = 4f;
        [Header("무적 마리오")] [SerializeField] private SpriteRenderer _invincibleMario;

        private Camera _mainCamera;
        private bool _isJumping = false;
        private bool _isBig = false;
        
        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main;
            _invincibleMario.gameObject.SetActive(false);
            SoundManager.Instance.PlayBGM("SuperMario BGM", _prevBgmTiming);
        }

        private float _prevSpeedX = 0f;
        private readonly string AnimationMoveKey = "IsMove";
        private readonly string AnimationDeadKey = "Dead";

        private bool _wasJumping = false;
        private void FixedUpdate()
        {
            if (!_isInteractable) return;
            var velocity = _rigidbody.velocity;

            // 점프해서 바닥에 닿는 순간 마찰력으로 인해 속도 감소되는 부분 수정
            if (!_isJumping && _wasJumping && velocity.y is >= -float.Epsilon and <= float.Epsilon)
            {
                _wasJumping = false;
                velocity.x = _prevSpeedX;
                _rigidbody.velocity = velocity;
            }
            _prevSpeedX = 0f;

            // 횡이동
            var isMove = HorizontalMove(velocity);
            var newMoveValue = isMove && !_isJumping;
            if(_animator.GetBool(AnimationMoveKey) != newMoveValue)
                _animator.SetBool(AnimationMoveKey, newMoveValue);
            
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
                    _spriteRenderer.flipX = _invincibleMario.flipX = true;
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
                    _spriteRenderer.flipX = _invincibleMario.flipX = false;
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
            _isJumping = true;
            _wasJumping = true;
            _rigidbody.velocity = new(_rigidbody.velocity.x, 0f);
            _rigidbody.AddRelativeForce(new Vector2(0f, _jumpPower), ForceMode2D.Impulse);
            SoundManager.Instance.PlayEffect("SuperMario_Jump");
            _animator.Play("Jump",0,0f);
            
            if(_animator.GetBool(AnimationMoveKey))
                _animator.SetBool(AnimationMoveKey, false);

        }

        private bool _isInteractable = true;
        protected virtual void GetDamage()
        {
            if (_isInvincible) return;
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlayEffect("SuperMario_Dead");
            _isInteractable = false;
            _animator.SetBool(AnimationMoveKey, false);
            
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
            SoundManager.Instance.PlayBGM("SuperMario BGM", _prevBgmTiming);
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

        #region #충돌 처리 ===============================================

        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_isInteractable) return;
            var velocity = _rigidbody.velocity;
            if (other.gameObject.layer == _obstacleLayer)
            {
                // 점프해서 바닥에 닿는 순간 마찰력으로 인해 속도 감소되는 부분 수정
                velocity.x = _prevSpeedX;
                _rigidbody.velocity = velocity;
                _isJumping = false;
                _animator.Play("Idle",0,0f);
                return;
            }

            if (other.gameObject.layer != _enemyLayer) return;
            if (_isJumping)
            {
                velocity.y = 0f;
                _rigidbody.velocity = velocity;
                _rigidbody.AddRelativeForce(new Vector2(0f, _jumpPower*0.8f), ForceMode2D.Impulse);
            }
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            if (!_isInteractable) return;
            if(other.gameObject.layer == _obstacleLayer)
            {
                var contactPoint = other.GetContact(0).point;
                if (!(contactPoint.y >= transform.position.y)) return;
                
                if (other.transform.TryGetComponent<InteractableBlock>(out var itemBlock))
                {
                    itemBlock.React();
                    //TODO 바운스하고 전환 후 아이템 주기
                }
                else if (other.transform.TryGetComponent<ObstacleBlock>(out var block))
                {
                    //block.React();
                    if (_isBig)
                    {
                        //TODO 부수기
                    }
                    else
                    {
                        // 바운스만 시키기
                    }
                }

                return;
            }

            if(other.gameObject.layer == _enemyLayer)
            {
                var contactPoint = other.GetContact(0).point;
                if (other.transform.TryGetComponent<Enemy>(out var enemy)
                    && (_isInvincible || _isJumping && contactPoint.y < transform.position.y))
                {
                    enemy.GetDamage();
                }
                else
                {
                    GetDamage();
                }
                
            }
        }

        #endregion #충돌 처리 ===============================================


        private bool _isInvincible = false;
        private float _prevBgmTiming = 0f;

        /// <summary>
        /// 마리오 무적
        /// </summary>
        public void SetInvincible()
        {
            _isInvincible = true;
            _invincibleMario.gameObject.SetActive(true);
            StartCoroutine(InvincibleCoroutine());
            //TODO 배경음 변경
            _prevBgmTiming = SoundManager.Instance.GetCurrentBgmTime();
            Debug.Log($"중간 시간 : {_prevBgmTiming}");
            SoundManager.Instance.PlayBGM("SuperMario_Invincible");
        }

        IEnumerator InvincibleCoroutine()
        {
            yield return new WaitForSeconds(10f);
            DeactivateInvincible();
        }
        
        /// <summary>
        /// 마리오 무적해제
        /// </summary>
        public void DeactivateInvincible()
        {
            StopCoroutine(InvincibleCoroutine());
            _isInvincible = false;
            _invincibleMario.gameObject.SetActive(false);
            Debug.Log($"재개 시간 : {_prevBgmTiming}");
            SoundManager.Instance.PlayBGM("SuperMario BGM", _prevBgmTiming);
            _prevBgmTiming = 0f;
        }
    }
}