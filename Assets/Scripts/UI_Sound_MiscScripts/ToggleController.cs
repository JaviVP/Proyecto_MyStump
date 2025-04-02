using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle sfxToggle;

    void Start()
    {
        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        bool isSfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;

        musicToggle.isOn = isMusicOn;
        sfxToggle.isOn = isSfxOn;

        musicToggle.onValueChanged.AddListener(SoundManager.instance.ToggleMusic);
        sfxToggle.onValueChanged.AddListener(SoundManager.instance.ToggleSFX);
    }
}
