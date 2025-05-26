using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    [Tooltip("Nombre del sonido que se reproducirá al pulsar este botón")]
    [SerializeField] private string soundName = "BotonNormal";

    void Awake()
    {
        // Solo ejecuta esta lógica una vez (en un objeto centralizador)
        if (gameObject.name != "ButtonSoundInitializer") return;

        // Encuentra todos los botones activos en la escena
        Button[] botones = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button boton in botones)
        {
            // Si no tienen el script, se lo añade
            if (boton.GetComponent<ButtonSounds>() == null)
            {
                var nuevo = boton.gameObject.AddComponent<ButtonSounds>();
                nuevo.soundName = boton.name.ToLower().Contains("back") ? "BotonBack" : "BotonNormal";
            }
        }
    }

    void Start()
    {
        // Añade el listener al botón
        Button boton = GetComponent<Button>();
        if (boton != null)
        {
            boton.onClick.AddListener(() =>
            {
                SoundManager.instance.PlaySound(soundName);
            });
        }
    }
}