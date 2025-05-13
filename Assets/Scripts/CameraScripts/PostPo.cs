using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostPo : MonoBehaviour
{

    public Volume globalVolume; // Assign this in the Inspector
    private WhiteBalance whiteBalance;

    void Start()
    {
        globalVolume= GetComponent<Volume>();
        if (globalVolume != null && globalVolume.profile.TryGet(out whiteBalance))
        {
            // Example: set the initial temperature to 15 (warmer)
            whiteBalance.temperature.value = 15f;
        }
        else
        {
            Debug.LogWarning("WhiteBalance override not found in Volume profile.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
