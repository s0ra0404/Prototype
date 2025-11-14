using UnityEngine;

namespace Camera
{
    [ExecuteAlways]
    public abstract class CameraBase : MonoBehaviour
    {
        [SerializeField] private int _id = 0;                  // カメラの優先度
        [SerializeField] private bool _isActive = true;        // カメラがアクティブかどうか
        [SerializeField] protected float _fieldOfView = 60f;   // カメラの視野角

        
        // 外部からの読み取り専用プロパティ
        public int Id => _id;
        public bool IsActive => _isActive;
        public float FieldOfView => _fieldOfView;
        
        public abstract void UpdateCamera(float deltaTime);
    }
}