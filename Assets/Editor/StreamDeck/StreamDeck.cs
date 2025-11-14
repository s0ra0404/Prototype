using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class StreamDeck : EditorWindow
{
    [SerializeField] private VisualTreeAsset _mVisualTreeAsset;

    [MenuItem("Utils/StreamDeck")]
    public static void ShowExample()
    {
        StreamDeck wnd = GetWindow<StreamDeck>();
        wnd.titleContent = new GUIContent("StreamDeck");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        
        VisualElement fromUxml = _mVisualTreeAsset.Instantiate();
        root.Add(fromUxml);
        root.Q<Button>("Server_Button").clickable.clicked += () => Application.OpenURL("https://www.oracle.com/jp/cloud/");
        root.Q<Button>("Editor_Button").clickable.clicked += OpenEditorForSelection;
        root.Q<Button>("Google_Button").clickable.clicked += () => Application.OpenURL("https://www.google.com");
        root.Q<Button>("Explorer_Button").clickable.clicked += () =>
        {
            // デスクトップのパスを取得
            string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            // エクスプローラーで開く
            Process.Start("explorer.exe", desktopPath);
        };
        root.Q<Button>("Github_Button").clickable.clicked += () => Application.OpenURL("https://github.com/");
        root.Q<Button>("Deepl_Button").clickable.clicked += () => Application.OpenURL("https://www.deepl.com/ja/translator");
        root.Q<Button>("Chatgpt_Button").clickable.clicked += () => Application.OpenURL("https://chatgpt.com/");
        root.Q<Button>("Copilot_Button").clickable.clicked += () => Application.OpenURL("https://copilot.microsoft.com/");
        root.Q<Button>("Figma_Button").clickable.clicked += () => Application.OpenURL("https://www.figma.com/ja-jp/");
        root.Q<Button>("Zenn_Button").clickable.clicked += () => Application.OpenURL("https://zenn.dev/");
        root.Q<Button>("Qiita_Button").clickable.clicked += () => Application.OpenURL("https://qiita.com/");
        root.Q<Button>("Note_Button").clickable.clicked += () => Application.OpenURL("https://note.com/");
        root.Q<Button>("Creator_Button").clickable.clicked += () => Application.OpenURL("https://creator.cluster.mu/");
        root.Q<Button>("HatenaBlog_Button").clickable.clicked += () => Application.OpenURL("https://hatena.blog/");
        root.Q<Button>("Soft-Rime_Button").clickable.clicked += () => Application.OpenURL("https://soft-rime.com/");
        root.Q<Button>("Otologic_Button").clickable.clicked += () => Application.OpenURL("https://otologic.jp/");
        root.Q<Button>("MaouAudio_Button").clickable.clicked += () => Application.OpenURL("https://maou.audio/");
        root.Q<Button>("Ryu110_Button").clickable.clicked += () => Application.OpenURL("https://ryu110.com/");
        root.Q<Button>("SoundEffect-Lab_Button").clickable.clicked += () => Application.OpenURL("https://soundeffect-lab.info/");
    }
    
    private static void OpenEditorForSelection()
    {
        string selectedPath = null;
        var obj = Selection.activeObject;
        if (obj != null)
        {
            var assetPath = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(assetPath))
                selectedPath = Path.GetFullPath(assetPath);
        }

        TryOpenConfiguredEditor(selectedPath);
    }

    private static void TryOpenConfiguredEditor(string fileToOpen = null)
    {
        string editorPath = GetConfiguredScriptEditorPath();
        string editorArgs = GetConfiguredScriptEditorArgs(); // optional

        if (string.IsNullOrEmpty(editorPath))
        {
            Debug.LogError("Unity に設定された外部スクリプトエディターが見つかりません。Edit → Preferences → External Tools で設定してください。");
            return;
        }

        // macOSの場合は.appを open -a で扱うと安全（editorPath が .app だったら）
        if (Application.platform == RuntimePlatform.OSXEditor && editorPath.EndsWith(".app", StringComparison.OrdinalIgnoreCase))
        {
            if (string.IsNullOrEmpty(fileToOpen))
                Process.Start("open", $"-a \"{editorPath}\"");
            else
                Process.Start("open", $"-a \"{editorPath}\" --args \"{fileToOpen}\"");
            return;
        }

        // 普通は exe に対してファイルパスを引数として渡す
        try
        {
            if (!string.IsNullOrEmpty(fileToOpen))
            {
                // もし EditorPrefs に引数テンプレートがあればそれを使って (単純な置換)
                if (!string.IsNullOrEmpty(editorArgs))
                {
                    // よくある置換パターンをカバー
                    string args = editorArgs
                        .Replace("%file%", $"\"{fileToOpen}\"")
                        .Replace("{file}", $"\"{fileToOpen}\"")
                        .Replace("$(File)", $"\"{fileToOpen}\"")
                        .Replace("%1", $"\"{fileToOpen}\"");
                    Process.Start(editorPath, args);
                }
                else
                {
                    Process.Start(editorPath, $"\"{fileToOpen}\"");
                }
            }
            else
            {
                Process.Start(editorPath);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"外部エディタの起動に失敗しました: {editorPath}\n{ex}");
        }
    }

    private static string GetConfiguredScriptEditorPath()
    {
        // 1) リフレクションで ScriptEditorUtility 系を探す
        var asm = typeof(Editor).Assembly;
        string[] candidates =
        {
            "UnityEditor.ScriptEditorUtility",
            "UnityEditorInternal.ScriptEditorUtility",
            "UnityEditorInternal.ScriptEditor"
        };

        foreach (var typeName in candidates)
        {
            var t = asm.GetType(typeName);
            if (t == null) continue;
            var m = t.GetMethod("GetExternalScriptEditor", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (m == null) continue;
            try
            {
                var val = m.Invoke(null, null) as string;
                if (!string.IsNullOrEmpty(val)) return val;
            }
            catch { /* ignore */ }
        }

        // 2) EditorPrefs の既知キーを試す（古いバージョンで使われることがある）
        if (EditorPrefs.HasKey("kScriptsDefaultApp"))
        {
            var v = EditorPrefs.GetString("kScriptsDefaultApp");
            if (!string.IsNullOrEmpty(v)) return v;
        }
        if (EditorPrefs.HasKey("kExternalScriptEditor"))
        {
            var v = EditorPrefs.GetString("kExternalScriptEditor");
            if (!string.IsNullOrEmpty(v)) return v;
        }

        // 見つからなければ null
        return null;
    }

    private static string GetConfiguredScriptEditorArgs()
    {
        if (EditorPrefs.HasKey("kScriptEditorArgs"))
            return EditorPrefs.GetString("kScriptEditorArgs");
        return null;
    }
}
