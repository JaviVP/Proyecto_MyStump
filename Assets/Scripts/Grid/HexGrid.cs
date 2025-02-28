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
                Debug.Log(q+"-"+ r);
                hexMap[new Vector2Int(q, r)] = hexTile;
                units[new Vector2Int(q, r)] = null ;
            }
        }
    }
    public void GenerateUnits()
    {

        //Termites
        Vector2Int vector2Int = new Vector2Int(1,3);
        Unit unitTermiteRunner = new UnitRunner();
        unitTermiteRunner.UnitRenderer = unitsTermitePrefabs[0];
        unitTermiteRunner.AxialCoords = vector2Int;
        unitTermiteRunner.UnitRenderer.transform.position= AxialToWorld(vector2Int.x,vector2Int.y);
        units[vector2Int] = unitTermiteRunner;
        HexTile hex = GetHexTile(vector2Int);
        hex.SetState(HexState.Termites);



        vector2Int = new Vector2Int(-2, 3);
        Unit unitTermitTerraformer = new UnitTerraFormer();
        unitTermitTerraformer.UnitRenderer = unitsTermitePrefabs[1];
        unitTermitTerraformer.AxialCoords = vector2Int;
        unitTermitTerraformer.UnitRenderer.transform.position = AxialToWorld(vector2Int.x, vector2Int.y);
        units[vector2Int] = unitTermitTerraformer;
        hex = GetHexTile(vector2Int);
        hex.SetState(HexState.Termites);


        vector2Int = new Vector2Int(-1, 1);
        Unit unitTermitPanchulina= new UnitPanchulina();
        unitTermitPanchulina.UnitRenderer = unitsTermitePrefabs[2];
        unitTermitPanchulina.AxialCoords = vector2Int;
        unitTermitPanchulina.UnitRenderer.transform.position = AxialToWorld(vector2Int.x, vector2Int.y);
        units[vector2Int] = unitTermitPanchulina;
        hex = GetHexTile(vector2Int);
        hex.SetState(HexState.Termites);

        //Ants

        vector2Int = new Vector2Int(0, 0);
        Unit unitAntRunner = new UnitRunner();
        unitAntRunner.UnitRenderer = unitsAntsPrefabs[0];
        unitAntRunner.AxialCoords = vector2Int;
        unitAntRunner.UnitRenderer.transform.position = AxialToWorld(vector2Int.x, vector2Int.y);
        units[vector2Int] = unitAntRunner;
        hex = GetHexTile(vector2Int);
        hex.SetState(HexState.Ants);

        vector2Int = new Vector2Int(-1, 2);
        Unit unitAntTerraformer = new UnitTerraFormer();
        unitAntTerraformer.UnitRenderer = unitsAntsPrefabs[1];
        unitAntTerraformer.AxialCoords = vector2Int;
        unitAntTerraformer.UnitRenderer.transform.position = AxialToWorld(vector2Int.x, vector2Int.y);
        units[vector2Int] = unitAntTerraformer;
        hex = GetHexTile(vector2Int);
        hex.SetState(HexState.Ants);


        vector2Int = new Vector2Int(-1, 1);
        Unit unitAntPanchulina = new UnitPanchulina();
        unitAntPanchulina.UnitRenderer = unitsAntsPrefabs[2];
        unitAntPanchulina.AxialCoords = vector2Int;
        unitAntPanchulina.UnitRenderer.transform.position = AxialToWorld(vector2Int.x, vector2Int.y);
        units[vector2Int] = unitAntPanchulina;
        hex = GetHexTile(vector2Int);
        hex.SetState(HexState.Ants);






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