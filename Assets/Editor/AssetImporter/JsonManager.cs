using System;
using System.IO;
using UnityEngine;

namespace EditorExpansion
{
    public static class JsonManager
    {
        /// <summary>
        /// データをJSON形式で保存します。
        /// </summary>
        public static bool SaveData<T>(T data, string filePath)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(filePath, json);

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"データの保存中にエラーが発生しました: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// JSONファイルからデータを読み込みます。
        /// </summary>
        public static T LoadData<T>(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    T data = JsonUtility.FromJson<T>(json);

                    return data;
                }
                else
                {
                    Debug.LogWarning($"ファイルが存在しません: {filePath}。デフォルト値を返します。");
                    return default;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"データの読み込み中にエラーが発生しました: {ex.Message}");
                return default;
            }
        }
    }
}