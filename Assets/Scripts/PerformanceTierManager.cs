using UnityEngine;
public enum PerformanceTier
{
    UltraLow,
    Low,
    Medium,
    High
}
public class PerformanceTierManager : MonoBehaviour
{
    public static PerformanceTierManager Instance;
    public static PerformanceTier CurrentTier = PerformanceTier.Medium;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Cargar tier guardado
        int savedTier = PlayerPrefs.GetInt("PerformanceTier", -1);
        if (savedTier >= 0 && savedTier < System.Enum.GetValues(typeof(PerformanceTier)).Length)
        {
            CurrentTier = (PerformanceTier)savedTier;
        }

        ApplyPerformanceSettings();
    }

    public void SetTierManual(PerformanceTier newTier)
    {
        Debug.Log($"Cambiando a tier: {newTier}");
        CurrentTier = newTier;
        PlayerPrefs.SetInt("PerformanceTier", (int)newTier);
        PlayerPrefs.Save();
        ApplyPerformanceSettings();
    }

    void ApplyPerformanceSettings()
    {
        int tier = 0;

        switch (CurrentTier)
        {
            case PerformanceTier.UltraLow:
                tier = 0;
                Application.targetFrameRate = 30;
                QualitySettings.shadowDistance = 0;
                QualitySettings.pixelLightCount = 0;
                QualitySettings.globalTextureMipmapLimit = 2; // Baja resolución
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                break;

            case PerformanceTier.Low:
                tier = 1;
                Application.targetFrameRate = 45;
                QualitySettings.shadowDistance = 15;
                QualitySettings.pixelLightCount = 1;
                QualitySettings.globalTextureMipmapLimit = 1; // Media resolución
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                break;

            case PerformanceTier.Medium:
                tier = 2;
                Application.targetFrameRate = 60;
                QualitySettings.shadowDistance = 30;
                QualitySettings.pixelLightCount = 2;
                QualitySettings.globalTextureMipmapLimit = 0; // Alta resolución
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                break;

            case PerformanceTier.High:
                tier = 3;
                Application.targetFrameRate = 60;
                QualitySettings.shadowDistance = 50;
                QualitySettings.pixelLightCount = 4;
                QualitySettings.globalTextureMipmapLimit = 0;
                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                break;
        }

        QualitySettings.SetQualityLevel(tier, true);
    }
}