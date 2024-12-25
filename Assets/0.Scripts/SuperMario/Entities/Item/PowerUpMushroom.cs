using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public class PowerUpMushroom : MarioItem
    {
        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.TryGetComponent<Mario>(out var mario))
            {
                mario.SetInvincible();
                Destroy(gameObject);
            }
            base.OnCollisionEnter2D(other);
        }
    }
}