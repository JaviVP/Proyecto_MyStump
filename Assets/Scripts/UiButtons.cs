using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UiButtons : MonoBehaviour
{

    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        Time.timeScale = 1.0f;
    }
    public void PlayButton()
    {

        SceneManager.LoadScene(4);

    }
    public void SettingsButton()
    {

        SceneManager.LoadScene(2);

    }
    public void CreditsButon()
    {

        SceneManager.LoadScene(3);

    }
    public void ReturnMenuButton()
    {

        SceneManager.LoadScene(1);

    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0.0f;

    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1.0f;

    }
}
