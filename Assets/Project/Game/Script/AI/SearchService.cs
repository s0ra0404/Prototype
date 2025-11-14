using Player;
using RaycastHitExtensions;
using UnityEngine;
using Zenject;

namespace AI
{
    public interface ISearchService
    {
        bool SearchPlayerNoise(NoiseType noiseType);
        bool IsPlayerInRange(Vector3 origin, Vector3 playerPosition, float maxDistance);
        bool CanSeePlayer(Vector3 origin, Vector3 direction, float maxDistance, LayerMask playerLayer);
    }
    
    /// <summary>
    /// プレイヤーを捜索するためのサービスを提供します。
    /// </summary>
    public class SearchService : ISearchService
    {
        //　プレイヤーの足音(ノイズ)を読み取るためのインターフェース
        [Inject] private readonly IReadCurrentNoise _readPlayerNoise;
        
        /// <summary>
        /// プレイヤーの足音(ノイズ)が閾値を超えているか確認します。
        /// </summary>
        /// <param name="noiseType">ノイズのタイプ</param>
        public bool SearchPlayerNoise(NoiseType noiseType)
        {
            return _readPlayerNoise.CurrentNoise == noiseType;
        }
        
        /// <summary>
        /// プレイヤーが指定された範囲内にいるか確認します。
        /// </summary>
        /// <param name="origin">自身の位置</param>
        /// <param name="playerPosition">プレイヤーの位置</param>
        /// <param name="maxDistance">最大距離</param>
        /// <returns>範囲内ならtrue、そうでないならfalse</returns>
        public bool IsPlayerInRange(Vector3 origin, Vector3 playerPosition, float maxDistance)
        {
            var distance = Vector3.Distance(origin, playerPosition);
            return distance <= maxDistance;
        }

        /// <summary>
        /// プレイヤーとの視線が通っているか確認します。
        /// </summary>
        /// <param name="origin">自身の位置</param>
        /// <param name="direction">プレイヤーへの方向</param>
        /// <param name="maxDistance">レイキャストの最大距離</param>
        /// <param name="playerLayer">プレイヤーに設定されているレイヤーを設定したレイヤーマスク</param>
        /// <returns>視線が通るならtrue、そうでないならfalse</returns>
        public bool CanSeePlayer(Vector3 origin, Vector3 direction, float maxDistance, LayerMask playerLayer)
        {
            // レイキャストを実行して、プレイヤーにヒットするか
            Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance);
            Debug.DrawRay(origin, direction * maxDistance, Color.red);
            if (hit.collider == null) return false;
            
            // ヒットしたオブジェクトがプレイヤーのレイヤーに
            return hit.IsInLayerMask(playerLayer);
        }
    }
}