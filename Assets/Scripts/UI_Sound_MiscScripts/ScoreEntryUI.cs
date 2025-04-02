using TMPro;
using UnityEngine;

public class ScoreEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statsText;

    // Este método debe ser llamado para configurar los valores de cada entrada
    public void SetupEntry(int rank, string playerName, int partidasGanadas, int hormigasEliminadas, int termitasEliminadas, int parcelasHormigas, int parcelasTermitas)
    {
        if (nameText != null && statsText != null)
        {
            nameText.text = $"{rank}. {playerName}";

            statsText.text = $"Partidas: {partidasGanadas} Hormigas eliminadas: {hormigasEliminadas} Termitas eliminadas: {termitasEliminadas} Parcelas Hormigas: {parcelasHormigas} Parcelas Termitas: {parcelasTermitas}";
        }
        else
        {
            Debug.LogError("Text fields are not assigned in ScoreEntryUI.");
        }
    }
}