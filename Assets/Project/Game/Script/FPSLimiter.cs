using UnityEngine;

public class FPSLimiter : MonoBehaviour
{
    [SerializeField] private int targetFPS = 60;

    private void Start()
    {
        QualitySettings.vSyncCount = 0; // VSyncを無効化
        Application.targetFrameRate = targetFPS;
    }
}
