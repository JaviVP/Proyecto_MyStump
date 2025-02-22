using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;
    public AudioClip sound1;
    public AudioClip sound2;

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
