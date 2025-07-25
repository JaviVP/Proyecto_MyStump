﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static GameManager;
using static HexGrid;

public class UnitTerraFormer : Unit
{
    private HexGrid hexGrid;
    private HashSet<HexTile> validMoveTiles = new HashSet<HexTile>(); // Store valid move tiles
    private HashSet<HexTile> validMoveTilesAnimation = new HashSet<HexTile>(); // Store valid move tiles

    private void Start()
    {
        hexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
        if (GetComponent<Animator>())
        {
            PoseTransition("Idle");
        }
        
       

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
                        //tile.ChangeColor(Color.black);
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
        //validMoveTiles.Clear();
    }


    public override bool Move(Vector2Int targetPosition)
    {
        HexTile targetTile = hexGrid.GetHexTile(targetPosition);
        Vector2Int currentPos = AxialCoords;

        if (targetTile == null || !validMoveTiles.Contains(targetTile))
        {
            ClearHighlights();
            return false;
        }

        GameManager.Instance.LockTiles = true;
        StartCoroutine(MoveWithConversion(targetTile, targetPosition, currentPos));

        return true;
    }

    private IEnumerator MoveWithConversion(HexTile targetTile, Vector2Int targetPosition, Vector2Int currentPos)
    {
        Vector3 lookAtPos = hexGrid.AxialToWorld(targetPosition.x, targetPosition.y);
        lookAtPos.y = transform.position.y; // mantener la altura del personaje
        transform.LookAt(lookAtPos);
        //Ejecuta VFX y cambia el estado de la casilla antes de moverse
        if ((Team == Team.Ants && targetTile.state == HexState.Termites) ||
            (Team == Team.Termites && targetTile.state == HexState.Ants))
        {
            transform.GetChild(1).gameObject.SetActive(true);
            if (currentPos != targetPosition && GetComponent<Animator>())
            {
                PoseTransition("Short");
                if (Team == Team.Ants)
                {
                    SoundManager.instance.PlaySound("Flame");
                }
                else if (Team == Team.Termites)
                {
                    SoundManager.instance.PlaySound("WaterCanon");
                }
            }
        
       
        //Espera breve para que el VFX se vea
                yield return new WaitForSeconds(1.0f);
        } 
        targetTile.SetState(EnumHelper.ConvertToHexState(this.Team));
        //Mueve la unidad a la nueva posición
        hexGrid.UpdateUnitPosition(AxialCoords, targetPosition, this);

        //Reproduce animación de movimiento si es necesario
        if (currentPos != targetPosition && GetComponent<Animator>())
        {
            PoseTransition("Long");
        }

        //Limpia los highlights después del movimiento
        ClearHighlights();

        //Ejecuta la animación de desplazamiento visual
        yield return StartCoroutine(Animation(targetPosition));

        GameManager.Instance.LockTiles = false;
    }

    public override bool CanMove()
    {
        if (TurnsUntilAvailable < 0)
        {
            return false;
        }

        List<HexTile> adjacentTiles = hexGrid.GetTilesWithinRange(AxialCoords, 1);

        foreach (HexTile tile in adjacentTiles)
        {
            bool isOccupied = hexGrid.GetUnitInTile(tile.axialCoords) != null;

            if (!isOccupied)
            {
                return true; // ✅ Found a free adjacent tile
            }
        }

        return false; // ❌ No free adjacent tiles
    }



