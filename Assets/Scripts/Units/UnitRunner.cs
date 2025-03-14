using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static HexGrid;

public class UnitRunner : Unit
{
    private HexGrid hexGrid;
    private HashSet<HexTile> validMoveTiles = new HashSet<HexTile>(); // Store valid move tiles


    private void Start()
    {
        hexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
    }


    public override void OnSelected()
    {
        validMoveTiles.Clear(); // Clear previous selections
        Vector2Int[] directions = {
        new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1),
        new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, -1)
    };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int currentPos = AxialCoords;
            HexTile lastValidTile = null;

            for (int i = 0; i < 3; i++) // ✅ Max range of 3
            {
                currentPos += direction;
                //if (!hexGrid.HasTile(currentPos)) break; // ✅ Out of grid = obstacle

                HexTile tile = hexGrid.GetHexTile(currentPos);
                if (tile == null) break;

                Team? tileTeam = EnumHelper.ConvertToTeam(tile.state);
                bool isOccupied = hexGrid.GetUnitInTile(tile.axialCoords) != null;

                if (tile.state == HexState.Neutral) // ✅ Can pass through neutral
                {
                    lastValidTile = tile;
                }
                else if (tileTeam.HasValue && tileTeam.Value == this.Team && !isOccupied) // ✅ Can pass through team-owned tiles
                {
                    lastValidTile = tile;
                }
                else // ❌ Obstacle (enemy tile or occupied)
                {
                    break;
                }
            }

            if (lastValidTile != null)
            {
                validMoveTiles.Add(lastValidTile); // ✅ Add only the last valid tile
            }
        }

        foreach (HexTile tile in validMoveTiles)
        {
            tile.HighlightTile(); // ✅ Highlight final movement options
        }
    }


    public override void ClearHighlights()
    {
        foreach (HexTile tile in validMoveTiles)
        {
            tile.ResetTileColor();
        }
        validMoveTiles.Clear();
    }


    public override bool Move(Vector2Int targetPosition)
    {
        throw new System.NotImplementedException();
    }

    
}
