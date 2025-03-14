﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static HexGrid;

public class UnitTerraFormer : Unit
{
    private HexGrid hexGrid;
    private HashSet<HexTile> validMoveTiles = new HashSet<HexTile>(); // Store valid move tiles


    private void Start()
    {
        hexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
    }


    public override void OnSelected()
    {
        Queue<HexTile> tilesCheck = new Queue<HexTile>();
        validMoveTiles.Clear(); // Clear previous selections

        HexTile startTile = hexGrid.GetHexTile(AxialCoords);
        tilesCheck.Enqueue(startTile);

        int limit = 100;

        while (tilesCheck.Count > 0 && limit > 0)
        {
            limit--;
            HexTile currentTile = tilesCheck.Dequeue();
            List<HexTile> adjacentTiles = hexGrid.GetTilesWithinRange(currentTile.axialCoords, 1);

            foreach (HexTile tile in adjacentTiles)
            {
                Team? tileTeam = EnumHelper.ConvertToTeam(tile.state);

                // ✅ 1️⃣ Can move to adjacent unoccupied tiles (Neutral or Enemy)
                if ((tile.state == HexState.Neutral || (tileTeam.HasValue && tileTeam.Value != this.Team)) && limit == 99)
                {
                    if (!validMoveTiles.Contains(tile) && hexGrid.GetUnitInTile(tile.axialCoords) == null)
                    {
                        validMoveTiles.Add(tile);
                    }
                }

                // ✅ 2️⃣ Can move freely within **connected** tiles of its own team
                else if (tileTeam.HasValue && tileTeam.Value == this.Team && hexGrid.GetUnitInTile(tile.axialCoords) == null)
                {
                    if (!validMoveTiles.Contains(tile))
                    {
                        validMoveTiles.Add(tile);
                        tilesCheck.Enqueue(tile); // Continue checking connected tiles
                    }
                }
            }
        }

        // ✅ 3️⃣ Highlight valid move tiles
        foreach (HexTile tile in validMoveTiles)
        {
            tile.HighlightTile();
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
        HexTile targetTile = hexGrid.GetHexTile(targetPosition);

        // ✅ 1️⃣ Ensure target tile is valid for movement
        if (targetTile == null || !validMoveTiles.Contains(targetTile))
        {
            ClearHighlights();
            return false;
        }

        // ✅ 2️⃣ Move the unit to the new position
        hexGrid.UpdateUnitPosition(AxialCoords, targetPosition, this);
        AxialCoords = targetPosition;
        //transform.position = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);

        // ✅ 3️⃣ Convert **any stepped-on tile** to Terraformer's team
        targetTile.SetState(EnumHelper.ConvertToHexState(this.Team));

        // ✅ 4️⃣ Clear highlights after moving
        ClearHighlights();
        StartCoroutine(Animation(targetPosition));
        return true;
    }

    IEnumerator Animation(Vector2Int targetPos)
    {
        Vector3 endPos = hexGrid.AxialToWorld(targetPos.x,targetPos.y);
        float speed = 10.0f;
        float minDistance = 0.2f;
        transform.LookAt(endPos);

        while(true)
        {
            transform.position = Vector3.MoveTowards(transform.position, endPos, speed * Time.deltaTime);
            yield return new WaitForSeconds(0.05f);
            if (Vector3.Distance(transform.position, endPos) < 0.2f)
            {
                break;
            }
        }

        yield return new WaitForSeconds(2.0f);

    }

    
}
