using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Rendering;

public class HexGrid : MonoBehaviour
{
    [SerializeField] private int gridRadius = 3; // Set in Inspector
    [SerializeField] private GameObject hexPrefab;
    private Dictionary<Vector2Int, HexTile> hexMap = new Dictionary<Vector2Int, HexTile>();
    private Dictionary<Vector2Int, Unit> units = new Dictionary<Vector2Int, Unit>();
    [SerializeField]  private GameObject[] unitsTermitePrefabs;
    [SerializeField]  private GameObject[] unitsAntsPrefabs;


    private const float HEX_WIDTH = 1.732f; // sqrt(3)
    private const float HEX_HEIGHT = 2f;

    [SerializeField] private List<UnitPlacement> antPlacements = new List<UnitPlacement>();
    [SerializeField] private List<UnitPlacement> termitePlacements = new List<UnitPlacement>();




    void Start()
    {
        Debug.Log("HexGrid Start() is running...");
        GenerateHexGrid();
        Debug.Log("HexGrid has generated " + hexMap.Count + " hex tiles.");
        GenerateUnits();
    }

    private void GenerateHexGrid()
    {
        Debug.Log("Generating Hex Grid...");

        for (int q = -gridRadius; q <= gridRadius; q++)
        {
            for (int r = -gridRadius; r <= gridRadius; r++)
            {
                if (Mathf.Abs(q + r) > gridRadius) continue;

                Vector3 worldPos = AxialToWorld(q, r);
                GameObject hexObj = Instantiate(hexPrefab, worldPos, Quaternion.identity, transform);
                HexTile hexTile = hexObj.GetComponent<HexTile>();

                if (hexTile == null)
                {
                    Debug.LogError("❌ HexTile component is missing on instantiated prefab!");
                    return;
                }

                hexTile.axialCoords = new Vector2Int(q, r);
                hexMap[new Vector2Int(q, r)] = hexTile;
            }
        }

        Debug.Log("✅ Hex Grid Generation Completed! Total tiles: " + hexMap.Count);
    }

    public void GenerateUnits()
    {
        foreach (UnitPlacement placement in antPlacements)
        {
            PlaceUnit(placement, HexState.Ants, unitsAntsPrefabs);
        }
        foreach (UnitPlacement placement in termitePlacements)
        {
            PlaceUnit(placement, HexState.Termites, unitsTermitePrefabs);
        }
    }

