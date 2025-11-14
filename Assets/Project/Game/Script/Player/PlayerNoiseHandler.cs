using UnityEngine;

namespace Player
{
    public enum NoiseType
    {
        Silent,   // 静か（しゃがみ歩き・ゆっくり歩く）
        Normal,   // 普通（歩く）
        Loud      // 騒音（走る）
    }
    
    public interface IReadCurrentNoise
    {
        NoiseType CurrentNoise { get; }
    }

    public interface IWriteCurrentNoise
    {
        void SetNoiseType(NoiseType noiseType);
    }
    
    /// <summary>
    /// プレイヤーの足音(ノイズ)を通知するクラス
    /// </summary>
    public class PlayerNoiseHandler : MonoBehaviour, IReadCurrentNoise, IWriteCurrentNoise
    {
        private NoiseType _currentNoise;
        
        // プロパティ
        public NoiseType CurrentNoise => _currentNoise;

        /// <summary>
        /// プレイヤーの足音(ノイズ)を設定します。
        /// </summary>
        /// <param name="noiseType">ノイズタイプ</param>
        public void SetNoiseType(NoiseType noiseType)
        {
            _currentNoise = noiseType;
        }
    }
}