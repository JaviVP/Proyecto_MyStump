﻿using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using static GameManager;

public class HexGrid : MonoBehaviour
{
    [SerializeField] private float hexSpacing = 1f;
    [SerializeField] private int gridRadius = 3; // Set in Inspector
    [SerializeField] private GameObject hexPrefab;
    private Dictionary<Vector2Int, HexTile> hexMap = new Dictionary<Vector2Int, HexTile>();
    private List<HexTile> InactiveTiles= new List<HexTile>();
    private Dictionary<Vector2Int, Unit> units = new Dictionary<Vector2Int, Unit>();
    [SerializeField]  private GameObject[] unitsTermitePrefabs;
    [SerializeField]  private GameObject[] unitsAntsPrefabs;


    private const float HEX_WIDTH = 1.732f; // sqrt(3)
    private const float HEX_HEIGHT = 2f;

    [SerializeField] private List<UnitPlacement> antPlacements = new List<UnitPlacement>();
    [SerializeField] private List<UnitPlacement> termitePlacements = new List<UnitPlacement>();


    /// BASE

    [SerializeField] private bool useCustomBaseCoordinates = false;
    [SerializeField] private Vector2Int termiteBaseCoords;
    [SerializeField] private Vector2Int antBaseCoords;
    [SerializeField] private GameObject baseTermitePrefab;
    [SerializeField] private GameObject baseAntPrefab;



    private int antsKilled;
    private int termsKilled;
    private int termiteUnitsCount;
    private int antUnitsCount;

    void Start()
    {
        termiteUnitsCount = GetNumberOfGameObjects(unitsTermitePrefabs);  // Obtiene el número de unidades en el array de termitas
        antUnitsCount = GetNumberOfGameObjects(unitsAntsPrefabs);  // Obtiene el número de unidades en el array de hormigas
        PlayerPrefs.SetInt("TermCount",termiteUnitsCount);
        PlayerPrefs.SetInt("AntCount", antUnitsCount);
        Debug.Log("Número de unidades de termitas: " + PlayerPrefs.GetInt("TermCount"));
        Debug.Log("Número de unidades de hormigas: " + PlayerPrefs.GetInt("AntCount"));
        //Debug.Log("HexGrid Start() is running...");
        GenerateHexGrid();
        //Debug.Log("HexGrid has generated " + hexMap.Count + " hex tiles.");
        SpawnBases();
        GenerateUnits();
        //Testing
       // TerraFormerTilesProves();
    }

    public void RemoveTile(Vector2Int pos)
    {
        HexTile tile = GetHexTile(pos);
        tile.TileRenderer.gameObject.SetActive(false);
        hexMap.Remove(pos);
        InactiveTiles.Add(tile);
    }



     private int GetNumberOfGameObjects(GameObject[] unitsArray)
        
    {
            return unitsArray.Length;  
        
    }