    private void PlaceUnit(UnitPlacement placement, HexState team, GameObject[] unitPrefabs)
    {
        if (!hexMap.ContainsKey(placement.position)) return;  // Prevent placing outside grid

        GameObject unitObj = Instantiate(unitPrefabs[(int)placement.unitType - 1], AxialToWorld(placement.position.x, placement.position.y), Quaternion.identity);
        //unitObj.name = unitPrefabs[(int)placement.unitType - 1].name;
        Unit unit = unitObj.GetComponent<Unit>();
        unit.AxialCoords = placement.position;

        units[placement.position] = unit;
        hexMap[placement.position].SetState(team);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Vector2Int newPos = new Vector2Int(-2, 2);
            Unit unit = units[new Vector2Int(1, 3)];
            units[new Vector2Int(1, 3)] = null;
            units[newPos] = unit;
            unit.UnitRenderer.transform.position= AxialToWorld(newPos.x,newPos.y);

            HexTile hex = GetHexTile(new Vector2Int(1, 3));
            hex.SetState(HexState.Neutral);


            hex = GetHexTile(newPos);
            hex.SetState(HexState.Termites);

        }
    }
    public void ClearHexGrid()
    {
        for (int q = -gridRadius; q <= gridRadius; q++)
        {
            for (int r = -gridRadius; r <= gridRadius; r++)
            {
                if (Mathf.Abs(q + r) > gridRadius) continue;

                HexTile hex = GetHexTile(new Vector2Int(q, r));
                hex.SetState(HexState.Neutral);
                
            }
        }
    }


    /*
    public void CreateTerraMallaProve()  //Testing
    {

        int contador = 0;
        for (int q = -gridRadius; q <= gridRadius; q++)
        {
            for (int r = -gridRadius; r <= gridRadius; r++)
            {
                
                if (Mathf.Abs(q + r) > gridRadius) continue;
                if (contador >=1 )
                {
                    HexTile hex = GetHexTile(new Vector2Int(q, r));
                    hex.SetState(HexState.Termites);
                }

            }
            contador++;
            if (contador ==2)
            {
                break;
            }
        }
    

    }


    */


    

    public Vector3 AxialToWorld(int q, int r)
    {
        float x = HEX_WIDTH * (q + r / 2f);
        float z = HEX_HEIGHT * (r * 0.75f);
        return new Vector3(x, 0, z);
    }

    public HexTile GetHexTile(Vector2Int coords)
    {
        return hexMap.TryGetValue(coords, out HexTile tile) ? tile : null;
    }
    public Unit GetUnitInTile(Vector2Int coords)
    {
        return units.TryGetValue(coords, out Unit unit) ? unit : null;
    }
    public List<HexTile> GetHexLine(Vector2Int startPos, Vector2Int direction)
    {
        List<HexTile> lineTiles = new List<HexTile>();
        Vector2Int currentPos = startPos;

        while (hexMap.ContainsKey(currentPos + direction))
        {
            currentPos += direction;
            if (hexMap[currentPos].state != HexState.Neutral) break; // Stop at an obstacle
            lineTiles.Add(hexMap[currentPos]);
        }
        return lineTiles;
    }


    //Look for tiles around
    public List<HexTile> GetTilesWithinRange(Vector2Int start, int range)
    {
        List<HexTile> inRange = new List<HexTile>();

        foreach (var tile in hexMap.Values)
        {
            int distance = GetHexDistance(start, tile.axialCoords);
            if (distance <= range)
            {
                inRange.Add(tile);
            }
        }

        return inRange;
    }

    private int GetHexDistance(Vector2Int a, Vector2Int b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.x + a.y - b.x - b.y)) / 2;
    }










    /// 
    /// Some things for Debugger UI
    /// 



    // Find all the positions
    public List<Vector2Int> GetAllHexPositions()
    {
        if (hexMap == null)
        {
            Debug.LogError("❌ HexMap is NULL! Check if GenerateHexGrid() runs.");
            return new List<Vector2Int>();
        }

        Debug.Log("✅ HexGrid contains " + hexMap.Count + " hex tiles.");
        return new List<Vector2Int>(hexMap.Keys);
    }


    // Find the existance of a tile
    public bool HasTile(Vector2Int pos)
    {
        return hexMap.ContainsKey(pos);
    }

    //Set Units
    public int GetNextUnitIndex(Vector2Int pos, HexState team)
    {
        if (!units.ContainsKey(pos) || units[pos] == null)
            return 1; // Start with the first unit

        int currentIndex = GetUnitIndex(units[pos]);
        return (currentIndex % 3) + 1; // Cycle through 1 → 2 → 3 → 1
    }


    //Positions
    public void SetUnitAt(Vector2Int pos, int unitIndex, HexState team)
    {
        if (!hexMap.ContainsKey(pos)) return;

        // Remove existing unit if setting to neutral
        if (unitIndex == 0)
        {
            if (units.ContainsKey(pos) && units[pos] != null)
            {
                Destroy(units[pos].gameObject); // Destroy existing unit
            }

            units[pos] = null;
            hexMap[pos].SetState(HexState.Neutral);
            return;
        }

        // Ensure we're only affecting the clicked tile
        if (units.ContainsKey(pos) && units[pos] != null)
        {
            Destroy(units[pos].gameObject); // Destroy old unit before adding a new one
        }

        GameObject prefab = (team == HexState.Ants) ? unitsAntsPrefabs[unitIndex - 1] : unitsTermitePrefabs[unitIndex - 1];
        GameObject unitObj = Instantiate(prefab, AxialToWorld(pos.x, pos.y), Quaternion.identity);

        // Add the correct unit script dynamically
        Unit newUnit = null;
        if (unitIndex == 1) newUnit = unitObj.AddComponent<UnitRunner>();
        else if (unitIndex == 2) newUnit = unitObj.AddComponent<UnitTerraFormer>();
        else newUnit = unitObj.AddComponent<UnitPanchulina>();

        newUnit.AxialCoords = pos;
        units[pos] = newUnit;
        hexMap[pos].SetState(team);
    }



    //All Units
    public Dictionary<Vector2Int, Unit> GetAllUnits()
    {
        return units;
    }

    public int GetUnitIndex(Unit unit)
    {
        if (unit == null) return 0; // No unit = Neutral

        if (unit is UnitRunner) return 1;
        if (unit is UnitTerraFormer) return 2;
        if (unit is UnitPanchulina) return 3;

        return 0; // Default to Neutral if unknown
    }



    public int GetUnitIndexFromColor(Color color)
    {
        Color[] antColors = { Color.red, new Color(0.8f, 0, 0), new Color(0.6f, 0, 0), Color.white };
        Color[] termiteColors = { new Color(1, 0.5f, 0), new Color(0.8f, 0.4f, 0), new Color(0.6f, 0.3f, 0), Color.white };

        for (int i = 0; i < antColors.Length; i++)
        {
            if (color == antColors[i]) return i;
            if (color == termiteColors[i]) return i;
        }
        return -1; // Not found
    }

    public void ClearUnitAt(Vector2Int pos)
    {
        if (!units.ContainsKey(pos) || units[pos] == null) return;

        Destroy(units[pos].gameObject); // Destroy the old unit
        units[pos] = null; // Remove unit reference
        hexMap[pos].SetState(HexState.Neutral); // Reset the tile ownership
    }



}