using System.Collections.Generic;
using UnityEngine;
using static HexGrid;

public class UnitPanchulina : Unit
{
    private HexGrid hexGrid;
    private HashSet<HexTile> validMoveTiles = new HashSet<HexTile>(); // Store valid move tiles
    private bool firstMove;

    public bool FirstMove { get => firstMove; set => firstMove = value; }

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
        if (!FirstMove)
        {
            HexTile targetTile = hexGrid.GetHexTile(targetPosition);
            if (targetTile == null || !validMoveTiles.Contains(targetTile))
            {
                ClearHighlights();
                return false; // ❌ Invalid move
            }

            // ✅ First Move: Move normally
            hexGrid.UpdateUnitPosition(AxialCoords, targetPosition, this);
            AxialCoords = targetPosition;
            transform.position = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);
            targetTile.SetState(EnumHelper.ConvertToHexState(this.Team));
            ClearHighlights();


            List<HexTile> secondMoveOptions = hexGrid.GetTilesWithinRange(AxialCoords, 1);

            foreach (HexTile tile in secondMoveOptions)
            {
                Unit enemyUnit = hexGrid.GetUnitInTile(tile.axialCoords);


                validMoveTiles.Add(tile);
                tile.HighlightTile();
                if (enemyUnit == null)
                {

                }
                else if (EnumHelper.ConvertToTeam(tile.state) != this.Team)
                {
                    Vector2Int pushPos = tile.axialCoords + (tile.axialCoords - AxialCoords);
                    HexTile pushTile = hexGrid.GetHexTile(pushPos);

                    if (pushTile != null && hexGrid.GetUnitInTile(pushPos) == null)
                    {
                        // ✅ Push enemy forward
                        hexGrid.UpdateUnitPosition(tile.axialCoords, pushPos, enemyUnit);
                        enemyUnit.AxialCoords = pushPos;
                        enemyUnit.transform.position = hexGrid.AxialToWorld(pushPos.x, pushPos.y);

                        // ✅ Convert pushed tiles to Panchulinas’ team
                        tile.SetState(EnumHelper.ConvertToHexState(this.Team));
                        pushTile.SetState(EnumHelper.ConvertToHexState(this.Team));

                        // ✅ Move Panchulinas into enemy's original position
                        hexGrid.UpdateUnitPosition(AxialCoords, tile.axialCoords, this);
                        AxialCoords = tile.axialCoords;
                        transform.position = hexGrid.AxialToWorld(tile.axialCoords.x, tile.axialCoords.y);
                        break;
                    }
                }
            }
            firstMove = true;
        }
        else
        {
            HexTile targetTile = hexGrid.GetHexTile(targetPosition);
            if (targetTile == null || !validMoveTiles.Contains(targetTile))
            {
                ClearHighlights();
                return false; // ❌ Invalid move
            }

            // ✅ First Move: Move normally
            hexGrid.UpdateUnitPosition(AxialCoords, targetPosition, this);
            AxialCoords = targetPosition;
            transform.position = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);
            targetTile.SetState(EnumHelper.ConvertToHexState(this.Team));
            ClearHighlights();
            firstMove = false;
        }

        //ClearHighlights(); // ✅ Remove highlights after movement
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
