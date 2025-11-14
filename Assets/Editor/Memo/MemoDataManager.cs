using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EditorExpansion
{
    public static class MemoDataManager
    {
        /// <summary>
        /// メモデータを格納する構造体
        /// </summary>
        [Serializable]
        public struct MemoData
        {
            public List<Memo> memo;
            
            [Serializable]
            public struct Memo
            {
                public string dateTime;
                public string title;
                public string text;
            }
        }
        
        public static void SaveMemoData(MemoData.Memo newMemo, string filePath)
        {
            MemoData memoData = LoadMemoData(filePath);  // 既存のデータをロード
    
            // 新しいメモを追加
            memoData.memo.Add(newMemo);
    
            // JSON 形式で保存
            string json = JsonUtility.ToJson(memoData, true);
    
            // ファイルに書き込む
            File.WriteAllText(filePath, json);
        }
    
        public static MemoData LoadMemoData(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                MemoData loadedData = JsonUtility.FromJson<MemoData>(json);
                return loadedData;
            }
            else
            {
                // ファイルが存在しない場合、新しい MemoData を返す
                return new MemoData { memo = new List<MemoData.Memo>() };
            }
        }
    
        public static void DeleteMemoData(int memoNum, string filePath)
        {
            MemoData memoData = LoadMemoData(filePath);  // 既存のデータをロード
    
            // 新しいメモを追加
            memoData.memo.RemoveAt(memoNum);
    
            // JSON 形式で保存
            string json = JsonUtility.ToJson(memoData, true);
    
            // ファイルに書き込む
            File.WriteAllText(filePath, json);
        }
    }
}


