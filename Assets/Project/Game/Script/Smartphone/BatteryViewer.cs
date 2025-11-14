using System;
using System.Globalization;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Smartphone
{
    /// <summary>
    /// 電池残量をUIに表示するクラス
    /// </summary>
    public class BatteryViewer : MonoBehaviour
    {
        [SerializeField] private Image _batteryImage; // 電池残量を表示するイメージ
        [SerializeField] private TMP_Text _batteryText;   // 電池残量を表示するテキスト
        
        // 電池残量の閾値と閾値以下だった場合の設定する色を保持する構造体配列
        [SerializeField] BatteryStatus[] _batteryStatus = new BatteryStatus[3]
        {
            new BatteryStatus(100, Color.green),
            new BatteryStatus(30, Color.yellow),
            new BatteryStatus(20, Color.red)
        };
        
        // 電池残量の変更を購読するためのインターフェース
        [Inject] private readonly IReadBatteryLevel _readBatteryLevel;

        private void Awake()
        {
            UIUpdate();
        }

        /// <summary>
        /// 電池残量を購読し、UIを更新します。
        /// </summary>
        private void UIUpdate()
        {
            _readBatteryLevel.BatteryLevel
                .Subscribe(level =>
                {
                    UpdateColor(level);
                    UpdateText(level);
                })
                .AddTo(this);
        }
        
        /// <summary>
        /// 電池残量に応じてイメージの色を更新します。
        /// </summary>
        /// <param name="batteryLevel"></param>
        private void UpdateColor(float batteryLevel)
        {
            Color color = new Color(0, 0,0);
            
            
            foreach (var status in _batteryStatus)
            {
                // 現在の電池残量が閾値を超えていたら
                if (batteryLevel < status.BatteryThreshold)
                {
                    color = status.BatteryColor;
                }
            }
            // イメージの色を更新
            _batteryImage.color = color;
        }
        
        /// <summary>
        /// 電池残量の表示を更新します。
        /// </summary>
        /// <param name="batteryLevel"></param>
        private void UpdateText(float batteryLevel)
        {
            // 電池残量を切り上げ
            float ceilLevel = Mathf.Ceil(batteryLevel);
            
            // 文字列化しテキストを更新
            _batteryText.text = ceilLevel.ToString(CultureInfo.CurrentCulture);
        }
    }

    /// <summary>
    /// 電池の残量ごとのUIの設定を持つデータクラス
    /// </summary>
    [Serializable]
    public class BatteryStatus
    {
        [SerializeField] private float _batteryThreshold; // 電池残量の閾値
        [SerializeField] private Color _batteryColor;     // 閾値以下の場合に設定する色

        // 外部からの読み取り専用プロパティ
        public float BatteryThreshold => _batteryThreshold;
        public Color BatteryColor => _batteryColor;

        // コンストラクタ
        public BatteryStatus(float batteryThreshold, Color batteryColor)
        {
            _batteryThreshold = batteryThreshold;
            _batteryColor = batteryColor;
        }
    }
}