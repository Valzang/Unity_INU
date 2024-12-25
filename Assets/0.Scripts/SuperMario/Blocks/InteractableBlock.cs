using System;
using DG.Tweening;
using UnityEngine;

namespace _0.Scripts.SuperMario.Blocks
{
    [RequireComponent(typeof(Animator))]
    public class InteractableBlock : Block
    {
        [Serializable]
        private enum BlockType
        {
            Coin = 0,
            PowerUp = 1,
            Invincible = 2,
            MultipleCoin = 3,
        }
        [Header("타입")] [SerializeField] private BlockType _blockType;
        [Header("일반 블럭 스프라이트")] [SerializeField] private Sprite _normalBlockSprite;
        [Header("스프라이트 렌더러")] [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("무적 별 프리팹")] [SerializeField] private InvincibleStar _starPrefab;
        [Header("버섯 프리팹")] [SerializeField] private PowerUpMushroom _mushroomPrefab;
        
        protected Animator _animator;
        protected BoxCollider2D _collider;
        
        protected override void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
            _isInteractable = true;
            _multipleCoinCount = 0;
            if (_blockType == BlockType.MultipleCoin)
            {
                _spriteRenderer.sprite = _normalBlockSprite;
            }
            else
            {
                _animator.Play("Item_Idle",0,0f);
            }
        }

        private int _multipleCoinCount = 0;
        private bool _isInteractable = true;
        /// <summary>
        /// 마리오와 부딪혔을 때 발생.
        /// </summary>
        public void React()
        {
            if (!_isInteractable) return;
            
            switch (_blockType)
            {
                case BlockType.Coin:
                {
                    //TODO 동전 이미지 소환
                    if (CoinPool.Instance.TryGetItem(out var coin))
                    {
                        var spawnPos = transform.position;
                        spawnPos.y += _collider.size.y * 1.5f;
                        coin.transform.position = spawnPos;
                        coin.gameObject.SetActive(true);
                        coin.Bounce();
                        Bounce();
                    }
                    SetNoneInteractable();
                    break;
                }
                case BlockType.PowerUp:
                {
                    var mushroomInstance = Instantiate(_mushroomPrefab.gameObject);
                    var spawnPos = transform.position;
                    spawnPos.y += _collider.size.y * 1.5f;
                    mushroomInstance.transform.position = spawnPos;
                    var mushroom = mushroomInstance.GetComponent<PowerUpMushroom>();
                    mushroom.Show();
                    Bounce();
                    SetNoneInteractable();
                    break;
                }
                case BlockType.Invincible:
                {
                    var starInstance = Instantiate(_starPrefab.gameObject);
                    var spawnPos = transform.position;
                    spawnPos.y += _collider.size.y * 1.5f;
                    starInstance.transform.position = spawnPos;
                    var star = starInstance.GetComponent<InvincibleStar>();
                    star.Show();
                    Bounce();
                    SetNoneInteractable();
                    break;
                }
                case BlockType.MultipleCoin:
                {
                    if (++_multipleCoinCount > 10)
                    {
                        SetNoneInteractable();
                    }
                    else
                    {
                        if (CoinPool.Instance.TryGetItem(out var coin))
                        {
                            var spawnPos = transform.position;
                            //spawnPos.y += _collider.size.y * 1.5f;
                            coin.transform.position = spawnPos;
                            coin.gameObject.SetActive(true);
                            coin.Bounce();
                        }

                        Bounce();
                    }
                    break;
                }
            }
        }

        private Tween _bounceTween = null;
        private void Bounce()
        {
            _bounceTween?.Kill(true);
            _bounceTween = null;
            _bounceTween = _spriteRenderer.transform
                                .DOPunchPosition(Vector3.up*0.1f, 0.4f, 1)
                                .OnComplete(()=>_spriteRenderer.transform.localPosition = Vector3.zero)
                                .SetAutoKill(true);
        }

        private void SetNoneInteractable()
        {
            _animator.Play("Idle",0,0f);
            _animator.Rebind();
            _isInteractable = false;
        }
    }
}