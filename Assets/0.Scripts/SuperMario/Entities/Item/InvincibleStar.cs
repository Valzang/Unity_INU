using System;
using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public class InvincibleStar : MarioItem
    {
        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == _obstacleLayer)
            {
                var contact = other.GetContact(0);
                var contactSides = contact.normal;

                if (contactSides.y is > 0.1f or < -0.1f) return;
                if (_isLeftMove)
                {
                    _isLeftMove = false;
                    _moveVector.x *= -1f;
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
                }
                else
                {
                    _isLeftMove = true;
                    _moveVector.x *= -1f;
                    _spriteRenderer.flipX = !_spriteRenderer.flipX;
                }

                return;
            }
            
            if (other.transform.TryGetComponent<Mario>(out var mario))
            {
                mario.SetInvincible();
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var velocity = _moveVector;
            // 점프해서 바닥에 닿는 순간 마찰력으로 인해 속도 감소되는 부분 수정
            velocity.y = 0f;
            _rigidbody.velocity = velocity;
            _rigidbody.AddRelativeForce(new Vector2(0f, 3.5f), ForceMode2D.Impulse);
        }
    }
}