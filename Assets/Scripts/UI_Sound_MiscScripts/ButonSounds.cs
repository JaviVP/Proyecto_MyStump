using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    [Tooltip("Nombre del sonido que se reproducir� al pulsar este bot�n")]
    [SerializeField] private string soundName = "BotonNormal";

    void Awake()
    {
        // Solo ejecuta esta l�gica una vez (en un objeto centralizador)
        if (gameObject.name != "ButtonSoundInitializer") return;

        // Encuentra todos los botones activos en la escena
        Button[] botones = Object.FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button boton in botones)
        {
            // Si no tienen el script, se lo a�ade
            if (boton.GetComponent<ButtonSounds>() == null)
            {
                var nuevo = boton.gameObject.AddComponent<ButtonSounds>();
                nuevo.soundName = boton.name.ToLower().Contains("back") ? "BotonBack" : "BotonNormal";
            }
        }
    }

    void Start()
    {
        // A�ade el listener al bot�n
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