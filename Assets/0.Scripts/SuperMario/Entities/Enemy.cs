using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public abstract class Enemy : Entity
    {
        [Header("첫 이동은 왼쪽인지")] [SerializeField] protected bool _isLeftMove = true;

        private Vector2 _moveVector = new(-1f, 0f);

        protected void Start()
        {
            _moveVector = new Vector2(_isLeftMove ? -_moveSpeed : _moveSpeed, 0f);
            _rigidbody.velocity = _moveVector;
            _spriteRenderer.flipX = _isLeftMove;
        }

        protected void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer(Obstacle)) return;
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
        }
    }
}