    IEnumerator Animation(Vector2Int targetPos)
    {
        List<HexTile> way = FindPath(AxialCoords, targetPos);
        if (way != null)
        {
            for (int i = 0; i < way.Count; i++)
            {
                HexTile tile = way[i];
                //tile.ChangeColor(Color.blue); // Si deseas cambiar el color de las baldosas, lo puedes hacer aquí.
            }
        }

        //float speed = 10.0f;
        int counter = 0;

        while (true)
        {
            // Convertimos la posición axial a la posición en el mundo
            Vector3 endPos = hexGrid.AxialToWorld(way[counter].axialCoords.x, way[counter].axialCoords.y);

            // Mantenemos la componente Y fija en 1.1
            endPos.y = 0.2f; // Offset en Y durante toda la animación

            // Hacemos que la unidad mire hacia la nueva posición
            transform.LookAt(endPos);

            // Movemos la unidad hacia la nueva posición
            transform.position = Vector3.MoveTowards(transform.position, endPos, GameManager.Instance.AnimationSpeed * Time.deltaTime);

            // Esperamos un corto tiempo para el siguiente paso
            yield return new WaitForSeconds(0.05f);

            // Verificamos si la unidad ha llegado a la posición final
            if (Vector3.Distance(transform.position, endPos) < 0.2f)
            {
                // Si ya ha llegado al final de la lista de tiles, terminamos la animación
                if (counter >= way.Count - 1)
                {
                    break;
                }

                // De lo contrario, continuamos al siguiente tile
                counter++;
            }
        }

        // Actualizamos las coordenadas del juego
        AxialCoords = targetPos;

        // Esperamos un poco después de la animación
        yield return new WaitForSeconds(0.2f);
        if (GetComponent<Animator>() && !UsedPreviusTurn)
        {
            PoseTransition("Die");
        }
        GameManager.Instance.LockTiles = false;
        GameManager.Instance.CheckTurnEnd();
       

    }


    public List<HexTile> FindPath(Vector2Int start, Vector2Int target)
    {
        


        if (hexGrid.GetHexTile(start) == null || hexGrid.GetHexTile(target) == null)
        {
            return new List<HexTile>(); // Return empty if start or target is invalid
        }
        
        HexTile startTile = hexGrid.GetHexTile(start);
        HexTile targetTile = hexGrid.GetHexTile(target);

        Queue<HexTile> queue = new Queue<HexTile>();
        HashSet<HexTile> visited = new HashSet<HexTile>();
        Dictionary<HexTile, HexTile> cameFrom = new Dictionary<HexTile, HexTile>();

        queue.Enqueue(startTile);
        visited.Add(startTile);
        
        while (queue.Count > 0)
        {
            HexTile current = queue.Dequeue();
            if (current==null)
            {
               
                continue;
                
            }
           
            
            if (current.axialCoords == target)
            {
                Debug.Log("-- Find it  --");

                return ReconstructPath(cameFrom, current);
            }
            
            foreach (HexTile neighbor in GetNeighbors(current))
            {
            
                if ( visited.Contains(neighbor) || !validMoveTiles.Contains(neighbor))
                {
                    continue; // Ignore non-walkable or already visited nodes
                }
            
                if (neighbor != null)
                {
                    //With the same color

                    
                    Debug.Log("Team: "+ this.Team+ "-->estado: "+neighbor.state);
                    if (this.Team.ToString()==neighbor.state.ToString())
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);

                        cameFrom[neighbor] = current; // Record the path
                    }

                    
                }
            }
        }
        return new List<HexTile>();


    }

    private List<HexTile> GetNeighbors(HexTile tile)
    {
        List<HexTile> neighbors = new List<HexTile>();
        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1),
            new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, -1)
        };

        foreach (var direction in directions)
        {
            Vector2Int neighborPosition = tile.axialCoords + direction;
            HexTile ntile= hexGrid.GetHexTile(neighborPosition);
            //If the tile is in the list of validMoveTiles

            neighbors.Add(ntile);
            
        }

        return neighbors;
    }
    private List<HexTile> ReconstructPath(Dictionary<HexTile, HexTile> cameFrom, HexTile current)
    {
        if (GetComponent<Animator>())
        {
            PoseTransition("Short");
        }
        List<HexTile> totalPath = new List<HexTile> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }

        totalPath.Reverse(); // Reverse the path to get it from start to target
        if (GetComponent<Animator>())
        {
            PoseTransition("Idle");
        }
        
        return totalPath;
    }
}
