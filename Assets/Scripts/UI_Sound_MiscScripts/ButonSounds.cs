using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    [SerializeField] private string soundName = "BotonNormal";
    private bool listenerAdded = false;

    public void SetSoundName(string newName)
    {
        soundName = newName;
    }

    void OnEnable()
    {
        TryAddListener();
    }

    void Start()
    {
        TryAddListener();
    }

    private void TryAddListener()
    {
        if (listenerAdded) return;

        Button boton = GetComponent<Button>();
        if (boton != null)
        {
            boton.onClick.AddListener(() =>
            {
                if (SoundManager.instance != null)
                    SoundManager.instance.PlaySound(soundName);
                else
                    Debug.LogWarning("SoundManager no está inicializado.");
            });

            listenerAdded = true;
        }
    }
}