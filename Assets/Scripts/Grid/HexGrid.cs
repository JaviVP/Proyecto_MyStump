using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [SerializeField] private int gridRadius = 3; // Set in Inspector
    [SerializeField] private GameObject hexPrefab;
    private Dictionary<Vector2Int, HexTile> hexMap = new Dictionary<Vector2Int, HexTile>();

    private const float HEX_WIDTH = 1.732f; // sqrt(3)
    private const float HEX_HEIGHT = 2f;


    [SerializeField]
    private Vector2[] terraMalla;

    void Start()
    {
        GenerateHexGrid();
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

                hexMap[new Vector2Int(q, r)] = hexTile;
            }
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

           for (int i=0; i< terraMalla.Length; i++)
           {
                HexTile hex = GetHexTile(new Vector2Int(Mathf.FloorToInt(terraMalla[i].x), Mathf.FloorToInt(terraMalla[i].y)));
                if (hex!=null)
                {
                    hex.SetState(HexState.Ants);
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