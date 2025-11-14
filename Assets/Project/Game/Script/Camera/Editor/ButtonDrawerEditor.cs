using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Ver2
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    [CanEditMultipleObjects] 
    public class ButtonDrawerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            // 対象オブジェクト
            var mono = target as MonoBehaviour;
            if (mono == null) return;

            // メソッドをすべて取得
            var methods = mono.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                // ButtonAttribute がついているメソッドのみ
                var attr = method.GetCustomAttribute<ButtonAttribute>();
                if (attr == null) continue;

                string label = attr.Label ?? method.Name;

                if (GUILayout.Button(label))
                {
                    method.Invoke(mono, null);
                }
            }
        }
    }
}
