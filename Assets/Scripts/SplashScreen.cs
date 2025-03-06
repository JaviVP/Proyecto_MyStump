using UnityEngine;
using UnityEngine.SceneManagement;
public class SplashScreen : MonoBehaviour
{
    private float delay = 2f;

    void Start()
    {
        Invoke("LoadMainScene", delay);
    }

    void LoadMainScene()
    {
        SceneManager.LoadScene(2);
    }
}
