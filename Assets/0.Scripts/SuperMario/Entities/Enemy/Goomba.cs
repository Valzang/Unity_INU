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

        private void Dead()
        {
            ResetEnemy();
            gameObject.SetActive(false);
        }
    }
}