using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorExpansion
{
    [InitializeOnLoad]
    public static class HierarchyIconRenderer
    {
        private const int IconWidth = 16; // アイコンの幅
        private const int IconHeight = 16; // アイコンの高さ

        static HierarchyIconRenderer()
        {
            // Hierarchyウィンドウで項目描画時に呼ばれるメソッドを登録
            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
        }

        private static void OnGUI(int instanceID, Rect selectionRect)
        {
            // インスタンスIDからGameObjectを取得
            var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            // GameObjectがnullの場合は処理を終了
            if (gameObject == null)
            {
                return;
            }

            // 描画位置の設定
            var iconRect = selectionRect;
            iconRect.x = iconRect.xMax - 45; 
            iconRect.width = IconWidth;  // アイコンの幅を設定
            iconRect.height = IconHeight; // アイコンの高さを設定

            // GameObjectの全てのコンポーネントを取得し、Transformを除外
            var components = gameObject
                .GetComponents<Component>()
                .Where(component => component != null)      // nullでないコンポーネントのみ
                .Where(component => !(component is Transform)) // Transformを除外
                .Reverse(); // 逆順に処理

            // コンポーネントごとにアイコンを描画
            foreach (var component in components)
            {
                // コンポーネントのミニサムネイルを取得
                Texture icon = AssetPreview.GetMiniThumbnail(component);

                // ミニサムネイルがnullの場合、MonoBehaviourならばMonoScriptからアイコンを取得
                if (icon == null && component is MonoBehaviour)
                {
                    var monoScript = MonoScript.FromMonoBehaviour(component as MonoBehaviour);
                    var scriptPath = AssetDatabase.GetAssetPath(monoScript);
                    icon = AssetDatabase.GetCachedIcon(scriptPath);
                }

                // アイコンが取得できなかった場合は次のコンポーネントへ
                if (icon == null)
                {
                    continue;
                }

                // アイコンを描画
                var originalColor = GUI.color;
                GUI.color = Color.white; // アイコンの色を白に設定
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit); // アイコンを描画
                GUI.color = originalColor; // 元の色に戻す

                // 次のアイコンの描画位置を調整
                iconRect.x -= iconRect.width;
            }
        }
    }
}


