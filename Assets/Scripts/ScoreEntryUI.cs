using TMPro;
using UnityEngine;

public class ScoreEntryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statsText;

    // Este m�todo debe ser llamado para configurar los valores de cada entrada
    public void SetupEntry(string playerName, int partidasGanadas, int hormigasEliminadas, int termitasEliminadas, int parcelasHormigas, int parcelasTermitas)
    {
        // Asegurarnos de que los textos est�n correctamente asignados
        if (nameText != null && statsText != null)
        {
            nameText.text = playerName;

            // Mostramos las estad�sticas en un formato adecuado
            statsText.text = $"Partidas: {partidasGanadas} Hormigas eliminadas: {hormigasEliminadas} Termitas eliminadas: {termitasEliminadas} Parcelas Hormigas: {parcelasHormigas} Parcelas Termitas: {parcelasTermitas}";
        }
        else
        {
            Debug.LogError("Text fields are not assigned in ScoreEntryUI.");
        }
    }
}