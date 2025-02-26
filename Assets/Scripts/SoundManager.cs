using UnityEngine;
using UnityEngine.Audio;
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource sfxSource;  // Para efectos de sonido (botones)
    public AudioSource musicSource; // Para música de fondo
    public AudioClip sound1;
    public AudioClip sound2;
    public AudioClip backgroundMusic; // Música de fondo

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
      
        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        bool isSfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;

        musicSource.mute = !isMusicOn;
        sfxSource.mute = !isSfxOn;

        if (isMusicOn) PlayMusic();
    }

    public void PlaySound(int soundID)
    {
        if (!sfxSource.mute) // Solo reproduce si SFX está activado
        {
            AudioClip clipToPlay = null;

            switch (soundID)
            {
                case 1:
                    clipToPlay = sound1;
                    break;
                case 2:
                    clipToPlay = sound2;
                    break;
            }

            if (clipToPlay != null)
            {
                sfxSource.PlayOneShot(clipToPlay);
            }
        }
    }

    public void PlayMusic()
    {
        if (!musicSource.mute && !musicSource.isPlaying)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void ToggleMusic(bool isOn)
    {
        musicSource.mute = !isOn;
        if (isOn) PlayMusic();
        else musicSource.Stop();

        PlayerPrefs.SetInt("MusicOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSFX(bool isOn)
    {
        sfxSource.mute = !isOn;
        PlayerPrefs.SetInt("SFXOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}