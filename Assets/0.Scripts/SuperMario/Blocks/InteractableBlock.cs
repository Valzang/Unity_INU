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
        [Header("이미 작동한 블럭 스프라이트")] [SerializeField] private Sprite _noneBlockSprite;
        [Header("스프라이트 렌더러")] [SerializeField] private SpriteRenderer _spriteRenderer;
        
        protected Animator _animator;
        protected BoxCollider2D _collider;
        
        protected void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
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
            
            _animator.Play("Bounce", 0, 0f);
            switch (_blockType)
            {
                case BlockType.Coin:
                {
                    SetNoneInteractable();
                    break;
                }
                case BlockType.PowerUp:
                {
                    SetNoneInteractable();
                    break;
                }
                case BlockType.Invincible:
                {
                    SetNoneInteractable();
                    break;
                }
                case BlockType.MultipleCoin:
                {
                    if (++_multipleCoinCount > 10)
                    {
                        SetNoneInteractable();
                    }
                    break;
                }
            }
        }

        private void SetNoneInteractable()
        {
            _spriteRenderer.transform.DOPunchPosition(Vector3.up*0.1f, 0.4f, 1);
            _animator.Play("Idle",0,0f);
            _animator.Rebind();
            _isInteractable = false;
        }
    }
}