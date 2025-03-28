using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static HexGrid;

public class UnitPanchulina : Unit
{
    private HexGrid hexGrid;
    private HashSet<HexTile> validMoveTiles = new HashSet<HexTile>(); // Store valid move tiles
    private bool firstMove;
    private Unit enemyUnit;

    public bool FirstMove { get => firstMove; set => firstMove = value; }

    private void Start()
    {
        firstMove = false;
        hexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
    }



    public override void OnSelected()
    {
        validMoveTiles.Clear();
        List<HexTile> firstMoveOptions = hexGrid.GetTilesWithinRange(AxialCoords, 1);




        foreach (HexTile tile in firstMoveOptions)
        {
            Unit unitOnTile = hexGrid.GetUnitInTile(tile.axialCoords);

            // ✅ Check if tile is neutral, team-owned, OR occupied by an enemy
            bool isFriendlyOrNeutral = (tile.state == HexState.Neutral || tile.state == EnumHelper.ConvertToHexState(this.Team));
            bool isEnemyTile = (unitOnTile != null && unitOnTile.Team != this.Team);

            if (!firstMove && isFriendlyOrNeutral && unitOnTile == null)
            {
                // ✅ First move - can move onto empty friendly or neutral tiles
                validMoveTiles.Add(tile);
                tile.HighlightTile();
            }
            else if (firstMove)
            {
                // ✅ Second move - allow movement onto empty tiles AND highlight enemies
                if (isFriendlyOrNeutral && unitOnTile == null)
                {
                    validMoveTiles.Add(tile);
                    tile.HighlightTile();
                }
                else if (isEnemyTile) // ✅ Enemy tile is now considered separately
                {
                    validMoveTiles.Add(tile);
                    tile.HighlightTile();
                }
            }
        }

    }



    /*
    public override bool Move(Vector2Int targetPosition)
    {
        HexTile targetTile = hexGrid.GetHexTile(targetPosition);
        if (!FirstMove)
        {
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
            if (targetTile == null || !validMoveTiles.Contains(targetTile))
            {
                return false; // ❌ Invalid move
            }
            else
            {

                // ✅ First Move: Move normally
                hexGrid.UpdateUnitPosition(AxialCoords, targetPosition, this);
                AxialCoords = targetPosition;
                transform.position = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);
                targetTile.SetState(EnumHelper.ConvertToHexState(this.Team));
                //ClearHighlights();
                firstMove = false;
            }
        }

        //ClearHighlights(); // ✅ Remove highlights after movement
        return true;
    }

    */
    public override bool Move(Vector2Int targetPosition)
    {
        HexTile targetTile = hexGrid.GetHexTile(targetPosition);
        if (!FirstMove)
        {
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
            firstMove = true;
            OnSelected();

        }
        else
        {
            if (targetTile == null || !validMoveTiles.Contains(targetTile))
            {
                return false; // ❌ Invalid move
            } 
            else
            {

                enemyUnit = hexGrid.GetUnitInTile(targetTile.axialCoords);
                if (enemyUnit != null && enemyUnit.Team != this.Team)
                {
                    PushEnemy(enemyUnit, targetTile.axialCoords);
                }

                // ✅ First Move: Move normally
                hexGrid.UpdateUnitPosition(AxialCoords, targetPosition, this);
                AxialCoords = targetPosition;
                transform.position = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);

                targetTile.SetState(EnumHelper.ConvertToHexState(this.Team));

                ClearHighlights();
                firstMove = false;

            }
            
        }
        return true;
    }


    private void PushEnemy(Unit enemy, Vector2Int enemyPosition)
    {
        Vector2Int direction = new Vector2Int(
            Mathf.Clamp(enemyPosition.x - AxialCoords.x, -1, 1),
            Mathf.Clamp(enemyPosition.y - AxialCoords.y, -1, 1)
        );

        Vector2Int nextPos = enemyPosition + direction;
        Vector2Int lastValidPos = enemyPosition; // Store last valid enemy position

        // ✅ Move the enemy until it hits an obstacle
        while (hexGrid.HasTile(nextPos) && hexGrid.GetUnitInTile(nextPos) == null)
        {
            HexTile tile = hexGrid.GetHexTile(nextPos);

            /// Actualizar para cuando hayan obstaculos 

            /*
            // ❌ Stop if the tile is an obstacle
            if (tile.state != HexState.Neutral && EnumHelper.ConvertToTeam(tile.state) != this.Team)
            {
                break;
            }
            */
            // ✅ Paint tile to Panchulinas’ team
            tile.SetState(EnumHelper.ConvertToHexState(this.Team));

            lastValidPos = nextPos; // ✅ Save last valid push position
            nextPos += direction;
        }

        // ✅ Move enemy to its final pushed position
        hexGrid.UpdateUnitPosition(enemy.AxialCoords, lastValidPos, enemy);
        enemy.AxialCoords = lastValidPos;
        //enemy.transform.position = hexGrid.AxialToWorld(lastValidPos.x, lastValidPos.y);


        // ✅ Restore enemy’s color on final position
        HexTile finalTile = hexGrid.GetHexTile(lastValidPos);
        finalTile.SetState(EnumHelper.ConvertToHexState(enemy.Team)); // ✅ Keep the enemy’s color


        GameManager.Instance.LockTiles = true;
        StartCoroutine(Animation(lastValidPos));

    }




    public override void ClearHighlights()
    {

        foreach (HexTile tile in validMoveTiles)
        {
            tile.ResetTileColor();
        }
        validMoveTiles.Clear();
    }

    IEnumerator Animation(Vector2Int targetPos)
    {
        Vector3 endPos = hexGrid.AxialToWorld(targetPos.x, targetPos.y);
        float speed = 10.0f;

        // Establecemos la rotación de la unidad hacia la posición final
        enemyUnit.transform.LookAt(endPos);

        // Mantenemos la componente Y fija en 1.1 durante el movimiento
        while (true)
        {
            // Movemos la unidad en dirección al objetivo, manteniendo la Y fija
            Vector3 currentPos = Vector3.MoveTowards(enemyUnit.transform.position, endPos, speed * Time.deltaTime);

            // Fijamos la componente Y a 1.1
            currentPos.y = 0.1f;

            // Actualizamos la posición de la unidad
            enemyUnit.transform.position = currentPos;

            // Esperamos un pequeño intervalo antes de continuar
            yield return new WaitForSeconds(0.05f);

            // Verificamos si hemos llegado a la posición final
            if (Vector3.Distance(enemyUnit.transform.position, endPos) < 0.2f)
            {
                break;
            }
        }

        // Esperamos un poco después de la animación si es necesario
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.LockTiles = false;
    }









}
