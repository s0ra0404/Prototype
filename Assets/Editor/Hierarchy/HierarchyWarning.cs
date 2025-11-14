using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorExpansion
{
    /// <summary>
    /// ヒエラルキーウィンドウで警告アイコンを表示するクラス
    /// </summary>
    public static class HierarchyWarning
    {
        private const int IconWidth = 28;

        [InitializeOnLoadMethod]
        private static void Initialize() =>
            EditorApplication.hierarchyWindowItemOnGUI += DrawIcon;

        /// <summary>
        /// 無効なコンポーネントがある場合に警告アイコンを表示します。
        /// </summary>
        private static void DrawIcon(int instanceID, Rect rect)
        {
            // 取得したオブジェクトがGameObject型ではない場合とすべてのコンポーネントが非nullの場合は処理を終了
            if (EditorUtility.InstanceIDToObject(instanceID) is not GameObject go 
                || go.GetComponents<MonoBehaviour>().All(c => c != null)) return;

            // ビルトインアイコンを取得
            var warningIcon = EditorGUIUtility.IconContent("d_console.warnicon").image;
        
            // 右端にアイコンを描画
            rect.x = rect.xMax - IconWidth;
            rect.width = IconWidth;
            GUI.Label(rect, warningIcon as Texture2D);
        }
    }
}