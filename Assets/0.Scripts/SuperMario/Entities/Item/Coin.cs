using _0.Scripts.Utility;
using DG.Tweening;
using UnityEngine;

namespace _0.Scripts.SuperMario
{
    public class Coin : MonoBehaviour
    {
        private Tween _bounceTween = null;
        [ContextMenu("바운스")]
        public void Bounce()
        {
            _bounceTween?.Kill();
            _bounceTween = null;
            SoundManager.Instance.PlayEffect("SuperMario_Coin");
            _bounceTween = transform
                            .DOPunchPosition(Vector3.up*0.3f, 0.4f, 1)
                            .OnComplete(()=>CoinPool.Instance.ReleaseItem(this))
                            .SetAutoKill(true);
        }
    }
}