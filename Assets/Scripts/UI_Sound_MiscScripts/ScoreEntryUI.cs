using TMPro;
using UnityEngine;

public class ScoreEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statsText;

    // Nuevo método con solo score y partidas ganadas
    public void SetupEntry(int rank, string playerName, int partidasGanadas, int score)
    {
        if (nameText != null && statsText != null)
        {
            nameText.text = $"{rank}. {playerName}";
            statsText.text = $"{partidasGanadas}                          |                                               {score}";
        }
        else
        {
            Debug.LogError("Text fields are not assigned in ScoreEntryUI.");
        }
    }
}