using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

[System.Serializable]
public class SoundCategory
{
    public string name;
    public List<AudioClip> clips;
}

[System.Serializable]
public class TeamSoundGroup
{
    public string actionName;
    public AudioClip hormigaClip;
    public AudioClip termitaClip;
}

public enum Equipo
{
    Hormiga,
    Termita
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Fuentes de Audio")]
    public AudioSource sfxSource;
    public AudioSource musicSource;

    [Header("Música")]
    public AudioClip introMusic;
    public AudioClip menuMusic;
    public AudioClip inGameMusic;

    [Header("Efectos de Sonido (Individuales)")]
    public List<Sound> sfxClips;

    [Header("Categorías de Sonido (Variantes)")]
    public List<SoundCategory> soundCategories;

    [Header("Sonidos de Tropas por Equipo")]
    public List<TeamSoundGroup> teamSounds;

    private Dictionary<string, AudioClip> sfxDict;
    private Dictionary<string, List<AudioClip>> categoryDict;
    private Dictionary<string, TeamSoundGroup> teamSoundDict;

    private AudioClip currentMusic;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitSFXDict();
            InitCategoryDict();
            InitTeamSounds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        bool isSfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;

        musicSource.mute = !isMusicOn;
        sfxSource.mute = !isSfxOn;
    }

    void InitSFXDict()
    {
        sfxDict = new Dictionary<string, AudioClip>();
        foreach (Sound s in sfxClips)
        {
            if (!sfxDict.ContainsKey(s.name))
                sfxDict.Add(s.name, s.clip);
        }
    }

    void InitCategoryDict()
    {
        categoryDict = new Dictionary<string, List<AudioClip>>();
        foreach (SoundCategory cat in soundCategories)
        {
            if (!categoryDict.ContainsKey(cat.name))
                categoryDict.Add(cat.name, cat.clips);
        }
    }

    void InitTeamSounds()
    {
        teamSoundDict = new Dictionary<string, TeamSoundGroup>();
        foreach (TeamSoundGroup group in teamSounds)
        {
            if (!teamSoundDict.ContainsKey(group.actionName))
                teamSoundDict.Add(group.actionName, group);
        }
    }

    public void PlaySound(string soundName)
    {
        if (sfxSource.mute) return;

        if (sfxDict.TryGetValue(soundName, out AudioClip clip))
        {
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] No existe SFX con nombre '{soundName}'");
        }
    }

    public void PlaySoundFromCategory(string categoryName)
    {
        if (sfxSource.mute) return;

        if (categoryDict.TryGetValue(categoryName, out List<AudioClip> clips) && clips.Count > 0)
        {
            int idx = Random.Range(0, clips.Count);
            sfxSource.PlayOneShot(clips[idx]);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Categoría '{categoryName}' vacía o inexistente");
        }
    }

    public void PlayTeamSound(string actionName, Equipo equipo)
    {
        if (sfxSource.mute) return;

        if (teamSoundDict.TryGetValue(actionName, out TeamSoundGroup group))
        {
            AudioClip clip = equipo == Equipo.Hormiga ? group.hormigaClip : group.termitaClip;
            if (clip != null)
                sfxSource.PlayOneShot(clip);
            else
                Debug.LogWarning($"[SoundManager] Clip vacío para {equipo} en acción {actionName}");
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Acción '{actionName}' no encontrada en sonidos de equipo");
        }
    }

    public void PlayMusic(string musicType)
    {
        switch (musicType)
        {
            case "Intro":
                currentMusic = introMusic;
                break;
            case "Menu":
                currentMusic = menuMusic;
                break;
            case "InGame":
                currentMusic = inGameMusic;
                break;
            default:
                Debug.LogWarning($"[SoundManager] Música '{musicType}' no reconocida");
                return;
        }

        if (!musicSource.mute && currentMusic != null)
        {
            musicSource.clip = currentMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void ToggleMusic(bool isOn)
    {
        musicSource.mute = !isOn;
        if (isOn && currentMusic != null)
            musicSource.Play();
        else
            musicSource.Stop();

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