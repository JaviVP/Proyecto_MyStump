using System.Collections.Generic;
using UnityEngine;

public class UnitTerraFormer : Unit
{
    private HexGrid hexGrid;
    private HashSet<HexTile> validMoveTiles = new HashSet<HexTile>(); // Store valid move tiles

    public override void Move(Vector2Int targetPosition)
    {
        HexTile targetTile = hexGrid.GetHexTile(targetPosition);

        // ✅ 1️ Ensure target tile is valid for movement
        if (targetTile == null || !validMoveTiles.Contains(targetTile)) return;

        // ✅ 2️ Move the unit to the new position
        hexGrid.UpdateUnitPosition(AxialCoords, targetPosition, this);
        AxialCoords = targetPosition;
        transform.position = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);

        // ✅ 3️ Convert enemy tiles to Terraformer's team when stepping on them
        if (targetTile.state != HexState.Neutral && targetTile.state != this.Team)
        {
            targetTile.SetState(this.Team); // Instead of neutral, now converts to Terraformer’s team
        }

        // ✅ 4️ Clear highlights after moving
        ClearHighlights();
    }

    public override void OnSelected()
    {
        Queue<HexTile> tilesCheck = new Queue<HexTile>();
        validMoveTiles.Clear(); // Clear previous selections

        tilesCheck.Enqueue(hexGrid.GetHexTile(AxialCoords));
        int limit = 100;

        while (tilesCheck.Count > 0 && limit-- > 0)
        {
            HexTile currentTile = tilesCheck.Dequeue();
            List<HexTile> adjacentTiles = hexGrid.GetTilesWithinRange(currentTile.axialCoords, 1);

            foreach (HexTile tile in adjacentTiles)
            {
                if (tile.state == HexState.Neutral && validMoveTiles.Count == 0)
                {
                    validMoveTiles.Add(tile);
                }
                else if (tile.state == this.Team && hexGrid.GetUnitInTile(tile.axialCoords) == null)
                {
                    if (!validMoveTiles.Contains(tile))
                    {
                        validMoveTiles.Add(tile);
                        tilesCheck.Enqueue(tile);
                    }
                }
            }
        }

        foreach (HexTile tile in validMoveTiles)
        {
            tile.HighlightTile();
        }
    }

    private void ClearHighlights()
    {
        foreach (HexTile tile in validMoveTiles)
        {
            tile.ResetTileColor();
        }
        validMoveTiles.Clear();
    }

    private void Start()
    {
        hexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
    }
}
