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
    [SerializeField] private GameObject antHighlight;
    [SerializeField] private GameObject termHighlight;
    [SerializeField] private TextMeshProUGUI antTiles;
    [SerializeField] private TextMeshProUGUI termTiles;

    private bool touchEnabled = true;

    private Button[] buttons;
    public static UiManager Instance { get; private set; }
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

        buttons = FindObjectsOfType<Button>();

        foreach (Button button in buttons)
        {
            // Verificar si el botón ya tiene el script, si no, añadirlo
            if (button.GetComponent<ButtonPressEffect>() == null)
            {
                button.gameObject.AddComponent<ButtonPressEffect>();
            }
        }
    }
    private void Update()
    {
        UpdateTiles();

    }


    public void PlayButton()
    {

        SceneManager.LoadScene(4);

    }

    public void ChampionshipButton()
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

   

   private void UpdateTiles()
    {

        antTiles.text = GameManager.Instance.NumberOfAntTiles().ToString();
        termTiles.text = GameManager.Instance.NumberOfTermTiles().ToString();

        if(GameManager.Instance.CurrentTurn== GameManager.Team.Ants)
        {
            antHighlight.gameObject.GetComponent<Image>().enabled = true;
            termHighlight.gameObject.GetComponent<Image>().enabled = false;
        }
        else
        {
            antHighlight.gameObject.GetComponent<Image>().enabled = false; 
            termHighlight.gameObject.GetComponent<Image>().enabled = true; 

        }

    }

}
