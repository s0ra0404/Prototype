#if My_Utils

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace EditorExpansion
{
    public class DeepLWindow
    {
        [MenuItem("Utils/DeepL")]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(DrawDeepLWindow), false, "DeepL");
        }

        private class DrawDeepLWindow : EditorWindow
        {
            private string _inputText = string.Empty;
            private string _translationText = string.Empty;
            
            private DeepLAPI.Language _fromLanguage = DeepLAPI.Language.Ja;
            private DeepLAPI.Language _toLanguage = DeepLAPI.Language.En;
            
            private string _apiKey = string.Empty;

            private bool _isTranslating;
            
            private async void OnGUI()
            {
                DrawAPITextArea();
                EditorGUILayout.Space(5f);
                DrawLanguageSelectors();
                EditorGUILayout.Space(5f);
                DrawTextAreas();
                EditorGUILayout.Space(2f);
                DrawSeparator();
                EditorGUILayout.Space(2f);
                
                if (GUILayout.Button("Translation"))
                {
                    if (_isTranslating) return; // 同時に多重に翻訳が開始しないように
                    
                    // フォーカスを外す処理
                    EditorGUI.FocusTextInControl("");
                    
                    _isTranslating = true;
                    // 非同期処理をスタートさせる
                    _translationText = await GetTranslationAsync();
                    Repaint();
                    _isTranslating = false;
                }
                else if (GUILayout.Button("Copy"))
                {
                    // フォーカスを外す処理
                    EditorGUI.FocusTextInControl("");
                    
                    CopyTextToClipboard(_translationText);
                }
                else if (GUILayout.Button("Delete"))
                {
                    // フォーカスを外す処理
                    EditorGUI.FocusTextInControl("");
                    _inputText = "";
                    _translationText = "";
                }
            }

            private async UniTask<string> GetTranslationAsync()
            {
                if (string.IsNullOrWhiteSpace(_inputText))
                {
                    return "Please enter text to translate.";
                }

                try
                {
                    var requestUrl = DeepLAPI.BuildRequestUrl(_fromLanguage, _toLanguage, _inputText);
                    var translation = await DeepLAPI.SendTranslationRequestAsync(requestUrl);
                    return translation;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Translation failed: {e.Message}");
                    return "An unexpected error occurred";
                }
            }
            

            private void CopyTextToClipboard(string text)
            {
                GUIUtility.systemCopyBuffer = text;  // 文字列をクリップボードにコピー
            }

            private void DrawAPITextArea()
            {
                _apiKey = EditorGUILayout.PasswordField("API", _apiKey);
                DeepLAPI.SetApiKey(_apiKey);
            }
            
            private void DrawLanguageSelectors()
            {
                _fromLanguage = (DeepLAPI.Language)EditorGUILayout.EnumPopup("From", _fromLanguage);
                _toLanguage = (DeepLAPI.Language)EditorGUILayout.EnumPopup("To", _toLanguage);
            }
            
            private void DrawTextAreas()
            {
                // 矢印部分
                var textAreaStyle = new GUIStyle(EditorStyles.textArea)
                {
                    fontSize = 20,
                    fixedHeight = EditorGUIUtility.singleLineHeight * 7
                };
                
                _inputText = EditorGUILayout.TextArea(_inputText, textAreaStyle);
                _translationText = EditorGUILayout.TextArea(_translationText, textAreaStyle);
            }
            
            private void DrawSeparator()
            {
                GUILayout.Box("", GUILayout.Height(2), GUILayout.ExpandWidth(true));
            }
        }
    }

}
#endif
