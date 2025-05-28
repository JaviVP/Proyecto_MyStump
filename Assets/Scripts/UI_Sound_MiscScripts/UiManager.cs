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
    [SerializeField] private GameObject turnScrollGeneral;
    [SerializeField] private GameObject lockUi;
    [SerializeField] private TextMeshProUGUI turnText;


    private bool touchEnabled = true;

    private Button[] buttons;
    private Slider scrollSliderGeneral;
    private string player1;
    private string player2;
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
        player1 = PlayerPrefs.GetString("PlayerName1", "Jugador 1");
        player2 = PlayerPrefs.GetString("PlayerName2", "Jugador 2");
        if (turnScrollGeneral!=null && turnScrollGeneral.GetComponent<Slider>()) scrollSliderGeneral = turnScrollGeneral.GetComponent<Slider>();
        if (turnText!=null) turnText.text = scrollSliderGeneral.value.ToString();
        Time.timeScale = 1.0f;
        TouchEnabled = true;
        // Corrección de método obsoleto
        buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if (button.name == "Championship") continue; // Saltar este botón

            if (button.GetComponent<ButtonPressEffect>() == null)
            {
                button.gameObject.AddComponent<ButtonPressEffect>();
            }
        }
    }

    public void UpdateScroll()
    {
        if (GameManager.Instance.CurrentTurn == GameManager.Instance.CurrentTurn)
        {
            scrollSliderGeneral.value = GameManager.Instance.LimitTurns;
            turnText.text = scrollSliderGeneral.value.ToString();
        }
    }

    public void PlayButton()
    {
        PlayerPrefs.SetInt("ModoCampeonato", 0);
        SceneManager.LoadScene(3);
       
    }

    public void NameSelector()
    {
        SceneManager.LoadScene(6);
    }
    public void ChampionshipButton()
    {
        PlayerPrefs.SetInt("ModoCampeonato", 1);
        PlayerPrefs.SetInt($"RondasGanadas_{player1}", 0);
        PlayerPrefs.SetInt($"RondasGanadas_{player2}", 0);
        PlayerPrefs.SetInt($"ActualAntTiles_{player1}", 0);
        PlayerPrefs.SetInt($"ActualAntTiles_{player2}", 0);
        PlayerPrefs.SetInt($"ActualTermTiles_{player1}", 0);
        PlayerPrefs.SetInt($"ActualTermTiles_{player2}", 0);
        PlayerPrefs.DeleteKey("NumeroRondasCampeonato");
        int partidasSeleccionadas = PlayerPrefs.GetInt("NumeroPartidasCampeonato", 3); 
        Debug.Log("Número de partidas seleccionadas: " + partidasSeleccionadas);
        SceneManager.LoadScene(4);
      
    }
   
    public void ScoreBoardScene()
    {
        SceneManager.LoadScene(5);
    }
    public void TutorialScene()
    {
        PlayerPrefs.SetInt("ModoCampeonato", 0);
       // SceneManager.LoadScene(3);
    }

    public void SettingsButton()
    {
        SceneManager.LoadScene(2);
    }

    public void ReturnMenuButton()
    {
        PlayerPrefs.SetInt("ModoCampeonato", 0);
        SceneManager.LoadScene(1);
    }

    public void LoadHub()
    {
        
        PlayerPrefs.SetInt("ModoCampeonato", 0);
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
        
        //wonText.SetActive(true);
        //StartCoroutine(DeactivateTextAfterDelay(2f));
    }

    public void ResetGamePref()
    {
        PlayerPrefs.SetInt("FINISHGAME", 1);
        PlayerPrefs.Save();
        
        //resetText.SetActive(true);
        //StartCoroutine(DeactivateTextAfterDelay(2f));
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
