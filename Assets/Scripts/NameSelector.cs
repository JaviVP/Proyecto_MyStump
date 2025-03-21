using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NameSelector : MonoBehaviour
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

    private const string NameListKey = "AllPlayerNames";  // Lista de todos los nombres
    private const int MaxNameLength = 12;
    private readonly string[] forbiddenCharacters = { "@", "#", "$", "%", "&", "*", "!", "?" };

    void Start()
    {
        LoadNames();

        changeNameButton1.onClick.AddListener(() => AttemptNameChange(1));
        changeNameButton2.onClick.AddListener(() => AttemptNameChange(2));
    }

    private void LoadNames()
    {
        if (PlayerPrefs.HasKey(NameListKey))
        {
            // Si hay una lista de nombres guardada, la cargamos y mostramos
            string savedNames = PlayerPrefs.GetString(NameListKey);
            List<string> allNames = new List<string>(savedNames.Split(';'));

            if (allNames.Count > 0)
            {
                text1.text = allNames[0];  // Mostrar el primer nombre en el text1
                inputField1.text = allNames[0];  // Establecer el primer nombre en el inputField1
            }

            if (allNames.Count > 1)
            {
                text2.text = allNames[1];  // Mostrar el segundo nombre en el text2
                inputField2.text = allNames[1];  // Establecer el segundo nombre en el inputField2
            }
        }
    }

    private void AttemptNameChange(int playerNumber)
    {
        string newName = playerNumber == 1 ? inputField1.text : inputField2.text;

        if (!IsNameValid(newName))
        {
            ShowMessage("Nombre inválido: revisa la longitud o caracteres.");
            return;
        }

        List<string> allNames = GetAllSavedNames();

        // Verificar si el nombre ya existe en la lista antes de agregarlo
        if (!allNames.Contains(newName))
        {
            // Añadir el nuevo nombre a la lista
            allNames.Add(newName);
            SaveAllNames(allNames);
        }

        // Guardar el nombre actual del jugador
        if (playerNumber == 1)
        {
            PlayerPrefs.SetString("PlayerName1", newName);
            text1.text = newName;
        }
        else
        {
            PlayerPrefs.SetString("PlayerName2", newName);
            text2.text = newName;
        }

        ShowMessage("¡Nombre asignado!");
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
        float startAlpha = messageText.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            messageText.alpha = Mathf.Lerp(startAlpha, 0, elapsed / fadeDuration);
            yield return null;
        }

        messageText.alpha = 0;
    }

    private List<string> GetAllSavedNames()
    {
        string savedNames = PlayerPrefs.GetString(NameListKey, "");
        if (string.IsNullOrEmpty(savedNames))
            return new List<string>();  // Si no hay nombres guardados, retorna una lista vacía

        return new List<string>(savedNames.Split(';'));
    }

    private void SaveAllNames(List<string> allNames)
    {
        string joinedNames = string.Join(";", allNames);
        PlayerPrefs.SetString(NameListKey, joinedNames);
        PlayerPrefs.Save();
    }

    // Para borrar todos los nombres guardados (opcional)
    public void ClearAllNames()
    {
        PlayerPrefs.DeleteKey(NameListKey);
        text1.text = "";
        text2.text = "";
        inputField1.text = "";
        inputField2.text = "";
        ShowMessage("Nombres borrados.");
    }
}