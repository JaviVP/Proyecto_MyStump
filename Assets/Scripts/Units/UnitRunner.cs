using System.Collections;
using System.Collections.Generic;
using TMPro;
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

        if (GetComponent<Animator>())
        {
            PoseTransition("Idle");
        }
    }

    private HexTile FindLastValidTileInDirection(Vector2Int startPos, Vector2Int direction, int range)
    {
        Vector2Int currentPos = startPos;
        HexTile lastValidTile = null;

        for (int i = 0; i < range; i++)
        {
            currentPos += direction;
            HexTile tile = hexGrid.GetHexTile(currentPos);
            if (tile == null) break;

            Team? tileTeam = EnumHelper.ConvertToTeam(tile.state);
            bool isOccupied = hexGrid.GetUnitInTile(tile.axialCoords) != null;

            if (tile.state == HexState.Neutral)
            {
                lastValidTile = tile;
            }
            else if (tileTeam.HasValue && tileTeam.Value == this.Team && !isOccupied)
            {
                lastValidTile = tile;
            }
            else
            {
                break;
            }
        }

        return lastValidTile;
    }


    public override void OnSelected()
    {
        validMoveTiles.Clear();
        Vector2Int[] directions = {
        new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1),
        new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, -1)
    };

        foreach (Vector2Int direction in directions)
        {
            HexTile lastValidTile = FindLastValidTileInDirection(AxialCoords, direction, 3);
            if (lastValidTile != null)
            {
                validMoveTiles.Add(lastValidTile);
            }
        }

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
        if (targetTile == null || !validMoveTiles.Contains(targetTile))
        {
            ClearHighlights();
            if (GetComponent<Animator>())
            {
                PoseTransition("Idle");
            }
            return false;
        } // ❌ Invalid move

        Vector2Int direction = new Vector2Int(
            Mathf.Clamp(targetPosition.x - AxialCoords.x, -1, 1),
            Mathf.Clamp(targetPosition.y - AxialCoords.y, -1, 1)
        );

        Vector2Int currentPos = AxialCoords;

       
        // ✅ Paint only the path traveled
        while (currentPos != targetPosition)
        {
            currentPos += direction;
            HexTile tile = hexGrid.GetHexTile(currentPos);
            if (tile != null)
            {
                tile.SetState(EnumHelper.ConvertToHexState(this.Team)); // ✅ Paint tile
            }
        }

        // ✅ Update unit position
        hexGrid.UpdateUnitPosition(AxialCoords, targetPosition, this);
        AxialCoords = targetPosition;
        //transform.position = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);

        ClearHighlights(); // ✅ Remove movement highlights
        GameManager.Instance.LockTiles = true;
       
        StartCoroutine(Animation(targetPosition));
        

        return true; // ✅ Movement successful
       
    }


    public bool CanMove()
    {
        Vector2Int[] directions = {
        new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1),
        new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, -1)
    };

        foreach (Vector2Int direction in directions)
        {
            HexTile lastValidTile = FindLastValidTileInDirection(AxialCoords, direction, 3);
            if (lastValidTile != null)
            {
                return true; // Found at least one valid move
            }
        }

        return false; // No possible moves
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
        if (GetComponent<Animator>() && !UsedPreviusTurn)
        {
            PoseTransition("Die");
        } 
        GameManager.Instance.LockTiles = false;
       
    }






}
