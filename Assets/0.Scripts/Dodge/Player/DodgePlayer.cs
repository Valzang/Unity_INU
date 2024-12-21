using _0.Scripts.Dodge;
using _0.Scripts.Utility;
using UnityEngine;

public class DodgePlayer : Singleton<DodgePlayer>
{
    [Header("리지드바디")] [SerializeField] private Rigidbody2D _rigidbody;
    [Header("플레이어 체력")] [SerializeField] [Range(1, 100)] private int _startHp;
    [Header("플레이어 이동속도")] [SerializeField] [Range(1, 100)] private float _speed;

    private int _healthPoint = 1;
    private Camera _mainCamera = null;

    protected override void Awake()
    {
        base.Awake();
        _mainCamera ??= Camera.main;
        _healthPoint = _startHp;
    }

    public bool GetDamage(int damage)
    {
        _healthPoint -= damage;
        SoundManager.Instance.PlayEffect("Hit");
        if (_healthPoint <= 0)
        {
            DodgeGameManager.Instance.GameOver();
            return true;
        }

        return false;
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

        var viewportPoint = _mainCamera.WorldToViewportPoint(_rigidbody.position);

        bool needToReset = false;
        
        if (viewportPoint.x < 0.01f)
        {
            viewportPoint.x = 0.01f;
            needToReset = true;
        }
        if (viewportPoint.x > 0.99f)
        {
            viewportPoint.x = 0.99f;
            needToReset = true;
        }
        if (viewportPoint.y < 0.01f)
        {
            viewportPoint.y = 0.01f;
            needToReset = true;
        }
        if (viewportPoint.y > 0.99f)
        {
            viewportPoint.y = 0.99f;
            needToReset = true;
        }

        if (needToReset)
        {
            _rigidbody.position = _mainCamera.ViewportToWorldPoint(viewportPoint);
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