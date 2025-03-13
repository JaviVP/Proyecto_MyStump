using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
public class GameManager : MonoBehaviour
{
    public enum Team { Ants, Termites }
    private CinemachineBrain brain;
    private Camera mainCamera;
    private HexGrid hexGrid;
    private List<HexTile> highlightedTiles = new List<HexTile>();



    public static GameManager Instance { get; private set; }
    public HexGrid HexGrid { get => hexGrid; set => hexGrid = value; }

    private Unit selectedUnit = null;

    /// 
    /// CAMBIAR ESTO LO DE ABAJO. NO ES LA MEJOT MANERA
    /// 
    private Team currentTurn = Team.Ants; // Start with Ants' turn
    /// 
    /// ESTA LINEA
    /// 

    ///
    /// Para que es lo de abajo???
    ///
    private HashSet<Unit> movedUnits = new HashSet<Unit>(); // Track units that have moved
    ///
    /// Comptobar mas tarde
    ///

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Nueva variable para bloquear las entradas táctiles durante la transición
    private bool disableTouchInputDuringTransition = false;
    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        mainCamera = Camera.main;
        HexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
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
        if (UiManager.Instance.TouchesEnabled() == true)
        {
            RaycastTablet();
        }
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
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


    /// 
    /// PASAR AL HEXGRID
    /// 


    

    /// 
    /// HASTA AQUI
    /// 


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
                    Unit clickedUnit = GameManager.Instance.HexGrid.GetUnitInTile(clickedTile.axialCoords);
                    Debug.Log(clickedTile.axialCoords);

                    if (GameManager.Instance.selectedUnit == null)
                    {
                        // Select the unit if it belongs to the current turn team
                        GameManager.Instance.SelectUnit(clickedUnit);
                        Debug.Log("click:" + clickedUnit);
                    }
                    else
                    {
                        // Move the selected unit
                        GameManager.Instance.MoveSelectedUnit(clickedTile.axialCoords);
                    }

                }
            }
        }
    }

    public void SelectUnit(Unit unit)
    {
        if (unit == null || movedUnits.Contains(unit) || unit.Team != currentTurn) return;

        selectedUnit = unit;
        selectedUnit.OnSelected();
        Debug.Log($"Selected {unit.GetType().Name} for {unit.Team}");
    }

    public void MoveSelectedUnit(Vector2Int targetPosition)
    {
        if (selectedUnit == null) return;

        if (selectedUnit.Move(targetPosition))
        {
            movedUnits.Add(selectedUnit); // Mark unit as moved

            CheckTurnEnd();
        }

        selectedUnit = null;

    }

    private void CheckTurnEnd()
    {
        // If all units have moved, switch turn
        if (movedUnits.Count >= 1) // Since each team has 1 units
        {
            movedUnits.Clear();
            currentTurn = (currentTurn == Team.Ants) ? Team.Termites : Team.Ants;
            Debug.Log($"Turn switched to {currentTurn}");
        }
    }

}


