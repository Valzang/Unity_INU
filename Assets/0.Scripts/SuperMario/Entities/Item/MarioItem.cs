using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public class MarioItem : Entity
    {
        [Header("첫 이동은 왼쪽인지")] [SerializeField] protected bool _isLeftMove = true;
        
        protected Vector2 _moveVector = new(-1f, 0f);
        protected override void Awake()
        {
            base.Awake();
            _rigidbody.simulated = false;
        }

        public virtual void Show()
        {
            _animator.Play("Show", 0, 0f);
        }

        public virtual void ActivateRigidBody()
        {
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.simulated = true;
            
            _moveVector = new Vector2(_isLeftMove ? -_moveSpeed : _moveSpeed, 0f);
            _rigidbody.velocity = _moveVector;
            _spriteRenderer.flipX = _isLeftMove;
        }
        
        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != _obstacleLayer) return;
            var firstContact = other.GetContact(0);
            var currentObjectPointX = transform.position.x;

            if (_isLeftMove)
            {
                if (firstContact.point.x < currentObjectPointX)
                {
                    _isLeftMove = false;
                    _moveVector.x *= -1f;
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
                }
            }
            else if (firstContact.point.x > currentObjectPointX)
            {
                _isLeftMove = true;
                _moveVector.x *= -1f;
                _spriteRenderer.flipX = !_spriteRenderer.flipX;
            }

            _rigidbody.velocity = _moveVector;
        }
    }
}