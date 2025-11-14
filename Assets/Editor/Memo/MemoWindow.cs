using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace EditorExpansion
{
    public class MemoWindow
    {
        [MenuItem("Utils/Memo")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(DrawMemoWindow), false, "Memo");
        }

        private class DrawMemoWindow : EditorWindow
        {
            private readonly string _filePath = "Assets/Editor/Memo/MemoData.json";
            private string _inputTitle;
            private string _inputText;
            private Vector2 _scrollPosScope = Vector2.zero;

            private void OnGUI()
            {
                var memoData = MemoDataManager.LoadMemoData(_filePath); // 既存のデータをロード

                using (var scope = new EditorGUILayout.ScrollViewScope(_scrollPosScope))
                {
                    _scrollPosScope = scope.scrollPosition;

                    for (var i = memoData.memo.Count - 1; i >= 0; i--)
                    {
                        DrawMemo(i, memoData.memo[i].title, memoData.memo[i].dateTime, memoData.memo[i].text);
                    }
                }

                GUILayout.FlexibleSpace();

                DrawInputField();

                DrawSaveButton();
            }

            private void DrawSaveButton()
            {
                EditorGUILayout.Space(2f);

                if (GUILayout.Button("Save"))
                {
                    // フォーカスを外す処理
                    EditorGUI.FocusTextInControl("");

                    if (string.IsNullOrWhiteSpace(_inputText) || string.IsNullOrWhiteSpace(_inputTitle)) return;

                    // メモを追加
                    MemoDataManager.MemoData.Memo newMemo = new MemoDataManager.MemoData.Memo
                    {
                        dateTime = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                        title = _inputTitle,
                        text = _inputText
                    };

                    // メモを保存する
                    MemoDataManager.SaveMemoData(newMemo, _filePath);
                }

                EditorGUILayout.Space(2f);
            }

            private void DrawInputField()
            {
                GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
                EditorGUILayout.LabelField("Title");
                _inputTitle = EditorGUILayout.TextField(_inputTitle, TextAreaLayout(10, Color.white));
                EditorGUILayout.Space(2f);
                EditorGUILayout.LabelField("Memo");
                _inputText = EditorGUILayout.TextArea(_inputText, TextAreaLayout(10, Color.white, 3));
            }

            private void DrawMemo(int num, string title, string dateTime, string text)
            {
                using (new EditorGUILayout.HorizontalScope("HelpBox"))
                {
                    using (new EditorGUILayout.VerticalScope())
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Box(num.ToString(), BoxLayout(9, Color.white));
                        GUILayout.Box(dateTime, BoxLayout(9, Color.white));
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("☓", ButtonLayout(12, Color.red)))
                        {
                            MemoDataManager.DeleteMemoData(num, _filePath);
                        }

                        EditorGUILayout.EndHorizontal();


                        GUILayout.Box(title, BoxLayout(11, Color.white, 380));

                        using (new EditorGUI.IndentLevelScope(1))
                        {
                            EditorGUILayout.LabelField(text, TextLayout(10, Color.white, true));
                        }
                    }
                }
            }

            private GUIStyle BoxLayout(int fontSize, Color fontColor, int fixedWidth = 0,
                TextAnchor textAnchor = TextAnchor.MiddleLeft)
            {
                // カスタムGUIスタイルを作成
                var boxStyle = new GUIStyle(GUI.skin.box)
                {
                    // フォントサイズを変更
                    fontSize = fontSize,
                    normal = new GUIStyleState { textColor = fontColor },
                    fixedWidth = fixedWidth,
                    alignment = textAnchor
                };

                return boxStyle;
            }

            private GUIStyle TextLayout(int fontSize, Color fontColor, bool wordWrap = false)
            {
                var textLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = fontSize,
                    normal = new GUIStyleState { textColor = fontColor },
                    wordWrap = wordWrap,
                };

                return textLabelStyle;
            }

            private GUIStyle TextAreaLayout(int fontSize, Color fontColor, int fixedHeight = 0)
            {
                var textLabelStyle = new GUIStyle(EditorStyles.textArea)
                {
                    fontSize = fontSize,
                    normal = new GUIStyleState { textColor = fontColor },
                    fixedHeight = EditorGUIUtility.singleLineHeight * fixedHeight
                };

                return textLabelStyle;
            }

            private GUIStyle ButtonLayout(int fontSize, Color fontColor)
            {
                var textLabelStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = fontSize,
                    normal = new GUIStyleState { textColor = fontColor }
                };

                return textLabelStyle;
            }
        }
    }
}