    public int GetUnitsByType(Team team)
    {
        int i = 0;
        foreach(Unit unit in units.Values)
        {
            if (unit.Team == team)
            {
                i++;
            }
        }
        return i;

       
    }
    public void CheckDestroyUnity(Team team)
    {
        List<Vector2Int> destroyUnits= new List<Vector2Int>();

        foreach (Vector2Int pos in units.Keys)
        {
            if (pos == null) continue;
            Unit unit = GetUnitInTile(pos);
            if (unit != null && unit.UnitRenderer != null)
            {
                if (unit.Team==team)
                {
                    int contador = 1;
                    List<HexTile> neighbor = GetTilesWithinRange(unit.AxialCoords, 1);
                    if (neighbor != null)
                    {
                        foreach (HexTile neighborTile in neighbor)
                        {

                            //neighborTile.ChangeColor(Color.blue);
                            if (neighborTile.state!=HexState.Neutral && EnumHelper.ConvertToTeam(neighborTile.state) !=team)
                            {
                                contador++;
                            }
                        }
                        Debug.Log("-->"+ contador+ "--" + neighbor.Count);
                        if (contador == neighbor.Count)
                        {
                            //Destruyo la unidad
                            Debug.Log("---Destruyo la unidad---");

                            


                            unit.UnitRenderer.SetActive(false);
                            unit.AxialCoords = new Vector2Int(1000, 1000);
                            HexTile tile = GetHexTile(pos);
                            
                            destroyUnits.Add(pos);
                          if(unit.Team == Team.Ants)
                            {
                                if (unit is UnitBase)
                                {
                                    /// Si habran mas jugadores en el 
                                    /// posible futuro, este codigo
                                    /// se tendra que actualizar

                                    Debug.Log($"Base destroyed! {team} loses.");

                                    //Instance.DeclareWinner(team == Team.Ants ? Team.Termites : Team.Ants);

                                    PlayerPrefs.SetInt("AntCount", 1);



                                }
                                else
                                {
                                    antsKilled = PlayerPrefs.GetInt("AntsKilled");
                                    antsKilled += 1;
                                    PlayerPrefs.SetInt("AntsKilled", antsKilled);
                                    antUnitsCount--;
                                    PlayerPrefs.SetInt("AntCount", antUnitsCount);
                                }
                                

                            }
                            else if (unit.Team == Team.Termites)
                            {
                                if (unit is UnitBase)
                                { 
                                    Debug.Log($"Base destroyed! {team} loses.");

                                    PlayerPrefs.SetInt("TermCount", 1);

                                }
                                else
                                {
                                    termsKilled = PlayerPrefs.GetInt("TermsKilled");
                                    termsKilled += 1;
                                    PlayerPrefs.SetInt("TermsKilled", termsKilled);
                                    termiteUnitsCount--;
                                    PlayerPrefs.SetInt("TermCount", termiteUnitsCount);
                                }
                                
                            }
                            

                        }
                    }

                    
                }
                
            }
           

        }
        foreach (Vector2Int v in destroyUnits)
        {
            units.Remove(v);


        }
    }

