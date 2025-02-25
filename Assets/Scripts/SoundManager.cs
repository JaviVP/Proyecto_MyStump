using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip sound1;
    [SerializeField] private AudioClip sound2;
    [SerializeField] private AudioClip backGroundMusic;

    private void Start()
    {
        audioSource.clip = backGroundMusic;
        audioSource.Play();
    }
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
        }
    }

    public void PlaySound(int soundID)
    {
        audioSource.Stop(); // Detiene cualquier sonido anterior
        if (soundID == 1)
            audioSource.clip = sound1;
        else if (soundID == 2)
            audioSource.clip = sound2;

        audioSource.Play();
    }
}
