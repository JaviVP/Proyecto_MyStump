using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UiButtons : MonoBehaviour
{

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject wonText;
    [SerializeField] private GameObject resetText;
    private bool touchEnabled = true;

    public static UiButtons Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
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

        SceneManager.LoadScene(3);

    }

    public void ReturnMenuButton()
    {

        SceneManager.LoadScene(1);

    }

    public void LoadHub()
    {

        SceneManager.LoadScene(1);

    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        Time.timeScale = 0.0f;
        touchEnabled = false;
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1.0f;
        touchEnabled = true;
    }


    public void GameFinish()
    {
        PlayerPrefs.SetInt("FINISHGAME", 0);
        PlayerPrefs.Save();
        wonText.SetActive(true);
        StartCoroutine(DeactivateTextAfterDelay(2f));
    }
    public void ResetGamePref()
    {
        PlayerPrefs.SetInt("FINISHGAME", 1);
        PlayerPrefs.Save();
        resetText.SetActive(true);
        StartCoroutine(DeactivateTextAfterDelay(2f));
    }


    private IEnumerator DeactivateTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        wonText.SetActive(false);
        resetText.SetActive(false);
    }

    public bool TouchesEnabled()
    {
        return touchEnabled;
    }
}
