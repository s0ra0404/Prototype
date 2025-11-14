using UnityEngine;

namespace RaycastHitExtensions
{
    public static class RaycastHitExtensions
    {
        /// <summary>
        /// ヒットしたゲームオブジェクトのレイヤーが指定したレイヤーマスクに含まれているかどうかを判定します。
        /// </summary>
        /// <returns>含まれるなら : true / そうでないなら : false</returns>
        public static bool IsInLayerMask(this RaycastHit hit, LayerMask mask)
        {
            return (1 << hit.collider.gameObject.layer & mask) != 0;
        }
        
        public static bool IsInLayerMask(this Collider hit, LayerMask mask)
        {
            return (1 << hit.gameObject.layer & mask) != 0;
        }
    }
    

}