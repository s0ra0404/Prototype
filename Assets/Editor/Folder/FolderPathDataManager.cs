using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EditorExpansion
{
    public static class FolderPathDataManager
    {
        /// <summary>
        /// パスデータを格納する構造体
        /// </summary>
        [Serializable]
        public struct FolderPathData
        {
            public List<string> folderPath;
        }
    
        public static void SavePathData(FolderPathData newPathData, string filePath)
        {
            FolderPathData pathData = newPathData;  // 既存のデータをロード

            // JSON 形式で保存
            string json = JsonUtility.ToJson(pathData, true);

            // ファイルに書き込む
            File.WriteAllText(filePath, json);
        }

        public static FolderPathData LoadPathData(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                FolderPathData loadedData = JsonUtility.FromJson<FolderPathData>(json);
                return loadedData;
            }
            else
            {
                // ファイルが存在しない場合、新しい MemoData を返す
                return new FolderPathData { folderPath = new List<string>() };
            }
        }
    }
}