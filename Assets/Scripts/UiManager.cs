using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject wonText;
    [SerializeField] private GameObject resetText;
    [SerializeField] private GameObject turnUI;
    [SerializeField] private GameObject antHighlight;
    [SerializeField] private GameObject termHighlight;
    [SerializeField] private TextMeshProUGUI antTiles;
    [SerializeField] private TextMeshProUGUI termTiles;
    [SerializeField] private GameObject turnScroll;

    private bool touchEnabled = true;

    private Button[] buttons;
    private Slider scrollSlider;
    public static UiManager Instance { get; private set; }
    public bool TouchEnabled { get => touchEnabled; set => touchEnabled = value; }

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
        scrollSlider = turnScroll.GetComponent<Slider>();
        Time.timeScale = 1.0f;
        TouchEnabled = true;
        // Corrección de método obsoleto
        buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            // Verificar si el botón ya tiene el script, si no, añadirlo
            if (button.GetComponent<ButtonPressEffect>() == null)
            {
                button.gameObject.AddComponent<ButtonPressEffect>();
            }
        }
    }

    public void UpdateScroll()
    {
        
        scrollSlider.value = GameManager.Instance.LimitTurns;
    }

    public void PlayButton()
    {
        SceneManager.LoadScene(4);
    }

    public void ChampionshipButton()
    {
        //SceneManager.LoadScene(5);
    }

    public void TutorialScene()
    {
        SceneManager.LoadScene(5);
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
        TouchEnabled = false;
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        Time.timeScale = 1.0f;
        TouchEnabled = true;
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
        return TouchEnabled;
    }

    public void UpdateTiles()
    {
        antTiles.text = GameManager.Instance.NumberOfAntTiles().ToString();
        termTiles.text = GameManager.Instance.NumberOfTermTiles().ToString();

        if (GameManager.Instance.CurrentTurn == GameManager.Team.Ants)
        {
            antHighlight.GetComponent<Image>().enabled = true;
            termHighlight.GetComponent<Image>().enabled = false;
        }
        else
        {
            antHighlight.GetComponent<Image>().enabled = false;
            termHighlight.GetComponent<Image>().enabled = true;
        }
    }

    public void UpdateUiTurn(string content)
    {
        turnUI.GetComponent<TMP_Text>().text = content;
    }

}
