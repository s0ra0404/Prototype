using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace EditorExpansion
{
    [InitializeOnLoad]
    public static class AutoSave
    {
        private const float SaveInterval = 60f; // 自動保存の間隔 (秒)
        private static float _lastSaveTime;
    
        static AutoSave()
        {
            _lastSaveTime = (float)EditorApplication.timeSinceStartup;
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            // ゲーム再生中は処理をスキップ
            if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode) return; 

            if (EditorApplication.timeSinceStartup - _lastSaveTime >= SaveInterval)
            {
                SaveScene();
                _lastSaveTime = (float)EditorApplication.timeSinceStartup;
            }
        }

        private static void SaveScene()
        {
            if (!EditorSceneManager.SaveOpenScenes())
            {
                Debug.LogError("not AutoSave");
            }
            else
            {
                Debug.Log("AutoSave");
            }
            AssetDatabase.SaveAssets();
        }
    }
}

