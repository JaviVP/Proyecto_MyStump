using TMPro;
using UnityEngine;
using UnityEngine.UI; // Usa TextMeshProUGUI si usas TMP

public class RunTimeConsole : MonoBehaviour
{
    public TextMeshProUGUI consoleText; // Usa TextMeshProUGUI si usas TextMeshPro
    private string log = "";
    private int maxLines = 20;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Solo muestra errores y excepciones, puedes incluir Log y Warning si quieres
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
        //if (type == LogType.Error || type == LogType.Exception || type == LogType.Warning)
        {
            log += $"[{type}] {logString}\n";
            TrimLog();
            consoleText.text = log;
        }
    }

    void TrimLog()
    {
        string[] lines = log.Split('\n');
        if (lines.Length > maxLines)
        {
            log = string.Join("\n", lines[^maxLines..]); // muestra últimas líneas
        }
    }
}
