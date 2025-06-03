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
        if (GetComponent<Animator>())
        {
            PoseTransition("Idle");
        }
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
                else if (isEnemyTile)
                {
                    Vector2Int enemyPos = tile.axialCoords;
                    Vector2Int pushDirection = new Vector2Int(
                        Mathf.Clamp(enemyPos.x - AxialCoords.x, -1, 1),
                        Mathf.Clamp(enemyPos.y - AxialCoords.y, -1, 1)
                    );
                    Vector2Int nextPos = enemyPos + pushDirection;

                    // ✅ Check if there is an empty tile behind the enemy
                    if (hexGrid.HasTile(nextPos) && hexGrid.GetUnitInTile(nextPos) == null)
                    {
                        validMoveTiles.Add(tile);
                        tile.HighlightTile();
                    }
                }

            }
        }

    }


    public override bool Move(Vector2Int targetPosition)
    {
       
        HexTile targetTile = hexGrid.GetHexTile(targetPosition);
        Vector2Int currentPos = AxialCoords;

        if (currentPos != targetPosition)
        {
            if (GetComponent<Animator>())
            {
                PoseTransition("Move");
            }
        }
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
            //transform.position = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);
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
                    if (GetComponent<Animator>())
                    {
                        PoseTransition("Push");
                    }
                    PushEnemy(enemyUnit, targetTile.axialCoords);
                  
                        VfxManager.Instance.Vfxs[1].transform.position = enemyUnit.transform.position;
                        VfxManager.Instance.Vfxs[1].SetActive(true);
                     
                  
                }

                // ✅ First Move: Move normally
                hexGrid.UpdateUnitPosition(AxialCoords, targetPosition, this);
                AxialCoords = targetPosition;
                //transform.position = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);

                targetTile.SetState(EnumHelper.ConvertToHexState(this.Team));

                ClearHighlights();
                firstMove = false;
            
            }
            
        }
        if (GetComponent<Animator>())
        {
            PoseTransition("Idle");
        }
        GameManager.Instance.LockTiles = true;
        StartCoroutine(Animation(targetPosition));
       
        return true;
        
    }


    public override bool CanMove()
    {
        if (TurnsUntilAvailable < 0)
        {
            return false;
        }

        List<HexTile> firstMoveOptions = hexGrid.GetTilesWithinRange(AxialCoords, 1);

        foreach (HexTile tile in firstMoveOptions)
        {
            Unit unitOnTile = hexGrid.GetUnitInTile(tile.axialCoords);

            bool isFriendlyOrNeutral = (tile.state == HexState.Neutral || tile.state == EnumHelper.ConvertToHexState(this.Team));

            if (!firstMove && isFriendlyOrNeutral && unitOnTile == null)
            {
                return true; // Found a valid move spot
            }
        }

        return false; // No available moves
    }



    IEnumerator Animation(Vector2Int targetPos)
    {
        Vector3 endPos = hexGrid.AxialToWorld(targetPos.x, targetPos.y);
        //float speed = 10.0f;

        // Establecemos la rotación de la unidad hacia la posición final
        transform.LookAt(new Vector3(endPos.x, this.gameObject.transform.position.y, endPos.z));

        endPos.y = 0.2f;
        // Mantenemos la componente Y fija en 1.1 durante el movimiento
        while (true)
        {

            // Movemos la unidad en dirección al objetivo, manteniendo la Y fija
            Vector3 currentPos = Vector3.MoveTowards(transform.position, endPos, GameManager.Instance.AnimationSpeed * Time.deltaTime);

            // Fijamos la componente Y a 1.1
            currentPos.y = 0.2f;

            // Actualizamos la posición de la unidad
            transform.position = currentPos;
            if (currentPos != endPos)
            {
                if (GetComponent<Animator>())
                {
                    PoseTransition("Move");
                }
            }
            // Esperamos un pequeño intervalo antes de continuar
            yield return new WaitForSeconds(0.05f);

            // Verificamos si hemos llegado a la posición final
            if (Vector3.Distance(transform.position, endPos) < 0.2f)
            {
                break;
            }
        }

        // Esperamos un poco después de la animación si es necesario
        yield return new WaitForSeconds(0.2f);
        if (GetComponent<Animator>() && !UsedPreviusTurn && !FirstMove)
        {
            PoseTransition("Die");
        }
        GameManager.Instance.LockTiles = false;

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

        StartCoroutine(EnemyDisplacement(lastValidPos));

    }




    public override void ClearHighlights()
    {

        foreach (HexTile tile in validMoveTiles)
        {
            tile.ResetTileColor();
        }
        validMoveTiles.Clear();
    }

    
    IEnumerator EnemyDisplacement(Vector2Int targetPos)
    {

        if (GetComponent<Animator>())
        {
            SoundManager.instance.PlaySound("PushUnit");
            PoseTransition("Punch");
        }
        if (enemyUnit.GetComponent<Animator>())
        {
            enemyUnit.PoseTransition("Push");
        }

        Vector3 endPos = hexGrid.AxialToWorld(targetPos.x, targetPos.y);
        //float speed = 10.0f;
        endPos.y = 0.2f;
        // Establecemos la rotación de la unidad hacia la posición final
        enemyUnit.transform.LookAt(this.gameObject.transform.position);

        // Mantenemos la componente Y fija en 1.1 durante el movimiento
        while (true)
        {
            // Movemos la unidad en dirección al objetivo, manteniendo la Y fija
            Vector3 currentPos = Vector3.MoveTowards(enemyUnit.transform.position, endPos, GameManager.Instance.AnimationSpeed * Time.deltaTime);

            // Fijamos la componente Y a 1.1
            currentPos.y = 0.2f;

            // Actualizamos la posición de la unidad
            enemyUnit.transform.position = currentPos;

            // Esperamos un pequeño intervalo antes de continuar
            yield return new WaitForSeconds(0.05f);

            // Verificamos si hemos llegado a la posición final
            if (Vector3.Distance(enemyUnit.transform.position, endPos) < 0.2f)
            {
                if (enemyUnit.GetComponent<Animator>())
                {
                    VfxManager.Instance.Vfxs[0].transform.position = enemyUnit.transform.position;
                    VfxManager.Instance.Vfxs[0].SetActive(true);
                    enemyUnit.PoseTransition("Crash");
                    yield return new WaitForSeconds(0.4f);
                    if (enemyUnit.GetComponent<Animator>())
                    {
                        enemyUnit.PoseTransition("Idle");

                    }
                    
                }
                SoundManager.instance.PlaySound("EndHitUnit");
                break;
            }
        }

        // Esperamos un poco después de la animación si es necesario
        yield return new WaitForSeconds(1.5f);
        VfxManager.Instance.Vfxs[0].transform.position = new Vector3(1000, 1000, 1000);
        VfxManager.Instance.Vfxs[0].SetActive(false);
        VfxManager.Instance.Vfxs[1].transform.position = new Vector3(1000, 1000, 1000);
        VfxManager.Instance.Vfxs[1].SetActive(false);
        GameManager.Instance.LockTiles = false;
        
    }



}
