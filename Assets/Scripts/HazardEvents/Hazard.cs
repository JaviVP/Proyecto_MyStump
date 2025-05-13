using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "NewHazard", menuName = "Hazard")]
public class Hazard : ScriptableObject
{
    [Header("General Settings")]
    // --- General Event Info ---
    public string eventName;
    public string description;
    public Sprite eventImage;
    public GameObject tileToChangePrefab;
    public Volume posprocessing;
    public GameObject tileChangeVFX;
    public GameObject ScreenVFX;

    // --- Enums ---
    public enum TierEffect
    {
        TransformToNeutral,
        ChangeToOppositeTeam,
        DestroyObstacle
    }

    public enum AreaAffected
    {
        Interior,
        Exterior,
        Random,
        Both
    }

    public enum WhoIsAffected
    {
        Random,
        MostTilesOwner,
        LeastTilesOwner
    }

    // --- Tier Data Class  ---
    [System.Serializable]
    public class TierData
    {
        [Tooltip("Amount of tiles affected for this tier")]
        public int amountOfTiles;

        [Tooltip("Effect duration in seconds for this tier")]
        public int duration;
    }

    // --- Effect and Area (Global settings for the hazard) ---
    [Header("Effect Settings")]
    public TierEffect effect;   // Global Effect for the Hazard
    public AreaAffected area;   // Global Area for the Hazard

    // --- Static Tiers (Fixed) ---
    [Header("Tier Settings (if false will use T2 data as Default)")]
    public bool useTierSystem;

    public TierData tier1;
    public TierData tier2;
    public TierData tier3;

    public void ExecuteHazard()
    {

    }
}
