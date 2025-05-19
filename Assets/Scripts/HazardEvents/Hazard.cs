using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static GameManager;

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
    public int duration;


    private HexGrid hexGrid;

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
    //public bool useTierSystem;

    public TierData tier1;
    public TierData tier2;
    public TierData tier3;





    public void ExecuteHazard(bool usingTiers, int tier)
    {
        // Choose TierData based on input
        TierData selectedTier = tier2; // default fallback

        if (usingTiers)
        {
            switch (tier)
            {
                case 1:
                    selectedTier = tier1;
                    break;
                case 2:
                    selectedTier = tier2;
                    break;
                case 3:
                    selectedTier = tier3;
                    break;
                default:
                    Debug.LogWarning("Invalid tier passed to ExecuteHazard(). Using Tier 2.");
                    selectedTier = tier2;
                    break;
            }
        }

        Debug.Log($"[Hazard] Executing: {eventName} | Tier {tier} | Affecting {selectedTier.amountOfTiles} tiles for {selectedTier.duration} seconds.");

        // ✅ Get affected tiles
        List<HexTile> affectedTiles = GetAffectedTiles(area);

        // ✅ Shuffle them so it's not always the same spots
        affectedTiles.Shuffle(); // extension method or you can implement a local shuffle

        // ✅ Apply to selected number of tiles
        int appliedCount = 0;
        foreach (HexTile tile in affectedTiles)
        {
            if (appliedCount >= selectedTier.amountOfTiles)
                break;

            ApplyEffectToTile(tile);
            appliedCount++;
        }
    }





    private List<HexTile> GetAffectedTiles(AreaAffected areaType)
    {
        List<HexTile> result = new List<HexTile>();

        Vector2Int center = new Vector2Int(0, 0); // or pass this dynamically if needed

        List<HexTile> candidateTiles = new List<HexTile>();

        switch (areaType)
        {
            case AreaAffected.Interior:
                candidateTiles.Add(hexGrid.GetHexTile(center));
                candidateTiles.AddRange(hexGrid.GetTilesWithinRange(center, 1));
                break;

            case AreaAffected.Exterior:
                List<HexTile> interior = new List<HexTile>();
                interior.Add(hexGrid.GetHexTile(center));
                interior.AddRange(hexGrid.GetTilesWithinRange(center, 1));

                foreach (var tile in hexGrid.GetAllHexTiles())
                {
                    if (!interior.Contains(tile))
                        candidateTiles.Add(tile);
                }
                break;

            case AreaAffected.Random:
                AreaAffected randomChoice = (Random.value > 0.5f) ? AreaAffected.Interior : AreaAffected.Exterior;
                candidateTiles = GetAffectedTiles(randomChoice);
                break;

            case AreaAffected.Both:
                candidateTiles.AddRange(hexGrid.GetAllHexTiles());
                break;
        }

        //  Filter out occupied tiles or those adjacent to an occupied tile
        foreach (HexTile tile in candidateTiles)
        {
            bool isOccupied = hexGrid.GetUnitInTile(tile.axialCoords) != null;

            if (isOccupied)
                continue;

            List<HexTile> neighbors = hexGrid.GetTilesWithinRange(tile.axialCoords, 1);
            bool neighborOccupied = false;

            foreach (HexTile neighbor in neighbors)
            {
                if (hexGrid.GetUnitInTile(neighbor.axialCoords) != null)
                {
                    neighborOccupied = true;
                    break;
                }
            }

            if (!neighborOccupied)
                result.Add(tile);
        }

        return result;
    }


    private void ApplyEffectToTile(HexTile tile)
    {
        switch (effect)
        {
            case TierEffect.TransformToNeutral:
                tile.SetState(HexState.Neutral);
                break;

            case TierEffect.ChangeToOppositeTeam:
                var currentTeam = HexGrid.EnumHelper.ConvertToTeam(tile.state);
                if (currentTeam.HasValue)
                {
                    HexState newState = (currentTeam == Team.Ants) ? HexState.Termites : HexState.Ants;
                    tile.SetState(newState);
                }
                break;

            case TierEffect.DestroyObstacle:
                // If you have a way to mark obstacles — e.g. state == Obstacle or something
                break;
        }

        // Optional: VFX / Feedback  
        if (tileChangeVFX)
            Instantiate(tileChangeVFX, tile.transform.position, Quaternion.identity);
    }



}
