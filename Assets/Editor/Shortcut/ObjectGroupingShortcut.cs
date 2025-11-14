using UnityEditor;
using UnityEngine;

namespace EditorExpansion
{
    public static class ObjectGroupingShortcut
    {
        // 選択したオブジェクトの階層にグループ化
        [MenuItem("Edit/Group %g")]
        private static void Execute()
        {
            var gameObjects = Selection.gameObjects;

            if (gameObjects.Length == 0) return;

            // まず、選択されたオブジェクトの親を取得
            var parentTransform = gameObjects[0].transform.parent;

            // グループオブジェクトを作成
            var group = new GameObject("Group");
            Undo.RegisterCreatedObjectUndo(group, "Group");

            // グループを選択されたオブジェクトと同じ親に設定
            group.transform.SetParent(parentTransform);

            foreach (var go in gameObjects)
            {
                Undo.SetTransformParent(go.transform, group.transform, "Group");
            }

            // グループを選択
            Selection.activeGameObject = group;
        }

        [MenuItem("Edit/Group %g", true)]
        private static bool CanExecute()
        {
            return Selection.transforms.Length > 0;
        }
    }
}


