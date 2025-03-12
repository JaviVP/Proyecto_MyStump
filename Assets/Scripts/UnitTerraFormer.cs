using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class UnitTerraFormer : Unit
{
    private HexGrid hexGrid;
    public override void Move(Vector2Int targetPosition)
    {
        throw new System.NotImplementedException();
    }

    public override void OnSelected()
    {
        Queue<HexTile> tilesCheck = new Queue<HexTile>();
        HashSet<HexTile> tilesSelected = new HashSet<HexTile>();

        tilesCheck.Enqueue(hexGrid.GetHexTile(AxialCoords));
        int limit = 100;

        while (tilesCheck.Count > 0 && limit-- > 0)
        {
            HexTile currentTile = tilesCheck.Dequeue();
            List<HexTile> adjacentTiles = hexGrid.GetTilesWithinRange(currentTile.axialCoords, 1);

            foreach (HexTile tile in adjacentTiles)
            {
                if (tile.state == HexState.Neutral && tilesSelected.Count == 0)
                {
                    tilesSelected.Add(tile);
                }
                else if (tile.state == HexState.Termites && hexGrid.GetUnitInTile(tile.axialCoords) == null)
                {
                    if (!tilesSelected.Contains(tile))
                    {
                        tilesSelected.Add(tile);
                        tilesCheck.Enqueue(tile);
                    }
                }
            }
        }

        foreach (HexTile tile in tilesSelected)
        {
            tile.HighlightTile();
        }
    }




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
    }

}
