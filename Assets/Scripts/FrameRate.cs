using UnityEngine;

public class FrameRate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("FPS: " + (1.0f / Time.deltaTime));
    }
}
