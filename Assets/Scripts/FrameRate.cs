using UnityEngine;
using TMPro;
public class FrameRate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI frames;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        frames.text = "Fps: " + Mathf.RoundToInt(1.0f / Time.deltaTime);
        Debug.Log("FPS: " + (1.0f / Time.deltaTime));
    }
}
