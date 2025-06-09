using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using System.Collections;

public class PerformanceTierRotator : MonoBehaviour
{
    public TMP_Text buttonText;
    public string tableName = "Change Language";  // Nombre exacto de tu tabla

    void Start()
    {
        StartCoroutine(UpdateButtonText());
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
    }

    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }

    void OnLocaleChanged(Locale locale)
    {
        StartCoroutine(UpdateButtonText());
    }

    public void OnClickRotateTier()
    {
        if (PerformanceTierManager.Instance == null) return;

        int next = ((int)PerformanceTierManager.CurrentTier + 1) % System.Enum.GetValues(typeof(PerformanceTier)).Length;
        PerformanceTier nextTier = (PerformanceTier)next;

        PerformanceTierManager.Instance.SetTierManual(nextTier);

        StartCoroutine(UpdateButtonText());
    }

    IEnumerator UpdateButtonText()
    {
        yield return LocalizationSettings.InitializationOperation;

        var stringTable = LocalizationSettings.StringDatabase.GetTable(tableName);
        if (stringTable == null)
        {
            Debug.LogError($"String table '{tableName}' no encontrada");
            yield break;
        }

        string key = "PerformanceTier." + PerformanceTierManager.CurrentTier.ToString();
        var entry = stringTable.GetEntry(key);

        if (entry == null)
        {
            Debug.LogWarning($"Clave '{key}' no encontrada en la tabla '{tableName}'");
            buttonText.text = PerformanceTierManager.CurrentTier.ToString();
            yield break;
        }

        buttonText.text = entry.GetLocalizedString();
    }
}