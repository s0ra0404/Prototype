using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace EditorExpansion
{
    public class FolderCreator : EditorWindow
    {
        [MenuItem("Utils/CreateFolderWindow")]
        public static void ShowCreateWindow()
        {
            GetWindow<FolderCreator>("CreateFolder");
        }
        
        private static readonly string _filePath = "Assets/Editor/Folder/FolderPathData.json";
        private static string _sceneName;
        
        private void OnGUI()
        {
            _sceneName = EditorGUILayout.TextField(_sceneName);

            if(GUILayout.Button("Save"))
            {
                Save();
            }
            if (GUILayout.Button("Create"))
            {
                Create();
            }
        }
        
        private static void Save()
        {
            var newFolderPath = new FolderPathDataManager.FolderPathData
            {
                folderPath = new List<string>
                {
                    "Assets/Project",
                    "Assets/Project/" + _sceneName,
                    "Assets/Project/" + _sceneName + "/Script",
                    "Assets/Project/" + _sceneName + "/Prefab",
                    "Assets/Project/" + _sceneName + "/Animation",
                    "Assets/Project/" + _sceneName + "/Material",
                    "Assets/Project/" + _sceneName + "/PhysicsMaterial",
                    "Assets/Project/" + _sceneName + "/Font",
                    "Assets/Project/" + _sceneName + "/Texture",
                    "Assets/Project/" + _sceneName + "/Sprite",
                    "Assets/Project/" + _sceneName + "/Audio",
                    "Assets/Project/" + _sceneName + "/Model",
                    "Assets/Project/" + _sceneName + "/Shader",
                    "Assets/Project/" + _sceneName + "/Other",
                    "Assets/Project/" + _sceneName + "/Scene"
                }
            };

            // メモを保存する
            FolderPathDataManager.SavePathData(newFolderPath, _filePath);
        }

        private static void Create()
        {
            FolderPathDataManager.FolderPathData folderPathData = FolderPathDataManager.LoadPathData(_filePath);  // 既存のデータをロード

            foreach (var path in folderPathData.folderPath)
            {
                CreateFolder(ExtractFolderName(path).Item1, ExtractFolderName(path).Item2);
            }
        }
        
        private static void CreateFolder(string parentFolder ,string name)
        {
            // フォルダの完全なパス
            string folderPath = Path.Combine(parentFolder, name);

            // すでに同じ名前のフォルダが存在する場合
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError("Folder already exists: " + folderPath);
                return;
            }

            // フォルダを作成
            AssetDatabase.CreateFolder(parentFolder, name);

            // AssetDatabaseで変更を認識させる
            AssetDatabase.Refresh();

            Debug.Log("Folder created at: " + folderPath);
        }
        
        // パスから最後の部分（フォルダ名）を抽出
        private static Tuple<string, string> ExtractFolderName(string path)
        {
            // 最後のスラッシュの位置を取得
            int lastSlashIndex = path.LastIndexOf('/');

            // 最後のスラッシュの位置から後ろの部分を抽出
            if (lastSlashIndex >= 0)
            {
                string beforeSlash = path.Substring(0, lastSlashIndex);  // スラッシュの前
                string afterSlash = path.Substring(lastSlashIndex + 1); // スラッシュの後
                return new Tuple<string, string>(beforeSlash, afterSlash);
            }
            return new Tuple<string, string>("", ""); // スラッシュが無ければそのまま返す
        }
    }

}