    public void SelectTeam(Team team)
    {
        Debug.Log("Desmarco");
        foreach (Vector2Int pos in units.Keys)
        {
            
            Unit unit = GetUnitInTile(pos);
            if (unit != null && unit.UnitRenderer!=null)
            {
                //Debug.Log("V:" + unit.AxialCoords + " T:" + unit.Team.ToString() + " G:" + unit.UnitRenderer.name);
                for (int i=0; i< unit.UnitRenderer.transform.childCount; i++)
                {
                    
                    if (unit.UnitRenderer.transform.GetChild(i).GetComponent<MeshRenderer>())
                    {
                        if (unit.Team == team)
                        {
                            Color c;
                            if (team== Team.Ants)
                            {
                                c = Color.red;
                            }
                            else
                            {
                                c = Color.yellow;
                            }
                            unit.UnitRenderer.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = c ;
                        }
                        else
                        {
                            unit.UnitRenderer.transform.GetChild(i).GetComponent<MeshRenderer>().material.color = Color.gray;
                        }
                        
                    }

                }
                
               
                
                
            }
            // 'unit' represents the current 'Unit' object in the dictionary
            //Console.WriteLine(unit); // or any other action you want to perform with the unit
        }

    }
    private void TerraFormerTilesProves()
    {
        hexMap[new Vector2Int(-3,1)].SetState(HexState.Termites);
        hexMap[new Vector2Int(-2, 1)].SetState(HexState.Termites);
        hexMap[new Vector2Int(-1, 1)].SetState(HexState.Termites);
        hexMap[new Vector2Int(-2, 0)].SetState(HexState.Termites);
        hexMap[new Vector2Int(-1, 1)].SetState(HexState.Termites);
        hexMap[new Vector2Int(-1, 0)].SetState(HexState.Termites);
        hexMap[new Vector2Int(0,0)].SetState(HexState.Termites);
        hexMap[new Vector2Int(0, -1)].SetState(HexState.Termites);
        hexMap[new Vector2Int(0, -2)].SetState(HexState.Termites);
        hexMap[new Vector2Int(1, -2)].SetState(HexState.Termites);
        hexMap[new Vector2Int(2, -2)].SetState(HexState.Termites);
        hexMap[new Vector2Int(2, -1)].SetState(HexState.Termites);

       


        hexMap[new Vector2Int(1, 1)].SetState(HexState.Ants);
        hexMap[new Vector2Int(0, 1)].SetState(HexState.Ants);
        hexMap[new Vector2Int(-1, 2)].SetState(HexState.Ants);

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

                if (hexTile == null)
                {
                    //Debug.LogError("❌ HexTile component is missing on instantiated prefab!");
                    return;
                }

                hexTile.axialCoords = new Vector2Int(q, r);
                hexMap[new Vector2Int(q, r)] = hexTile;
            }
        }

        //Debug.Log("✅ Hex Grid Generation Completed! Total tiles: " + hexMap.Count);
    }

    public Vector3 AxialToWorld(int q, int r)
    {
        // Para hexágonos FLAT-TOPPED (lado plano arriba)
        float x = hexSpacing * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f) / 2f * r);
        float z = hexSpacing * (3f / 2f * r);
        return new Vector3(x, 0f, z);
    }
    public Vector2Int WorldToAxial(float x, float z)
    {
        // Primero, calculamos el valor de r
        float r = (2f / 3f) * (z / hexSpacing);

        // Luego, calculamos el valor de q
        float q = (x / (hexSpacing * Mathf.Sqrt(3f))) - (r / 2f);

        // Convertimos q y r a enteros y los devolvemos como Vector2Int
        return new Vector2Int(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
    }


    /*
    public Vector3 AxialToWorld(int q, int r)
    {
        float x = HEX_WIDTH * (q + r / 2f);
        float z = HEX_HEIGHT * (r * 0.75f);
        return new Vector3(x, 0, z);
    }
    */

    public List<HexTile> GetHexTileTerraFormar(Vector2Int coords)
    {
        return null;
    }

    public void GenerateUnits()
    {
        foreach (UnitPlacement placement in antPlacements)
        {
            PlaceUnit(placement, HexState.Ants, GameManager.Team.Ants, unitsAntsPrefabs);
        }
        foreach (UnitPlacement placement in termitePlacements)
        {
            PlaceUnit(placement, HexState.Termites, GameManager.Team.Termites, unitsTermitePrefabs);
        }
    }


    //Regular unit Spawning
    private void PlaceUnit(UnitPlacement placement, HexState owning, GameManager.Team team, GameObject[] unitPrefabs)
    {
        if (!hexMap.ContainsKey(placement.position)) return;  // Prevent placing outside grid

        // Obtén la posición en el mundo usando AxialToWorld
        Vector3 worldPosition = AxialToWorld(placement.position.x, placement.position.y);

        // Añadir el offset en el eje Y
        worldPosition.y += 0.1f;

        // Definir la rotación inicial (sin rotación, Quaternion.identity)
        Quaternion rotation = Quaternion.identity;

        // Asignar una rotación extra dependiendo del equipo
        if (team == GameManager.Team.Termites)
        {
            rotation = Quaternion.Euler(0, 90, 0);  // 90 grados en Y para las termitas
        }
        else if (team == GameManager.Team.Ants)
        {
            rotation = Quaternion.Euler(0, -90, 0);  // -90 grados en Y para las hormigas
        }

        // Instanciar la unidad con la rotación ajustada
        GameObject unitObj = Instantiate(unitPrefabs[(int)placement.unitType - 1], worldPosition, rotation);

        Unit unit = unitObj.GetComponent<Unit>();
        unit.UnitRenderer = unitObj;
        unit.AxialCoords = placement.position;
        unit.Team = team;

        units[placement.position] = unit;
        hexMap[placement.position].SetState(owning);
    }


    //Base unit Spawning
    public void PlaceUnit(Vector2Int position, HexState owning, GameManager.Team team, GameObject unitPrefab)
    {
        if (!hexMap.ContainsKey(position)) return;

        Vector3 worldPosition = AxialToWorld(position.x, position.y);
        worldPosition.y += 0.1f;

        Quaternion rotation = Quaternion.identity;
        if (team == GameManager.Team.Termites)
            rotation = Quaternion.Euler(0, 90, 0);
        else if (team == GameManager.Team.Ants)
            rotation = Quaternion.Euler(0, -90, 0);

        GameObject unitObj = Instantiate(unitPrefab, worldPosition, rotation);

        Unit unit = unitObj.GetComponent<Unit>();
        unit.UnitRenderer = unitObj;
        unit.AxialCoords = position;
        unit.Team = team;

        units[position] = unit;
        hexMap[position].SetState(owning);
    }



    private void SpawnBases()
    {
        Vector2Int termiteSpawn;
        Vector2Int antSpawn;

        if (useCustomBaseCoordinates)
        {
            termiteSpawn = termiteBaseCoords;
            antSpawn = antBaseCoords;
        }
        else
        {
            // Get leftmost for termites, rightmost for ants
            termiteSpawn = GetLeftmostHex();
            antSpawn = GetRightmostHex();
        }


        PlaceUnit(termiteSpawn, HexState.Termites, Team.Termites, baseTermitePrefab);
        PlaceUnit(antSpawn, HexState.Ants, Team.Ants, baseAntPrefab);


    }



    private void Update()
    {
       
    }

    public int CountNeutralTiles()
    {
        int count = 0;
        foreach (var hexTile in hexMap.Values)
        {
            if (hexTile.state == HexState.Neutral)
            {
                count++;
            }
        }
        return count;
    }
    public int totalNumberOfTiles()
    {
        return hexMap.Count;
    }

       
    public int GetCountStateTiles(HexState st)
    {
        int counter = 0;
        for (int q = -gridRadius; q <= gridRadius; q++)
        {
            for (int r = -gridRadius; r <= gridRadius; r++)
            {
                if (Mathf.Abs(q + r) > gridRadius) continue;

                HexTile hex = GetHexTile(new Vector2Int(q, r));

                if (hex!=null && hex.state==st)
                {
                    counter++;
                }
                
            }
        }
        return counter;
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




    private Vector2Int GetLeftmostHex()
    {
        Vector2Int leftmost = Vector2Int.zero;
        float minX = float.MaxValue;

        foreach (var kvp in hexMap)
        {
            Vector3 worldPos = AxialToWorld(kvp.Key.x, kvp.Key.y);
            if (worldPos.x < minX)
            {
                minX = worldPos.x;
                leftmost = kvp.Key;
            }
        }

        return leftmost;
    }

    private Vector2Int GetRightmostHex()
    {
        Vector2Int rightmost = Vector2Int.zero;
        float maxX = float.MinValue;

        foreach (var kvp in hexMap)
        {
            Vector3 worldPos = AxialToWorld(kvp.Key.x, kvp.Key.y);
            if (worldPos.x > maxX)
            {
                maxX = worldPos.x;
                rightmost = kvp.Key;
            }
        }

        return rightmost;
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

    public List<HexTile> GetTilesWithinRange(Vector2Int start, int rangeA, int rangeB)
    {
        List<HexTile> inRange = new List<HexTile>();
        int minRange = rangeA - 1;
        if (minRange < 0)
        {
            minRange = 0;
            
            inRange.Add(GetHexTile(start));
        }
        List<HexTile> outRange = GetTilesWithinRange(start, minRange);


        

        foreach (var tile in hexMap.Values)
        {
            int distance = GetHexDistance(start, tile.axialCoords);
            if (distance <= rangeB)
            {
                if (!outRange.Contains(tile))
                {
                    inRange.Add(tile);
                }
            }
        }

        return inRange;
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

    //Eliminate unit after movement
    public void UpdateUnitPosition(Vector2Int oldPos, Vector2Int newPos, Unit unit)
    {
        if (units.ContainsKey(oldPos))
            units[oldPos] = null; // Remove unit from old position

        units[newPos] = unit; // Place unit in new position
    }


    private int GetHexDistance(Vector2Int a, Vector2Int b)
    {
        return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.x + a.y - b.x - b.y)) / 2;
    }


    public bool HasTile(Vector2Int pos)
    {
        return hexMap.ContainsKey(pos);
    }

    public static class EnumHelper
    {
        public static Team? ConvertToTeam(HexState state) // Return nullable Team
        {
            return state switch
            {
                HexState.Ants => Team.Ants,
                HexState.Termites => Team.Termites,
                HexState.Neutral => null, // ✅ Return null instead of throwing an error
                _ => throw new System.ArgumentException("Invalid HexState value."),
            };
        }

        public static HexState ConvertToHexState(Team team)
        {
            return team switch
            {
                Team.Ants => HexState.Ants,
                Team.Termites => HexState.Termites,
                _ => throw new System.ArgumentException("Invalid Team value."),
            };
        }
    }
}