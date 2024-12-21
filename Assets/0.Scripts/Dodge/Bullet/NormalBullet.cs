using UnityEngine;

namespace _0.Scripts.Dodge
{
    public class NormalBullet : Bullet
    {
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.TryGetComponent<DodgePlayer>(out var player)) return;
            if(player.GetDamage(1))
                NormalBulletPool.Instance.ReleaseItem(this);
            else
            {
                DodgeGameManager.Instance.Recycle(this);
            }
        }
    }
}