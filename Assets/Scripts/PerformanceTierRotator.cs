using UnityEngine;
using TMPro;

public class PerformanceTierRotator : MonoBehaviour
{
    public TMP_Text buttonText;

    void Start()
    {
        UpdateButtonText();
    }

    public void OnClickRotateTier()
    {
        if (PerformanceTierManager.Instance == null) return;

        int next = ((int)PerformanceTierManager.CurrentTier + 1) % System.Enum.GetValues(typeof(PerformanceTier)).Length;
        PerformanceTier nextTier = (PerformanceTier)next;

        PerformanceTierManager.Instance.SetTierManual(nextTier);
        UpdateButtonText();
    }

    void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = $"Calidad: {PerformanceTierManager.CurrentTier}";
        }
    }
}
