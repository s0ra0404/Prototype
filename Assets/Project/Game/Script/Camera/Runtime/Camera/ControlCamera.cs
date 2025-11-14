using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera
{
    public class ControlCamera : CameraBase
    {
        [SerializeField] protected Transform _targetTransform; // カメラが追従するターゲットのTransform
        [SerializeField] protected float _distance;            // ターゲットからの距離
        [SerializeField] protected Vector3 _offsetPosition;    // ターゲット位置からのオフセット
        [SerializeField] protected Vector3 _offsetAngles;      // ターゲット角度からのオフセット
        [SerializeField] private InputAction _lookInput;
        [SerializeField] private Vector2 _rotationSpeed = Vector2.one;
        [SerializeField, Range(-89f, 89f)] private float _minPitch = -30f;
        [SerializeField, Range(-89f, 89f)] private float _maxPitch = 60f;

        private Vector2 _look;
        private float _yaw, _pitch;

        private void OnEnable() => _lookInput.Enable();
        private void OnDisable() => _lookInput.Disable();

        private void Update()
        {
            if (_targetTransform == null) return;
            _look = _lookInput.ReadValue<Vector2>();
        }

        public override void UpdateCamera(float deltaTime)
        {
            if (_targetTransform == null) return;

            _yaw += _look.x * _rotationSpeed.x * deltaTime;
            _pitch = Mathf.Clamp(_pitch - _look.y * _rotationSpeed.y * deltaTime, _minPitch, _maxPitch);

            Quaternion targetRot = Quaternion.Euler(_pitch + _offsetAngles.x, _yaw + _offsetAngles.y, _offsetAngles.z);
            Vector3 targetPos = _targetTransform.position + _offsetPosition + targetRot * new Vector3(0, 0, -_distance);

            transform.rotation = targetRot;
            transform.position = targetPos;
        }
    }
}