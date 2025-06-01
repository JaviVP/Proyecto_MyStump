using UnityEngine;
using UnityEngine.UI;

public class FondoPantallaManager : MonoBehaviour
{
    public Sprite[] fondos;               // Lista de fondos disponibles
    public Image fondoPantallaUI;         // UI Image donde se muestra el fondo
    private static Sprite fondoSeleccionado;
    private const string ULTIMO_FONDO_KEY = "UltimoFondoIndex";

    void Awake()
    {
        // Solo seleccionar nuevo fondo si aún no hay uno definido
        if (fondoSeleccionado == null)
        {
            int ultimoIndex = PlayerPrefs.GetInt(ULTIMO_FONDO_KEY, -1);
            int nuevoIndex;

            // Evitar repetir el último fondo
            do
            {
                nuevoIndex = Random.Range(0, fondos.Length);
            } while (fondos.Length > 1 && nuevoIndex == ultimoIndex);

            fondoSeleccionado = fondos[nuevoIndex];
            PlayerPrefs.SetInt(ULTIMO_FONDO_KEY, nuevoIndex);
            PlayerPrefs.Save();
        }

        if (fondoPantallaUI != null)
        {
            fondoPantallaUI.sprite = fondoSeleccionado;
        }
    }
}
