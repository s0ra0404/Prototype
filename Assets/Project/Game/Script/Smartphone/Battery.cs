using UniRx;
using UnityEngine;
using Zenject;

namespace Smartphone
{
    /// <summary>
    /// スマートフォンの電源が入っているかを購読するためのインターフェース
    /// </summary>
    public interface IReadBatteryLevel
    {
        IReadOnlyReactiveProperty<float> BatteryLevel { get; }
    }
    
    /// <summary>
    /// 電池残量の計算を行い、更新するためのインターフェース
    /// </summary>
    public interface ILevelCalculation
    {
        void LevelCalculation(float maxActiveTime, float operatingHours);
    }
    
    /// <summary>
    /// スマートフォンの電池残量の計算を行い、更新するクラス
    /// </summary>
    public class Battery : IReadBatteryLevel, ILevelCalculation
    {
        // 電池残量
        private readonly ReactiveProperty<float> _batteryLevel = new ReactiveProperty<float>(100);
        // 最大電池残量の定数
        private const float MaxBattery = 100;
        
        // 外部からの読み取り専用プロパティ
        public IReadOnlyReactiveProperty<float> BatteryLevel => _batteryLevel;

        /// <summary>
        /// 最大稼働可能時間と稼働時間から電池残量を計算し更新します。
        /// </summary>
        /// <param name="maxActiveTime">最大稼働可能時間</param>
        /// <param name="operatingHours">稼働時間</param>
        public void LevelCalculation(float maxActiveTime, float operatingHours)
        {
            // 残量は100 / 稼働可能時間 = a a * 計測中の時間　= 残量
            var value = MaxBattery / maxActiveTime;
            _batteryLevel.Value = MaxBattery - value * operatingHours;
        }
    }
}

