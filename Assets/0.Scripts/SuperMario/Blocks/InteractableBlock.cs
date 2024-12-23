using UnityEngine;

namespace _0.Scripts.SuperMario.Blocks
{
    [RequireComponent(typeof(Animator))]
    public class InteractableBlock : Block
    {
        protected Animator _animator;
        protected BoxCollider2D _collider;
        
        protected void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
        }
    }
}