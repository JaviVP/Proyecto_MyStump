using System.Collections.Generic;
using UnityEngine;

public class UnitBase : Unit
{
    private bool abilityUsed = false;
    private bool abilityUnlocked = false;

    private HexGrid hexGrid;
    private HashSet<HexTile> validTargets = new HashSet<HexTile>();

    private void Start()
    {
        hexGrid = FindAnyObjectByType<HexGrid>();
    }


    public override void OnSelected()
    {
        validTargets.Clear();
        if (abilityUsed || !abilityUnlocked) return;

        // Highlight available targets for ability
        HighlightAbilityTargets();
    }

    public override bool Move(Vector2Int targetPosition)
    {
        HexTile targetTile = hexGrid.GetHexTile(targetPosition);
        if (abilityUsed || !abilityUnlocked) return false;

        if (targetTile == null || !validTargets.Contains(targetTile))
        {
            ClearHighlights();
            return false;
        }
        // Execute ability logic
        UseAbility();

        // Mark ability as used
        abilityUsed = true;
        ClearHighlights();
        return true;
    }

    public void CheckAbilityUnlock()
    {
        if (!abilityUnlocked && (hexGrid.GetCountStateTiles(HexState.Ants) + hexGrid.GetCountStateTiles(HexState.Termites)) >= 20)
        {
            abilityUnlocked = true;
            Debug.Log($"{Team} Base can now use its ability!");
        }
    }

    private void HighlightAbilityTargets()
    {
        // Highlight logic for ability usage
    }

    private void UseAbility()
    {
        Debug.Log($"{Team} Base activated its ability!");
        // Implement actual effect later
    }

    public override void ClearHighlights()
    {
        foreach (HexTile tile in validTargets)
        {
            tile.ResetTileColor();
        }
        validTargets.Clear();
    }

    public override bool CanMove()
    {
        return false;
    }
}
