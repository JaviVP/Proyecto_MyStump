using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
public class GameManager : MonoBehaviour
{
    private CinemachineBrain brain;
    private Camera mainCamera;
    private HexGrid hexGrid;
    private List<HexTile> highlightedTiles = new List<HexTile>();

    private List<HexTile> terraFormTiles = new List<HexTile>();


    // Nueva variable para bloquear las entradas táctiles durante la transición
    private bool disableTouchInputDuringTransition = false;
    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        mainCamera = Camera.main;
        hexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
        //hexGrid.CreateTerraMallaProve(); 
    }

    void Update()
    {
        //Debug.Log(Screen.safeArea);
        // Comprobamos si está en transición (si es true, desactivamos las entradas táctiles)
        if (brain.IsBlending)
        {
            disableTouchInputDuringTransition = true;
        }
        else
        {
            disableTouchInputDuringTransition = false;
        }

        // Si las entradas táctiles están bloqueadas, no procesamos el movimiento táctil
        if (disableTouchInputDuringTransition)
            return;
        //PC
        // RaycastPC(); ACTIVAR LA FUNCION TAMBIÉN
        //Tablet
        if (UiButtons.Instance.TouchesEnabled() == true)
        {
            RaycastTablet();
        }
    }

    private void ShowHexLines(HexTile centerTile)
    {
        Vector2Int[] hexDirections = {
            new Vector2Int(1, 0), new Vector2Int(0, 1), new Vector2Int(-1, 1),
            new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, -1)
        };

        foreach (Vector2Int direction in hexDirections)
        {
            List<HexTile> lineTiles = hexGrid.GetHexLine(centerTile.axialCoords, direction);
            for (int i = 0; i < lineTiles.Count; i++)
            {
                if (i == lineTiles.Count - 1)
                {
                    lineTiles[i].HighlightTile(Color.red); // Last tile before an obstacle
                }
                else
                {
                    lineTiles[i].HighlightTile(Color.green); // Normal path
                }
                highlightedTiles.Add(lineTiles[i]);
            }
        }
    }

    


    private void ClearHighlights()
    {
        foreach (HexTile tile in highlightedTiles)
        {
            tile.ResetTileColor();
        }
        highlightedTiles.Clear();
    }

/*
    private void RaycastPC()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                HexTile clickedTile = hit.collider.GetComponent<HexTile>();
                if (clickedTile != null)
                {
                    ClearHighlights(); // Clear previous highlights
                    ShowHexLines(clickedTile);
                }
            }
        }

    }
*/

    private void RaycastTablet()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);  // Tomamos el primer toque

            // Solo procesamos el toque al comenzar (TouchPhase.Began)
            if (touch.phase == TouchPhase.Began)
            {
                // Convertimos las coordenadas del toque en un rayo
                Ray ray = mainCamera.ScreenPointToRay(touch.position);

                // Comprobamos si el rayo colisiona con algún objeto
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    HexTile clickedTile = hit.collider.GetComponent<HexTile>();
                    Debug.Log(clickedTile.axialCoords);
                    Unit unit = null;

                    if (clickedTile != null)
                    {
                        //ClearHighlights(); // Limpiar los resaltados previos
                        //ShowHexLines(clickedTile); // Mostrar líneas del hexágono
                                                   //Check if there's some unit in this hextile
                        unit= hexGrid.GetUnitInTile(clickedTile.axialCoords);
                        if (hexGrid.GetUnitIndex(unit) == 1) //Runner
                        {

                        }
                        else if (hexGrid.GetUnitIndex(unit) == 2) //TerraFormer
                        {
                            Debug.Log("Soy un terraformer");
                            terraFormTiles.Clear();
                            unit.OnSelected();
                            //hexGrid.GetHexTileTerraFormer(clickedTile.axialCoords);



                        }
}
                        else if (hexGrid.GetUnitIndex(unit) == 3) //Panchulina
                        {

                        }

                    }
                }
            }
        }

    }


