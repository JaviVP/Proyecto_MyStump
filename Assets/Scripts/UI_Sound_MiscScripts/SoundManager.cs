using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string name;    // clave para PlaySound("name")
    public AudioClip clip;
}

[System.Serializable]
public class SoundCategory
{
    public string name;          // clave para PlaySoundFromCategory("name")
    public List<AudioClip> clips;
}

[System.Serializable]
public class TeamSoundGroup
{
    public string actionName;    // "Mover", "Colocar", "Eliminar", etc.
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
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambienceSource;

    [Header("Música")]
    [SerializeField] private AudioClip introMusic;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip inGameMusic;

    [Header("SFX Individuales")]
    [SerializeField] private List<Sound> sfxClips;

    [Header("Categorías de SFX")]
    [SerializeField] private List<SoundCategory> soundCategories;

    [Header("Sonidos de Tropas por Equipo")]
    [SerializeField] private List<TeamSoundGroup> teamSounds;

    [Header("Ajustes de Fade")]
    [SerializeField] private float fadeDuration = 1.0f;

    [SerializeField] private Dictionary<string, AudioClip> sfxDict;
    [SerializeField] private Dictionary<string, List<AudioClip>> categoryDict;
    [SerializeField] private Dictionary<string, TeamSoundGroup> teamSoundDict;

    private Coroutine fadeCoroutine;
    private AudioClip currentMusic;

    void Awake()
    {
        // Singleton  
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Inicializaciones
            InitSFXDict();
            InitCategoryDict();
            InitTeamSounds();

            // Escucha cambio de escena
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Carga preferencias
        bool isMusicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        bool isSfxOn = PlayerPrefs.GetInt("SFXOn", 1) == 1;

        musicSource.mute = !isMusicOn;
        sfxSource.mute = !isSfxOn;

        // Música según escena inicial
        PlayMusicForScene();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Inicializa diccionario de SFX individuales
    void InitSFXDict()
    {
        sfxDict = new Dictionary<string, AudioClip>();
        foreach (var s in sfxClips)
            if (!sfxDict.ContainsKey(s.name))
                sfxDict.Add(s.name, s.clip);
    }

    // Inicializa diccionario de categorías
    void InitCategoryDict()
    {
        categoryDict = new Dictionary<string, List<AudioClip>>();
        foreach (var cat in soundCategories)
            if (!categoryDict.ContainsKey(cat.name))
                categoryDict.Add(cat.name, cat.clips);
    }

    // Inicializa diccionario de sonidos de equipo
    void InitTeamSounds()
    {
        teamSoundDict = new Dictionary<string, TeamSoundGroup>();
        foreach (var grp in teamSounds)
            if (!teamSoundDict.ContainsKey(grp.actionName))
                teamSoundDict.Add(grp.actionName, grp);
    }

    // --------------------
    //  SFX Methods
    // --------------------

    /// <summary>Reproduce un SFX individual por nombre.</summary>
    public void PlaySound(string soundName)
    {
        if (sfxSource.mute) return;

        if (sfxDict.TryGetValue(soundName, out var clip))
            sfxSource.PlayOneShot(clip);
        else
            Debug.LogWarning($"[SoundManager] SFX '{soundName}' no encontrado");
    }

    /// <summary>Reproduce un clip aleatorio de una categoría.</summary>
    public void PlaySoundFromCategory(string categoryName)
    {
        if (categoryDict.TryGetValue(categoryName, out var clips) && clips.Count > 0)
        {
            int idx = Random.Range(0, clips.Count);
            AudioClip selectedClip = clips[idx];

            if (categoryName == "Ambient")
            {
                string currentScene = SceneManager.GetActiveScene().name;
                if ((currentScene == "VladTEST" || currentScene == "Championship") && !ambienceSource.mute)
                {
                    ambienceSource.volume = 0.1f;
                    ambienceSource.PlayOneShot(selectedClip);
                }
            }
            else
            {
                if (!sfxSource.mute)
                    sfxSource.PlayOneShot(selectedClip);
            }
        }
        else
        {
            Debug.LogWarning($"[SoundManager] Categoría '{categoryName}' vacía o no existe");
        }
    }

    /// <summary>Reproduce el SFX de tropa adecuado según acción y equipo.</summary>
    public void PlayTeamSound(string actionName, Equipo equipo)
    {
        if (sfxSource.mute) return;

        if (teamSoundDict.TryGetValue(actionName, out var group))
        {
            AudioClip clip = equipo == Equipo.Hormiga ? group.hormigaClip : group.termitaClip;
            if (clip != null)
                sfxSource.PlayOneShot(clip);
            else
                Debug.LogWarning($"[SoundManager] Clip de '{actionName}' para {equipo} no asignado");
        }
        else
            Debug.LogWarning($"[SoundManager] Acción de tropa '{actionName}' no encontrada");
    }

    // --------------------
    //  Music Methods
    // --------------------

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene();
    }

    /// <summary>
    /// Selecciona Intro/Menu/InGame según el nombre de la escena.
    /// </summary>
    void PlayMusicForScene()
    {
        if (musicSource.mute) return;

        string name = SceneManager.GetActiveScene().name;
        AudioClip target = null;

        if (name == "MainMenu")
            target = introMusic;
        else if (name == "Hub" || name == "Settings" || name == "NameSelector" || name == "ScoreBoard")
            target = menuMusic;
        else if (name == "Championship" || name == "VladTEST")
            target = inGameMusic;

        if (target != null)
        {
            if (Time.timeScale == 0)
            {
                musicSource.clip = target;
                musicSource.Play();
            }
            else if (target != currentMusic)
            {
                currentMusic = target;
                if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
                fadeCoroutine = StartCoroutine(FadeToNewTrack(target));
            }
        }
    }

    /// <summary>Fade out/in hacia la nueva pista.</summary>
    IEnumerator FadeToNewTrack(AudioClip newClip)
    {
        float startVol = musicSource.volume;

        // Fade out
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVol, 0, t / fadeDuration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        // Fade in
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVol, t / fadeDuration);
            yield return null;
        }

        musicSource.volume = startVol;
    }

    /// <summary>Activa/desactiva música y guarda opción.</summary>
    public void ToggleMusic(bool isOn)
    {
        musicSource.mute = !isOn;

        if (!isOn)
        {
            musicSource.Stop();
            currentMusic = null;  // Reiniciar referencia para forzar recarga
        }
        else
        {
            // Si no está sonando música o el clip es null, forzamos que se reproduzca
            if (!musicSource.isPlaying || musicSource.clip == null)
            {
                PlayMusicForScene();
            }
            else
            {
                musicSource.UnPause();
            }
        }

        PlayerPrefs.SetInt("MusicOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    /// <summary>Activa/desactiva SFX y guarda opción.</summary>
    public void ToggleSFX(bool isOn)
    {
        sfxSource.mute = !isOn;
        ambienceSource.mute = !isOn;

        PlayerPrefs.SetInt("SFXOn", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void StopMusic()
    {
        if (musicSource.isPlaying)
            musicSource.Stop();
    }
}