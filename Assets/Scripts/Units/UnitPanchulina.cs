using System.Collections.Generic;
using UnityEngine;
using static HexGrid;

public class UnitPanchulina : Unit
{
    private HexGrid hexGrid;
    private HashSet<HexTile> validMoveTiles = new HashSet<HexTile>(); // Store valid move tiles


    private void Start()
    {
        hexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
    }



    public override void OnSelected()
    {
        validMoveTiles.Clear();
        List<HexTile> firstMoveOptions = hexGrid.GetTilesWithinRange(AxialCoords, 1);

        foreach (HexTile tile in firstMoveOptions)
        {
            if (hexGrid.GetUnitInTile(tile.axialCoords) == null) // ✅ Ensure tile is empty
            {
                validMoveTiles.Add(tile);
                tile.HighlightTile();
            }
        }
    }



    public override bool Move(Vector2Int targetPosition)
    {
        HexTile targetTile = hexGrid.GetHexTile(targetPosition);
        if (targetTile == null || !validMoveTiles.Contains(targetTile)) return false; // ❌ Invalid move

        // ✅ First Move: Move normally
        hexGrid.UpdateUnitPosition(AxialCoords, targetPosition, this);
        AxialCoords = targetPosition;
        transform.position = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);

        // ✅ Second Move: Show new move options
        validMoveTiles.Clear();
        Vector2Int[] directions = {
        new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1),
        new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, -1)
    };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int secondMovePos = AxialCoords + direction;
            HexTile secondMoveTile = hexGrid.GetHexTile(secondMovePos);
            Unit enemyUnit = hexGrid.GetUnitInTile(secondMovePos);

            if (secondMoveTile != null && enemyUnit == null)
            {
                validMoveTiles.Add(secondMoveTile);
                secondMoveTile.HighlightTile();
            }
            else if (enemyUnit != null && EnumHelper.ConvertToTeam(secondMoveTile.state) != this.Team)
            {
                Vector2Int pushPos = secondMovePos + direction;
                HexTile pushTile = hexGrid.GetHexTile(pushPos);

                if (pushTile != null && hexGrid.GetUnitInTile(pushPos) == null)
                {
                    // ✅ Push enemy forward
                    hexGrid.UpdateUnitPosition(secondMovePos, pushPos, enemyUnit);
                    enemyUnit.AxialCoords = pushPos;
                    enemyUnit.transform.position = hexGrid.AxialToWorld(pushPos.x, pushPos.y);

                    // ✅ Convert pushed tiles to Panchulinas’ team
                    secondMoveTile.SetState(EnumHelper.ConvertToHexState(this.Team));
                    pushTile.SetState(EnumHelper.ConvertToHexState(this.Team));

                    // ✅ Move Panchulinas into enemy's original position
                    hexGrid.UpdateUnitPosition(AxialCoords, secondMovePos, this);
                    AxialCoords = secondMovePos;
                    transform.position = hexGrid.AxialToWorld(secondMovePos.x, secondMovePos.y);
                    break;
                }
            }
        }

        ClearHighlights(); // ✅ Remove highlights after movement
        return true;
    }



    public override void ClearHighlights()
    {
        foreach (HexTile tile in validMoveTiles)
        {
            tile.ResetTileColor();
        }
        validMoveTiles.Clear();
    }


}
