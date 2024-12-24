using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public class InvincibleStar : MarioItem
    {
        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.layer == _obstacleLayer)
            {
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

                var velocity = _moveVector;
                // 점프해서 바닥에 닿는 순간 마찰력으로 인해 속도 감소되는 부분 수정
                velocity.y = 0f;
                _rigidbody.velocity = velocity;
                _rigidbody.AddRelativeForce(new Vector2(0f, 3.5f), ForceMode2D.Impulse);
                return;
            }
            
            if (other.transform.TryGetComponent<Mario>(out var mario))
            {
                mario.SetInvincible();
                Destroy(gameObject);
            }
        }
    }
}