using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ButtonSoundInjector : MonoBehaviour
{
    private static ButtonSoundInjector instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(InjectAfterFrame());
    }

    IEnumerator InjectAfterFrame()
    {
        yield return new WaitForEndOfFrame();

        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();

        foreach (Button b in allButtons)
        {
            // Filtrar botones de escenas actuales, evitar prefabs u ocultos no usados
            if (b.gameObject.scene.name == null || b.hideFlags != HideFlags.None)
                continue;

            if (b.GetComponent<ButtonSounds>() == null)
            {
                var nuevo = b.gameObject.AddComponent<ButtonSounds>();
                string nombre = b.name.ToLower();
                nuevo.SetSoundName(nombre.Contains("back") ? "BotonBack" : "BotonNormal");
            }
        }
    }
}