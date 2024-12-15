using _0.Scripts.Dodge;
using UnityEngine;

public class DodgePlayer : _0.Scripts.Utility.Singleton<DodgePlayer>
{
    [Header("리지드바디")] [SerializeField] private Rigidbody2D _rigidbody;
    [Header("플레이어 체력")] [SerializeField] [Range(1, 100)] private int _startHp;
    [Header("플레이어 이동속도")] [SerializeField] [Range(1, 100)] private float _speed;

    private int _healthPoint = 1;

    protected override void Awake()
    {
        base.Awake();
        _healthPoint = _startHp;
    }

    public void GetDamage(int damage)
    {
        _healthPoint -= damage;
        if (_healthPoint <= 0)
        {
            DodgeGameManager.Instance.GameOver();
        }
    }

    private float _horizontalValue;
    private float _verticalValue;
    
    private void FixedUpdate()
    {
        bool isMoveHorizontal = false;
        bool isMoveVertical = false;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _horizontalValue -= 1f;
            isMoveHorizontal = true;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _horizontalValue += 1f;
            isMoveHorizontal = true;
        }
        
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _verticalValue += 1f;
            isMoveVertical = true;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _verticalValue -= 1f;
            isMoveVertical = true;
        }

        if (!isMoveVertical && !isMoveHorizontal)
        {
            return;
        }

        var newPos = new Vector2(_horizontalValue, _verticalValue);

        if (newPos.magnitude > 1f)
        {
            newPos = newPos.normalized;
        }

        _rigidbody.MovePosition(_rigidbody.position + newPos * (_speed * Time.fixedDeltaTime));
        _horizontalValue = 0f;
        _verticalValue = 0f;
    }
}