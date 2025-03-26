using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NameSelector : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField inputField1;
    [SerializeField] private GameObject inputPanel;

    [Header("Textos Jugador")]
    [SerializeField] private TextMeshProUGUI text1;

    [Header("Botones")]
    [SerializeField] private Button changeNameButton1;

    [Header("Mensajes")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float messageDuration = 2f;

    private const string NameListKey = "AllPlayerNames";
    private const int MaxNameLength = 12;
    private readonly string[] forbiddenCharacters = { "@", "#", "$", "%", "&", "*", "!", "?" };

    private int partidasGanadas;
    private int partidasTotales;
    private int tilesHormigas;
    private int tilesTotalesHormigas;
    private int tilesTermitas;
    private int tilesTotalesTermitas;
    public PlayerStats playerStats;
    void Start()
    {
        playerStats = new PlayerStats();
        LoadNames();

        changeNameButton1.onClick.AddListener(() => AttemptNameChange(1));
        inputField1.onEndEdit.AddListener(delegate { AttemptNameChange(1); });
    }

    private void Update()
    {
        EnterKeyboard();
    }

    private void LoadNames()
    {
        if (PlayerPrefs.HasKey(NameListKey))
        {
            string savedNames = PlayerPrefs.GetString(NameListKey);
            List<string> allNames = new List<string>(savedNames.Split(';'));

            if (allNames.Count > 0)
            {
                text1.text = allNames[0];
                inputField1.text = allNames[0];
            }
        }
    }

    private void AttemptNameChange(int playerNumber)
    {
        string newName = inputField1.text;

        if (!IsNameValid(newName))
        {
            ShowMessage("Nombre inválido: revisa la longitud o caracteres.");
            return;
        }

        List<string> allNames = GetAllSavedNames();

        if (!allNames.Contains(newName))
        {
            allNames.Add(newName);
            SaveAllNames(allNames);
        }

        PlayerPrefs.SetString("PlayerName1", newName);
        text1.text = newName;
        partidasGanadas++;
        partidasTotales = PlayerPrefs.GetInt($"PartidasGanadas_{newName}");
        partidasTotales += partidasGanadas;
        PlayerPrefs.SetInt($"PartidasGanadas_{newName}", partidasTotales);

        if (GameManager.Instance.Winner() == "Ants")
        {
            tilesHormigas = GameManager.Instance.NumberOfAntTiles();
            tilesTotalesHormigas = PlayerPrefs.GetInt($"ParcelasHormigas_{newName}");
            tilesTotalesHormigas += tilesHormigas;
            PlayerPrefs.SetInt($"ParcelasHormigas_{newName}",tilesTotalesHormigas);
        }
        else if (GameManager.Instance.Winner() == "Termites")
        {
            tilesTermitas = GameManager.Instance.NumberOfTermTiles();
            tilesTotalesTermitas = PlayerPrefs.GetInt($"ParcelasTermitas_{newName}");
            tilesTotalesTermitas += tilesTermitas;
            PlayerPrefs.SetInt($"ParcelasTermitas_{newName}", tilesTotalesTermitas);
        }
        else 
        { 
        }
        Debug.Log(PlayerPrefs.GetInt($"PartidasGanadas_{newName}"));
        Debug.Log(PlayerPrefs.GetInt($"ParcelasHormigas_{newName}"));
        Debug.Log(PlayerPrefs.GetInt($"ParcelasTermitas_{newName}"));
        ShowMessage("¡Nombre asignado!");
        inputPanel.SetActive(false);
        text1.enabled = true;
        StartCoroutine(CloseKeyboard());
        
    }

    private bool IsNameValid(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > MaxNameLength)
            return false;

        foreach (string c in forbiddenCharacters)
        {
            if (name.Contains(c))
                return false;
        }

        return true;
    }

    private void ShowMessage(string message)
    {
        StopAllCoroutines();
        messageText.text = message;
        messageText.alpha = 1;
        StartCoroutine(FadeOutMessage());
    }

    private IEnumerator FadeOutMessage()
    {
        yield return new WaitForSeconds(messageDuration);

        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            messageText.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            yield return null;
        }

        messageText.alpha = 0;
    }

    private List<string> GetAllSavedNames()
    {
        string savedNames = PlayerPrefs.GetString(NameListKey, "");
        if (string.IsNullOrEmpty(savedNames))
            return new List<string>();

        return new List<string>(savedNames.Split(';'));
    }

    private void SaveAllNames(List<string> allNames)
    {
        string joinedNames = string.Join(";", allNames);
        PlayerPrefs.SetString(NameListKey, joinedNames);
        PlayerPrefs.Save();
    }

    public void ClearAllNames()
    {
        PlayerPrefs.DeleteKey(NameListKey);
        text1.text = "";
        inputField1.text = "";
        ShowMessage("Nombres borrados.");
    }

    public void EnterKeyboard()
    {
        if ((Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) && inputField1.isFocused)
        {
            AttemptNameChange(1);
        }
    }

    private IEnumerator CloseKeyboard()
    {
        yield return new WaitForSeconds(0.1f);
        inputField1.DeactivateInputField();
    }
}