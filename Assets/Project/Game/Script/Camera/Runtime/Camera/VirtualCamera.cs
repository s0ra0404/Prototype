using UnityEngine;

namespace Camera
{
    public class VirtualCamera : CameraBase
    {
        [SerializeField] protected Transform _targetTransform; // カメラが追従するターゲットのTransform
        [SerializeField] protected float _distance;            // ターゲットからの距離
        [SerializeField] protected Vector3 _offsetPosition;    // ターゲット位置からのオフセット
        [SerializeField] protected Vector3 _offsetAngles;      // ターゲット角度からのオフセット
        
        public override void UpdateCamera(float deltaTime)
        {
            Vector3 targetPos = _targetTransform ? _targetTransform.position : Vector3.zero;
            Quaternion rot = Quaternion.Euler(_offsetAngles);
            transform.position = targetPos + _offsetPosition + rot * new Vector3(0, 0, -_distance);
            transform.rotation = rot;
        }
    }
}