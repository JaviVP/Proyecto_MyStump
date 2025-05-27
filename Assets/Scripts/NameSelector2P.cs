using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;

public class NameSelector2P : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField inputField1;
    [SerializeField] private TMP_InputField inputField2;
 

    [Header("Textos Jugador")]
    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI text2;

    [Header("Botones")]
    [SerializeField] private Button changeNameButton1;
    [SerializeField] private Button changeNameButton2;

    [Header("Mensajes")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private float messageDuration = 2f;

    [SerializeField] private LocalizedString invalidNameMessage;
    [SerializeField] private LocalizedString namesCannotBeEqualMessage;
    [SerializeField] private LocalizedString nameSavedMessage;

    private const string NameListKey = "AllPlayerNames";
    private const int MaxNameLength = 12;
    private readonly string[] forbiddenCharacters = { "@", "#", "$", "%", "&", "*", "!", "?", "+", "=", "/", "\\", "|", "<", ">", "(", ")", "{", "}", "[", "]", "^", "~", "`", "'", "\"", ":", ";", ",", "." };

    private void Start()
    {
        LoadPlayerStats(); //Cargar estadisticas en consola
        LoadNames();
        inputField1.text = "";
        inputField2.text = "";
        text1.text = "";
        text2.text = "";
        // Asigna funciones a los botones
        changeNameButton1.onClick.AddListener(() => AttemptNameChange(1));
        changeNameButton2.onClick.AddListener(() => AttemptNameChange(2));

        // Permitir confirmar con "Enter"
        inputField1.onEndEdit.AddListener(delegate { AttemptNameChange(1); });
        inputField2.onEndEdit.AddListener(delegate { AttemptNameChange(2); });
    }

    private void LoadNames()
    {

        if (PlayerPrefs.HasKey("PlayerName1"))
        {
            text1.text = PlayerPrefs.GetString("PlayerName1");
            inputField1.text = text1.text;
        }

        if (PlayerPrefs.HasKey("PlayerName2"))
        {
            text2.text = PlayerPrefs.GetString("PlayerName2");
            inputField2.text = text2.text;
        }

    }

    private void AttemptNameChange(int playerNumber)
    {
        string newName = (playerNumber == 1) ? inputField1.text.Trim() : inputField2.text.Trim();

        if (!IsNameValid(newName))
        {
            invalidNameMessage.Arguments = new object[] { playerNumber };
            invalidNameMessage.GetLocalizedStringAsync().Completed += (handle) => {
                ShowMessage(handle.Result);
            };
            return;
        }

        string otherPlayerName = (playerNumber == 1) ? inputField2.text.Trim() : inputField1.text.Trim();

        if (newName == otherPlayerName)
        {
            namesCannotBeEqualMessage.GetLocalizedStringAsync().Completed += (handle) => {
                ShowMessage(handle.Result);
            };
            return;
        }

        List<string> allNames = GetAllSavedNames();
        if (!allNames.Contains(newName)) allNames.Add(newName);
        SaveAllNames(allNames);

        string playerKey = (playerNumber == 1) ? "PlayerName1" : "PlayerName2";
        PlayerPrefs.SetString(playerKey, newName);
        PlayerPrefs.Save();

        if (playerNumber == 1)
            text1.text = newName;
        else
            text2.text = newName;

        nameSavedMessage.Arguments = new object[] { playerNumber };
        nameSavedMessage.GetLocalizedStringAsync().Completed += (handle) => {
            ShowMessage(handle.Result);
        };
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
        return string.IsNullOrEmpty(savedNames) ? new List<string>() : new List<string>(savedNames.Split(';'));
    }

    private void SaveAllNames(List<string> allNames)
    {
        PlayerPrefs.SetString(NameListKey, string.Join(";", allNames));
    }
    
    private void LoadPlayerStats()
    {
        List<string> playerNames = GetAllSavedNames();

        foreach (string playerName in playerNames)
        {
            Debug.Log("Jugador guardado: " + playerName);  // Mostrar cada nombre guardado
        }

        List<PlayerStats> allStats = new List<PlayerStats>();

        foreach (string playerName in playerNames)
        {
            try
            {
                PlayerStats stats = LoadStatsForPlayer(playerName);
                allStats.Add(stats); // Guardamos las estadísticas del jugador
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading stats for player {playerName}: {e.Message}");
            }
        }
        for (int i = 0; i < allStats.Count; i++)
        {
            PlayerStats stats = allStats[i];
            int rank = i + 1; // Top 1, Top 2...

          

           

            // Ahora incluye el número de ranking
           SetupEntry(rank, stats.Nombre, stats.PartidasGanadas, stats.HormigasEliminadas, stats.TermitasEliminadas, stats.ParcelasHormigas, stats.ParcelasTermitas);
        }


    }
    private PlayerStats LoadStatsForPlayer(string name)
    {
        int partidasGanadas = PlayerPrefs.GetInt($"PartidasGanadas_{name}");
        int hormigasEliminadas = PlayerPrefs.GetInt($"HormigasEliminadas_{name}");
        int termitasEliminadas = PlayerPrefs.GetInt($"TermitasEliminadas_{name}");
        int parcelasHormigas = PlayerPrefs.GetInt($"ParcelasHormigas_{name}");
        int parcelasTermitas = PlayerPrefs.GetInt($"ParcelasTermitas_{name}");

        return new PlayerStats
        {
            Nombre = name,
            PartidasGanadas = partidasGanadas,
            HormigasEliminadas = hormigasEliminadas,
            TermitasEliminadas = termitasEliminadas,
            ParcelasHormigas = parcelasHormigas,
            ParcelasTermitas = parcelasTermitas
        };
    }

    private void SetupEntry(int rank, string playerName, int partidasGanadas, int hormigasEliminadas, int termitasEliminadas, int parcelasHormigas, int parcelasTermitas)
    {
        string statsMessage = $"{rank}. {playerName} - " +
                              $"Partidas: {partidasGanadas}, " +
                              $"Hormigas eliminadas: {hormigasEliminadas}, " +
                              $"Termitas eliminadas: {termitasEliminadas}, " +
                              $"Parcelas Hormigas: {parcelasHormigas}, " +
                              $"Parcelas Termitas: {parcelasTermitas}";

        Debug.Log(statsMessage);
    }

}