using UnityEngine;
using UnityEngine.UI;
public class ButonSounds : MonoBehaviour
{
    [SerializeField] private int soundID = 1; // Por defecto usa el primer sonido

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => SoundManager.instance.PlaySound(soundID));
    }
}
