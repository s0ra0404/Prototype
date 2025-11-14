using UnityEngine;
using UnityEngine.InputSystem;

namespace Camera
{
    public class SurveillanceCamera : CameraBase
    {
        [SerializeField] private InputAction _lookInput;
        [SerializeField] private Vector2 _rotationSpeed = Vector2.one;
        [SerializeField] private float _maxUpAngle = 80f;    // 上方向（視線を上げる）
        [SerializeField] private float _maxDownAngle = 80f;  // 下方向（視線を下げる）
        [SerializeField] private float _maxLeftAngle = 180f; // 左方向（左に振れる最大）
        [SerializeField] private float _maxRightAngle = 180f;// 右方向（右に振れる最大）

        private Vector2 _look;
        private float _yaw;
        private float _pitch;

        private void OnEnable()
        {
            if (_lookInput != null) _lookInput.Enable();
        }

        private void OnDisable()
        {
            if (_lookInput != null) _lookInput.Disable();
        }

        private void Start()
        {
            // 現在角度をオイラー角として格納（warp防止のため一度だけ取得）
            Vector3 euler = transform.localEulerAngles;
            _yaw = NormalizeAngle(euler.y);
            _pitch = NormalizeAngle(euler.x);
        }

        private void Update()
        {
            if (_lookInput == null) return;
            _look = _lookInput.ReadValue<Vector2>();
        }

        public override void UpdateCamera(float deltaTime)
        {
            // 入力を回転速度に反映
            float mouseX = _look.x * _rotationSpeed.x * deltaTime;
            float mouseY = _look.y * _rotationSpeed.y * deltaTime;

            _yaw += mouseX;
            _pitch -= mouseY; // マウス上で上を見るため逆方向

            // 上下左右別々の制限を適用
            _pitch = Mathf.Clamp(_pitch, -_maxUpAngle, _maxDownAngle);
            _yaw = Mathf.Clamp(_yaw, -_maxLeftAngle, _maxRightAngle);

            // 回転適用
            transform.localRotation = Quaternion.Euler(_pitch, _yaw, 0);
        }

        /// <summary>
        /// UnityのEuler角(0〜360)を-180〜180に正規化
        /// </summary>
        private float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            return angle;
        }
    }
}
