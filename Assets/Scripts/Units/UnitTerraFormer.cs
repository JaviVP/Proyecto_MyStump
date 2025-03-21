using System.Collections;
using System.Collections.Generic;
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
        //validMoveTiles.Clear();
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
        //AxialCoords = targetPosition;
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
        
        List<HexTile> way= FindPath(AxialCoords, targetPos);
        if (way != null)
        {
            for (int i = 0; i < way.Count; i++)
            {
                HexTile tile = way[i];
                //tile.ChangeColor(Color.blue);
            }
        }

        
        float speed = 10.0f;
        //float minDistance = 0.2f;
        
        int counter = 0;
        while (true)
        {
            Vector3 endPos = hexGrid.AxialToWorld(way[counter].axialCoords.x, way[counter].axialCoords.y);
            transform.LookAt(endPos);
            transform.position = Vector3.MoveTowards(transform.position, endPos , speed * Time.deltaTime);
            yield return new WaitForSeconds(0.05f);
            if (Vector3.Distance(transform.position, endPos) < 0.2f)
            {
                
                if (counter >= way.Count - 1)
                {
                    break;
                }
                counter++;
            }
        }

        yield return new WaitForSeconds(2.0f);

    }


    public List<HexTile> FindPath(Vector2Int start, Vector2Int target)
    {
        if (hexGrid.GetHexTile(start) == null || !hexGrid.GetHexTile(target) == null)
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
            //current.ChangeColor(Color.black);
            
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
                    
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    cameFrom[neighbor] = current; // Record the path
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
            if (!validMoveTiles.Contains(ntile))
            {
                
            }
        }

        return neighbors;
    }
    private List<HexTile> ReconstructPath(Dictionary<HexTile, HexTile> cameFrom, HexTile current)
    {
        List<HexTile> totalPath = new List<HexTile> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }

        totalPath.Reverse(); // Reverse the path to get it from start to target
        return totalPath;
    }
}
