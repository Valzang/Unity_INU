using System;
using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public abstract class Enemy : Entity
    {
        [Header("첫 이동은 왼쪽인지")] [SerializeField] protected bool _isLeftMove = true;

        protected Vector2 _moveVector = new(-1f, 0f);
        protected Camera _mainCamera;
        protected void Start()
        {
            _mainCamera = Camera.main;
            _moveVector = new Vector2(_isLeftMove ? -_moveSpeed : _moveSpeed, 0f);
            _rigidbody.velocity = _moveVector;
            _spriteRenderer.flipX = _isLeftMove;
        }

        protected void FixedUpdate()
        {
            _rigidbody.velocity = _moveVector;
            CheckMoveInCamera();
        }
        
        protected virtual bool CheckMoveInCamera()
        {
            var viewportPoint = _mainCamera.WorldToViewportPoint(_rigidbody.position);

            bool needToDestroy = false;
        
            if (viewportPoint.x < 0.01f)
            {
                viewportPoint.x = 0.015f;
                needToDestroy = true;
            }
            if (viewportPoint.x > 0.99f)
            {
                viewportPoint.x = 0.985f;
                needToDestroy = true;
            }

            if (!needToDestroy) return false;
            Destroy(gameObject);
            return true;

        }

        private bool _isFirst = true;
        protected void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer(Obstacle)) return;
            if (_isFirst)
            {
                _isFirst = false;
                return;
            }
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

        public abstract void GetDamage();
    }
}