using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HexGridUI : MonoBehaviour
{
    /*
    public GameObject hexButtonPrefab;
    public Transform canvasParent;  // Assign the UI Canvas in the Inspector
    public HexGrid hexGrid;
    public Canvas hexCanvas; // Assign the Canvas in the Inspector

    private Dictionary<Vector2Int, Button> buttonMap = new Dictionary<Vector2Int, Button>();

    private void Start()
    {
        GenerateButtonGrid();
        UpdateCanvasFromGame(); // Initialize UI with current game state
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            hexCanvas.gameObject.SetActive(!hexCanvas.gameObject.activeSelf);
        }
    }

    private void GenerateButtonGrid()
    {
        Debug.Log("Generating Button Grid...");

        foreach (Vector2Int pos in hexGrid.GetAllHexPositions())
        {
            Debug.Log("✅ Creating button at position: " + pos);

            // Convert world position to UI space
            Vector3 worldPos = hexGrid.AxialToWorld(pos.x, pos.y);
            Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);

            // Create button inside the canvas
            GameObject buttonObj = Instantiate(hexButtonPrefab, canvasParent);
            buttonObj.transform.position = screenPos; // Convert to screen space

            Button hexButton = buttonObj.GetComponent<Button>();
            if (hexButton == null)
            {
                Debug.LogError("❌ Button component missing on prefab!");
                return;
            }

            hexButton.onClick.AddListener(() => OnTileClicked(pos));
            buttonMap[pos] = hexButton;
        }

        Debug.Log("✅ Button grid generation completed.");
    }





    private void OnTileClicked(Vector2Int pos)
    {
        if (Input.GetMouseButton(0)) // Left Click (Ants)
        {
            CycleUnit(pos, HexState.Ants);
        }
        else if (Input.GetMouseButton(1)) // Right Click (Termites)
        {
            CycleUnit(pos, HexState.Termites);
        }

        UpdateGameFromCanvas(); // Sync game with UI changes
    }

    private void CycleUnit(Vector2Int pos, HexState team)
    {
        if (!hexGrid.HasTile(pos)) return;

        // Get the next unit type for this specific tile
        int nextUnitIndex = hexGrid.GetNextUnitIndex(pos, team);

        // Remove the previous unit before placing a new one
        hexGrid.ClearUnitAt(pos);

        // Place the new unit at the clicked tile
        hexGrid.SetUnitAt(pos, nextUnitIndex, team);

        // Update UI button color for this specific tile
        UpdateButtonColor(pos, team, nextUnitIndex);
    }



    private void UpdateButtonColor(Vector2Int pos, HexState team, int index)
    {
        if (!buttonMap.ContainsKey(pos)) return;

        Button button = buttonMap[pos];
        if (team == HexState.Ants)
            button.image.color = GetAntColor(index);
        else
            button.image.color = GetTermiteColor(index);
    }

    private void UpdateCanvasFromGame()
    {
        foreach (var unitEntry in hexGrid.GetAllUnits())
        {
            Vector2Int pos = unitEntry.Key;
            Unit unit = unitEntry.Value;

            if (unit == null) continue;

            HexState team = (unit is UnitRunner || unit is UnitTerraFormer || unit is UnitPanchulina) ? HexState.Ants : HexState.Termites;
            int unitIndex = hexGrid.GetUnitIndex(unit);

            UpdateButtonColor(pos, team, unitIndex);
        }
    }

    private void UpdateGameFromCanvas()
    {
        foreach (var entry in buttonMap)
        {
            Vector2Int pos = entry.Key;
            Button button = entry.Value;

            int unitIndex = hexGrid.GetUnitIndexFromColor(button.image.color);
            HexState team = (unitIndex != -1) ? (button.image.color == GetAntColor(unitIndex) ? HexState.Ants : HexState.Termites) : HexState.Neutral;

            hexGrid.SetUnitAt(pos, unitIndex, team);
        }
    }

    private Color GetAntColor(int index)
    {
        Color[] antColors = { Color.red, new Color(0.8f, 0, 0), new Color(0.6f, 0, 0), Color.white };
        return antColors[index];
    }

    private Color GetTermiteColor(int index)
    {
        Color[] termiteColors = { new Color(1, 0.5f, 0), new Color(0.8f, 0.4f, 0), new Color(0.6f, 0.3f, 0), Color.white };
        return termiteColors[index];
    }

    */

}
