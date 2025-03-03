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
        GenerateHexGrid();
        GenerateUnits();
    }

    private void GenerateHexGrid()
    {
        for (int q = -gridRadius; q <= gridRadius; q++)
        {
            for (int r = -gridRadius; r <= gridRadius; r++)
            {
                if (Mathf.Abs(q + r) > gridRadius) continue;

                Vector3 worldPos = AxialToWorld(q, r);
                GameObject hexObj = Instantiate(hexPrefab, worldPos, Quaternion.identity, transform);
                HexTile hexTile = hexObj.GetComponent<HexTile>();
                hexTile.axialCoords = new Vector2Int(q, r);
                //Debug.Log(q+","+ r); //Coords
                hexMap[new Vector2Int(q, r)] = hexTile;
                units[new Vector2Int(q, r)] = null ;
            }
        }
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



    private Vector3 AxialToWorld(int q, int r)
    {
        float x = HEX_WIDTH * (q + r / 2f);
        float z = HEX_HEIGHT * (r * 0.75f);
        return new Vector3(x, 0, z);
    }

    public HexTile GetHexTile(Vector2Int coords)
    {
        return hexMap.TryGetValue(coords, out HexTile tile) ? tile : null;
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
}