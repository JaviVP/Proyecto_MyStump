using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using static GameManager;

[CreateAssetMenu(fileName = "NewHazard", menuName = "Hazard")]
public class Hazard : ScriptableObject
{
    [Header("Content Data")]
    public Sprite eventBackground;
    public Sprite eventMainImage;
    public string eventName;
    public string description;
    public string lore;

    [Header("General Settings")]
    public GameObject extraPropPrefab;
    public Material tileToChangeMaterial;
    public Volume posprocessing;
    public GameObject tileChangeVFX;
    public GameObject ScreenVFX;
    public int duration;

    private HexGrid hexGrid;



    public enum HazardEffect
    {
        TransformToNeutral,
        ChangeToOppositeTeam,
        DestroyOrTransformIntoObstacle,
        ChangeToLosingTeam
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

    [System.Serializable]
    public class TierData
    {
        public int amountOfTiles;
        
    }

    [Header("Effect Settings")]
    public HazardEffect effect;
    public AreaAffected area;
    public WhoIsAffected whoIsAffected;

    [Header("Tile Targeting Restrictions")]
    public bool affectNeutralTiles = true;
    public bool affectAntTiles = true;
    public bool affectTermiteTiles = true;

    [Header("Tier Settings")]
    public TierData tier1;
    public TierData tier2;
    public TierData tier3;

    public void ExecuteHazard(bool usingTiers, int tier)
    {
        hexGrid = FindAnyObjectByType<HexGrid>();

        TierData selectedTier = tier2;
        if (usingTiers)
        {
            switch (tier)
            {
                case 1: selectedTier = tier1; break;
                case 2: selectedTier = tier2; break;
                case 3: selectedTier = tier3; break;
                default: Debug.LogWarning("Invalid tier, using Tier 2."); break;
            }
        }

        Debug.Log($"[Hazard] Executing {eventName} | Tier {tier} | Affecting {selectedTier.amountOfTiles} tiles");

        List<HexTile> candidateTiles = GetAffectedTiles(area, whoIsAffected);
        candidateTiles.Shuffle();

        int applied = 0;
        foreach (HexTile tile in candidateTiles)
        {
            if (applied >= selectedTier.amountOfTiles) break;

            if (!IsTileAllowedForEffect(tile)) continue;

            ApplyEffectToTile(tile);
            applied++;
        }

        if (applied < selectedTier.amountOfTiles)
            Debug.Log($"Hazard '{eventName}' applied to {applied} out of {selectedTier.amountOfTiles} requested tiles.");
    }

    private List<HexTile> GetAffectedTiles(AreaAffected areaType, WhoIsAffected targeting)
    {
        List<HexTile> result = new List<HexTile>();
        Vector2Int center = new Vector2Int(0, 0);

        List<HexTile> candidateTiles = new List<HexTile>();

        switch (areaType)
        {
            case AreaAffected.Interior:
                candidateTiles.AddRange(hexGrid.GetTilesWithinRange(center, 2));
                break;

            case AreaAffected.Exterior:
                var interior = new List<HexTile>();
                interior.AddRange(hexGrid.GetTilesWithinRange(center, 2));
                foreach (var tile in hexGrid.GetAllHexTiles())
                    if (!interior.Contains(tile))
                        candidateTiles.Add(tile);
                break;

            case AreaAffected.Random:
                AreaAffected randomArea = (Random.value > 0.5f) ? AreaAffected.Interior : AreaAffected.Exterior;
                return GetAffectedTiles(randomArea, targeting);

            case AreaAffected.Both:
                candidateTiles.AddRange(hexGrid.GetAllHexTiles());
                break;
        }

        foreach (HexTile tile in candidateTiles)
        {
            if (hexGrid.GetUnitInTile(tile.axialCoords) != null)
                continue;

            var neighbors = hexGrid.GetTilesWithinRange(tile.axialCoords, 1);
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

        // Apply WhoIsAffected filtering  
        switch (targeting)
        {
            case WhoIsAffected.MostTilesOwner:
                Team mostOwner = hexGrid.GetTeamWithMostTiles();
                result.RemoveAll(t => HexGrid.EnumHelper.ConvertToTeam(t.state) != mostOwner);
                break;

            case WhoIsAffected.LeastTilesOwner:
                Team leastOwner = hexGrid.GetTeamWithLeastTiles();
                result.RemoveAll(t => HexGrid.EnumHelper.ConvertToTeam(t.state) != leastOwner);
                break;

            case WhoIsAffected.Random:
                break;
        }

        return result;
    }

    private void ApplyEffectToTile(HexTile tile)
    {
        switch (effect)
        {
            case HazardEffect.TransformToNeutral:
                if (tile.state != HexState.Neutral)
                    tile.SetState(HexState.Neutral);
                break;

            case HazardEffect.ChangeToOppositeTeam:
                var currentTeam = HexGrid.EnumHelper.ConvertToTeam(tile.state);
                if (currentTeam.HasValue)
                {
                    HexState newState = (currentTeam == Team.Ants) ? HexState.Termites : HexState.Ants;
                    tile.SetState(newState);
                }
                break;

            case HazardEffect.DestroyOrTransformIntoObstacle:
                if (duration == 0)
                {
                    hexGrid.RemoveTile(tile.axialCoords, tileToChangeMaterial);
                }
                else
                {
                    hexGrid.TemporaryInactiveTiles.Add(tile);
                    hexGrid.RemoveTile(tile.axialCoords, tileToChangeMaterial);
                }
                break;

            case HazardEffect.ChangeToLosingTeam:
                Team losingTeam = hexGrid.GetTeamWithLeastTiles();
                if (HexGrid.EnumHelper.ConvertToTeam(tile.state) != losingTeam)
                {
                    tile.SetState(HexGrid.EnumHelper.ConvertToHexState(losingTeam));
                }
                break;
        }

        if (tileChangeVFX)
            Instantiate(tileChangeVFX, tile.transform.position, Quaternion.identity);

        if (ScreenVFX)
            Instantiate(ScreenVFX, Vector3.zero, Quaternion.identity);

        if (extraPropPrefab)
            Instantiate(extraPropPrefab, tile.transform.position, Quaternion.identity);
    }

    private bool IsTileAllowedForEffect(HexTile tile)
    {
        switch (tile.state)
        {
            case HexState.Neutral: return affectNeutralTiles;
            case HexState.Ants: return affectAntTiles;
            case HexState.Termites: return affectTermiteTiles;
            default: return false;
        }
    }

    public void LaunchUIHazard()
    {

    }

}
