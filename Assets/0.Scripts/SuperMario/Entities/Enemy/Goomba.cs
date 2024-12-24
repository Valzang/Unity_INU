using _0.Scripts.Utility;
using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public class Goomba : Enemy
    {
        public override void GetDamage()
        {
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.simulated = false;
            _collider.enabled = false;
            
            SoundManager.Instance.PlayEffect("SuperMario_GoombaStomp");
            _animator.Play("Dead", 0, 0f);
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

        private void Dead()
        {
            Destroy(gameObject);
        }
    }
}