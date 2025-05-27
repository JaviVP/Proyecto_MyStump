using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Components;
using System.Collections;
using System.Collections.Generic;

public class LanguageManager : MonoBehaviour
{
    // Ajustamos el código de Catalán a "ca-ES"
    private List<string> customOrder = new List<string> { "en", "es", "cat" };
    private int currentLanguageIndex = 0;
    private const string LANGUAGE_KEY = "selected_language";

    

    void Start()
    {
        StartCoroutine(InitializeLocalization());
    }

    IEnumerator InitializeLocalization()
    {
        yield return LocalizationSettings.InitializationOperation;

        if (PlayerPrefs.HasKey(LANGUAGE_KEY))
        {
            currentLanguageIndex = PlayerPrefs.GetInt(LANGUAGE_KEY);
        }
        else
        {
            currentLanguageIndex = customOrder.IndexOf("en"); // Forzar a que empiece en inglés
        }

        ApplyLanguage();
    }

    public void SwitchLanguage()
    {
        currentLanguageIndex = (currentLanguageIndex + 1) % customOrder.Count; // Cambia al siguiente idioma
        ApplyLanguage();
    }

    private void ApplyLanguage()
    {
        Locale newLocale = null;
        foreach (var locale in LocalizationSettings.AvailableLocales.Locales)
        {
            if (locale.Identifier.Code == customOrder[currentLanguageIndex]) // Ahora busca "ca-ES"
            {
                newLocale = locale;
                break;
            }
        }

        if (newLocale != null)
        {
            LocalizationSettings.SelectedLocale = newLocale;
            PlayerPrefs.SetInt(LANGUAGE_KEY, currentLanguageIndex);
            PlayerPrefs.Save();
            //Debug.Log("Idioma cambiado a: " + newLocale.LocaleName);
        }
        else
        {
            Debug.LogError("No se encontró el idioma: " + customOrder[currentLanguageIndex]);
        }
    }
}
