using UnityEngine;

namespace _0.Scripts.SuperMario
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Entity : MonoBehaviour
    {
        [Header("스프라이트")][SerializeField] protected SpriteRenderer _spriteRenderer;
        [Header("이동 속도")] [SerializeField] [Range(0f,10f)] protected float _moveSpeed;
        
        protected Rigidbody2D _rigidbody;
        protected BoxCollider2D _collider;
        protected Animator _animator;
        
        protected const string Obstacle = "Obstacle";
        protected const string Enemy = "Enemy";
        
        protected int _obstacleLayer = -1;
        protected int _enemyLayer    = -1;
        
        
        protected virtual void Awake()
        {
            _rigidbody = GetComponent<Rigidbody2D>();
            _collider = GetComponent<BoxCollider2D>();
            _animator = GetComponent<Animator>();
            if (_obstacleLayer == -1)
            {
                _obstacleLayer = LayerMask.NameToLayer(Obstacle);
            }

            if (_enemyLayer == -1)
            {
                _enemyLayer = LayerMask.NameToLayer(Enemy);
            }
        }
    }
}