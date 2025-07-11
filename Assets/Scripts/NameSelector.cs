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
    [SerializeField] private float messageDuration = 1f;

    private const string NameListKey = "AllPlayerNames";
    private const int MaxNameLength = 12;
    private readonly string[] forbiddenCharacters = { "@", "#", "$", "%", "&", "*", "!", "?" };

    private int partidasGanadas;
    private int partidasTotales;
    private int tilesHormigas;
    private int tilesTotalesHormigas;
    private int tilesTermitas;
    private int tilesTotalesTermitas;

    
    void Start()
    {
    
        LoadNames();
        inputField1.text = "";
        

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
            PlayerPrefs.SetInt($"TermitasEliminadas_{newName}", PlayerPrefs.GetInt("TermsKilled") );

        }
        else if (GameManager.Instance.Winner() == "Termites")
        {
            tilesTermitas = GameManager.Instance.NumberOfTermTiles();
            tilesTotalesTermitas = PlayerPrefs.GetInt($"ParcelasTermitas_{newName}");
            tilesTotalesTermitas += tilesTermitas;
            PlayerPrefs.SetInt($"ParcelasTermitas_{newName}", tilesTotalesTermitas);
            PlayerPrefs.SetInt($"HormigasEliminadas_{newName}", PlayerPrefs.GetInt("AntsKilled"));
        }
        else 
        { 
        }
        Debug.Log(PlayerPrefs.GetInt($"PartidasGanadas_{newName}"));
        Debug.Log(PlayerPrefs.GetInt($"ParcelasHormigas_{newName}"));
        Debug.Log(PlayerPrefs.GetInt($"ParcelasTermitas_{newName}"));
        Debug.Log(PlayerPrefs.GetInt($"TermitasEliminadas_{newName}"));
       
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