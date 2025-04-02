using UnityEngine;
using TMPro;

public class ChampionshipGameSelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI partidasText;  // Muestra el n�mero de partidas
 // Muestra el n�mero de partidas
    private int[] allowedOptions = { 3, 5, 7, 9 };          // Opciones posibles
    private int currentIndex = 0;                          // Inicia en 3 partidas (�ndice 0)



    private void Start()
    {
        UpdateDisplay();  // Mostrar valor inicial al arrancar
    }


    public void IncreaseGames()
    {
        currentIndex = (currentIndex + 1) % allowedOptions.Length;
        UpdateDisplay();
    }

    public void DecreaseGames()
    {
        currentIndex = (currentIndex - 1 + allowedOptions.Length) % allowedOptions.Length;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        int selectedNumber = allowedOptions[currentIndex];
        partidasText.text = $"{selectedNumber}";
        PlayerPrefs.SetInt("NumeroPartidasCampeonato", selectedNumber);  // Guardar selecci�n
        Debug.Log($"Partidas seleccionadas: {selectedNumber}");
    }


    // Si quieres acceder desde otro script:
    public int GetSelectedNumber()
    {
        return allowedOptions[currentIndex];
    }
}
