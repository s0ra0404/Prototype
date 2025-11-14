using UnityEditor;
using UnityEngine;

namespace EditorExpansion
{
    /// <summary>
    /// ヒエラルキーウィンドウでGameObjectのアクティブ状態を切り替えるクラス
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyActiveToggle
    {
        private const int IconWidth = 10;

        static HierarchyActiveToggle() =>
            EditorApplication.hierarchyWindowItemOnGUI += (id, rect) =>
            {
                if (EditorUtility.InstanceIDToObject(id) is GameObject go)
                {
                    var newActive = GUI.Toggle(
                        new Rect(rect.xMax - IconWidth, rect.y, IconWidth, rect.height),
                        go.activeSelf, string.Empty);

                    if (newActive != go.activeSelf)
                        go.SetActive(newActive);
                }
            };
    }
}

