using UnityEditor;

namespace EditorExpansion
{
    public static class PlayModeShortcut
    {
        // F5キーでゲームを再生
        [MenuItem("Edit/Run _F5", priority = 140)]
        private static void Run()
        {
            EditorApplication.isPlaying = true;
        }

        // 再生中でない場合にのみ再生できるようにする
        [MenuItem("Edit/Run _F5", validate = true)]
        private static bool CanRun()
        {
            return !EditorApplication.isPlaying;
        }

        // F5キーでゲームを停止
        [MenuItem("Edit/Stop _F6", priority = 141)]
        private static void Stop()
        {
            EditorApplication.isPlaying = false;
        }

        // 停止中でない場合にのみ停止できるようにする
        [MenuItem("Edit/Stop _F6", validate = true)]
        private static bool CanStop()
        {
            return EditorApplication.isPlaying;
        }
    }
}


