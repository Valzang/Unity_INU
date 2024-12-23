using UnityEngine;

namespace _0.Scripts.Utility
{
    [RequireComponent(typeof(Camera))]
    public class FollowingCamera : MonoBehaviour
    {
        [Header("따라다닐 대상")] [SerializeField] private Transform _targetTransform;

        private Camera _mainCamera;
        private Vector3 _middlePoint;
        private void Awake()
        {
            _mainCamera = GetComponent<Camera>();
            _middlePoint = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, -10f));
        }

        private void LateUpdate()
        {
            var targetCurrentPos = _targetTransform.position;
            var viewportPoint = _mainCamera.WorldToViewportPoint(targetCurrentPos);
            if (!(viewportPoint.x > 0.5f)) return;
            var curPos = transform.position;
            curPos.x += targetCurrentPos.x - _middlePoint.x;
            transform.position = curPos;
            _middlePoint = _mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0f, -10f));
        }
    }
}