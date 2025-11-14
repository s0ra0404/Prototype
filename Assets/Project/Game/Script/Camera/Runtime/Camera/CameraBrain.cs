using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Ver2;
using Random = UnityEngine.Random;

namespace Camera
{
    public enum Mode { Manual, Auto }

    [ExecuteAlways]
    public class CameraBrain : MonoBehaviour
    {
        [SerializeField] private Mode _selectMode = Mode.Auto;
        [SerializeField] private int _priorityId = 1;
        [SerializeField] private float _blendDuration = 1f;
        [SerializeField] private AnimationCurve _blendCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header("Shake Settings")]
        [SerializeField] private float _shakeDuration = 5f;
        [SerializeField] private float _shakeAmplitude = 0.3f;
        [SerializeField] private float _shakeFrequency = 20f;
        [SerializeField] private AnimationCurve _shakeFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

        private List<CameraBase> _virtualCameras = new List<CameraBase>();
        private UnityEngine.Camera _camera;

        private CameraBase _previousCamera;
        private CameraBase _activeCamera;
        private CameraBase _nextCamera;

        private float _blendElapsed;
        private Vector3 _blendStartPos;
        private Quaternion _blendStartRot;

        private float _shakeElapsed;
        private Vector3 _shakeOffsetPos;
        private Vector3 _shakeOffsetRot;
        private bool _isShaking = false;
        
        public void SetPriorityId(int id) => _priorityId = id;
        
        
        private void OnValidate()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _virtualCameras = FindObjectsByType<CameraBase>(FindObjectsSortMode.None).ToList();
            _activeCamera = _selectMode == Mode.Auto ? ChooseBestCamera() : GetPriorityCamera();

            // 初期はシェイクしないようにする
            _isShaking = false;
            _shakeElapsed = _shakeDuration; // 既に終わっている扱いにしておく（保険）
    
            if (_activeCamera != null)
            {
                _camera.transform.position = _activeCamera.transform.position;
                _camera.transform.rotation = _activeCamera.transform.rotation;
                _camera.fieldOfView = _activeCamera.FieldOfView;
            }
        }

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _virtualCameras = FindObjectsByType<CameraBase>(FindObjectsSortMode.None).ToList();
            _activeCamera = _selectMode == Mode.Auto ? ChooseBestCamera() : GetPriorityCamera();

            // 初期はシェイクしないようにする
            _isShaking = false;
            _shakeElapsed = _shakeDuration; // 既に終わっている扱いにしておく（保険）
    
            if (_activeCamera != null)
            {
                _camera.transform.position = _activeCamera.transform.position;
                _camera.transform.rotation = _activeCamera.transform.rotation;
                _camera.fieldOfView = _activeCamera.FieldOfView;
            }
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            _nextCamera = _selectMode == Mode.Auto ? ChooseBestCamera() : GetPriorityCamera();

            if (_nextCamera != null && _nextCamera != _activeCamera)
                StartBlend(_nextCamera);

            _previousCamera?.UpdateCamera(dt);
            _activeCamera?.UpdateCamera(dt);

            (Vector3 pos, Quaternion rot) = GetBlendedTransform(dt);

            _camera.transform.position = pos;
            _camera.transform.rotation = rot;
            _camera.fieldOfView = _activeCamera?.FieldOfView ?? _camera.fieldOfView;
        }

        private void StartBlend(CameraBase next)
        {
            _blendStartPos = _camera.transform.position;
            _blendStartRot = _camera.transform.rotation;

            _previousCamera = _activeCamera;
            _activeCamera = next;
            _blendElapsed = 0f;
        }

        private (Vector3 pos, Quaternion rot) GetBlendedTransform(float dt)
        {
            if (_previousCamera == null) return ApplyShake(_activeCamera.transform.position, _activeCamera.transform.rotation, dt);

            _blendElapsed += dt;
            float t = Mathf.Clamp01(_blendElapsed / _blendDuration);
            float blendT = _blendCurve.Evaluate(t);

            Vector3 pos = Vector3.Lerp(_blendStartPos, _activeCamera.transform.position, blendT);
            Quaternion rot = Quaternion.Slerp(_blendStartRot, _activeCamera.transform.rotation, blendT);

            if (_blendElapsed >= _blendDuration) _previousCamera = null;

            return ApplyShake(pos, rot, dt);
        }

        private CameraBase ChooseBestCamera()
        {
            return _virtualCameras
                .Where(c => c != null && c.IsActive)
                .OrderByDescending(c => c.Id)
                .FirstOrDefault();
        }

        private CameraBase GetPriorityCamera()
        {
            return _virtualCameras
                .FirstOrDefault(c => c != null && c.IsActive && c.Id == _priorityId);
        }


        [Button("Test Shake")]
        public void Test()
        {
            Shake();
        }
        
// シェイク開始（外部から呼ぶ）
        public void Shake(float? duration = null, float? amplitude = null)
        {
            if (duration.HasValue) _shakeDuration = duration.Value;
            if (amplitude.HasValue) _shakeAmplitude = amplitude.Value;

            _shakeElapsed = 0f;
            _isShaking = true;
        }

        // ApplyShake をフラグベースに書き換え、Perlin の入力を _shakeElapsed にする（初期ジャンプ対策）
        private (Vector3 pos, Quaternion rot) ApplyShake(Vector3 pos, Quaternion rot, float dt)
        {
            if (!_isShaking || _shakeDuration <= 0f) return (pos, rot);

            _shakeElapsed += dt;
            float t = Mathf.Clamp01(_shakeElapsed / _shakeDuration);
            float falloff = _shakeFalloff.Evaluate(t);

            // Time.time を使うと「開始時にランダムにジャンプ」することがあるので、
            // 経過時間を Perlin の入力に使うと開始が滑らかになります。
            float noiseBase = _shakeElapsed * _shakeFrequency;
            _shakeOffsetPos.x = (Mathf.PerlinNoise(noiseBase, 0f) - 0.5f) * 2f * _shakeAmplitude * falloff;
            _shakeOffsetPos.y = (Mathf.PerlinNoise(0f, noiseBase) - 0.5f) * 2f * _shakeAmplitude * falloff;
            _shakeOffsetPos.z = (Mathf.PerlinNoise(noiseBase, 1f) - 0.5f) * 2f * _shakeAmplitude * falloff;

            // 回転は簡易的にランダムでも良い（または Perlin に差し替え可能）
            _shakeOffsetRot.x = Random.Range(-_shakeAmplitude, _shakeAmplitude) * falloff;
            _shakeOffsetRot.y = Random.Range(-_shakeAmplitude, _shakeAmplitude) * falloff;
            _shakeOffsetRot.z = Random.Range(-_shakeAmplitude, _shakeAmplitude) * falloff;

            pos += _shakeOffsetPos;
            rot *= Quaternion.Euler(_shakeOffsetRot);

            // シェイク終了処理
            if (_shakeElapsed >= _shakeDuration)
            {
                _isShaking = false;
                _shakeOffsetPos = Vector3.zero;
                _shakeOffsetRot = Vector3.zero;
            }

            return (pos, rot);
        }
    }
}
