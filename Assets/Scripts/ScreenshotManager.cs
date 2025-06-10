using UnityEngine;
using System.IO;

public class ScreenshotTaker : MonoBehaviour
{
    public int superSize = 1; // Aumenta la resolución (por ejemplo, 2 o 4 = más calidad)

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            string folderPath = "Screenshots";
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string filename = $"capture_{System.DateTime.Now:yyyyMMdd_HHmmss}.png";
            ScreenCapture.CaptureScreenshot(Path.Combine(folderPath, filename), superSize);
            Debug.Log("Captura guardada en: " + Path.Combine(folderPath, filename));
        }
    }
}