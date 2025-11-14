using Camera;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Smartphone
{
    /// <summary>
    /// スマートフォンの管理クラス
    /// 総起動時間を計算し、保持します。
    /// </summary>
    public class Smartphone : MonoBehaviour
    {
        [SerializeField] private InputAction _powerSupply;
        [SerializeField] private Button _playerBack;
        [SerializeField] private GameObject _mapUI;
        [SerializeField] private GameObject _cameraUI;
        [SerializeField] private float _activeTime; // 総起動可能時間
        
        private bool _isActive;        // 起動しているか
        private float _startTime;      // 起動時の時間
        private float _temporaryTime;  // 起動停止時に今までの計測時間を一時敵に保存する
        private float _operatingHours; // 起動時間
        private bool _isInit;          // 起動時の初期化がすでに行われたか
        
        // 電池残量の計算を行い、更新するためのインターフェース
        [Inject] private readonly ILevelCalculation _levelCalculation;
        [Inject] private readonly IReadBatteryLevel _readBatteryLevel;
        
        private void OnEnable() => _powerSupply.Enable();
        private void OnDisable() => _powerSupply.Disable();
        
        private void Start()
        {
            // マップ開閉入力を購読します。
            _powerSupply
                .PerformedAsObservable()
                .Subscribe(_ =>
                {
                    if(_cameraUI.activeInHierarchy) return;
                    if(_readBatteryLevel.BatteryLevel.Value <= 0f) return;
                    
                    _isActive = !_isActive;
                    
                    if (!_isActive)
                    {
                        _temporaryTime = _operatingHours;
                        _mapUI.gameObject.SetActive(false);
                        _isInit = false;
                    }
                    
                    if (_isActive)
                    {
                        _mapUI.gameObject.SetActive(true);
                    }
                })
                .AddTo(this);

            _readBatteryLevel.BatteryLevel
                .Where(level => level <= 0f)
                .Subscribe(_ =>
                {
                    if(!_isActive) return;
                    
                    _isInit = false;
                    _isActive = false;
                    _temporaryTime = _operatingHours;
                    _playerBack.onClick.Invoke();
                    _mapUI.SetActive(false);
                    _cameraUI.SetActive(false);
                    
                })
                .AddTo(this);
        }

        private void Update()
        {
            // 起動していなければ処理を中断
            if (!_isActive) return;

            // 総起動時間を計算し、更新
            TimeCalculation();
            
            // 電池残量を計算し、更新
            _levelCalculation.LevelCalculation(_activeTime, _operatingHours);
        }

        /// <summary>
        /// 総起動時間を計算し、更新します。
        /// </summary>
        private void TimeCalculation()
        {
            if (!_isInit)
            {
                _isInit = true;
                _startTime = Time.time;
            }

            // 使用時間の計算
            _operatingHours = Time.time - _startTime + _temporaryTime;
        }
    }
}