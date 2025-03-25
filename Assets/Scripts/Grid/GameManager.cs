using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using System.Runtime.InteropServices.WindowsRuntime;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    public enum Team { Ants, Termites }
    private CinemachineBrain brain;
    private Camera mainCamera;
    private HexGrid hexGrid;
    private List<HexTile> highlightedTiles = new List<HexTile>();

    //Limit of turns
    private float limitTurns;
    private int numberAntsTiles; //At the end of the match, number of ants tiles
    private int numberTermitesTiles; //At the end of the match, number of termites tiles
    private int totalTiles;  //total number of tiles in the grid
   
    public static GameManager Instance { get; private set; }
    public HexGrid HexGrid { get => hexGrid; set => hexGrid = value; }
    public float LimitTurns { get => limitTurns; set => limitTurns = value; }
    public Team CurrentTurn { get => currentTurn; set => currentTurn = value; }

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

        // DontDestroyOnLoad(gameObject);
    }

    // Nueva variable para bloquear las entradas táctiles durante la transición
    private bool disableTouchInputDuringTransition = false;
    void Start()
    {
        limitTurns = 20;
        brain = Camera.main.GetComponent<CinemachineBrain>();
        mainCamera = Camera.main;
        HexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
        //hexGrid.CreateTerraMallaProve(); 
        HexState result = CheckMoreColorTiles();
        //UiManager.Instance.UpdateUiTurn("Current Turn: " + currentTurn + "\nLimitTurns:" + limitTurns + "\nAnts Tiles: " + numberAntsTiles + "\nTermites Tiles:" + numberTermitesTiles + "\nTotal Tiles: " + totalTiles);
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

        UiManager.Instance.UpdateTiles();
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

                    HexTile clickedTile = null;
                    clickedTile = hit.collider.GetComponent<HexTile>();
                    if (clickedTile == null && hit.collider.transform.root.GetComponent<Unit>())
                    {
                        float x = hit.collider.transform.root.transform.position.x;
                        float z = hit.collider.transform.root.transform.position.z;
                        Vector2Int pos = hexGrid.WorldToAxial(x, z);
                        clickedTile = hexGrid.GetHexTile(pos);

                    }
                    if (clickedTile == null) return;
                    //Update selected Unit on the hexGrid
                    //HexTile clickedTile = hit.collider.GetComponent<HexTile>();


                    Unit clickedUnit = GameManager.Instance.HexGrid.GetUnitInTile(clickedTile.axialCoords);
                    //Debug.Log(clickedTile.axialCoords);

                    if (selectedUnit == null)
                    {
                        // Select the unit if it belongs to the current turn team
                        GameManager.Instance.SelectUnit(clickedUnit);
                        // Debug.Log("click:" + clickedUnit);
                    }
                    
                    else if (selectedUnit is UnitPanchulina)
                    {
                        UnitPanchulina up = (UnitPanchulina)selectedUnit;
                        //Debug.Log("Panchulina:" + up.FirstMove);
                        if (!up.FirstMove)
                        {
                            if (!selectedUnit.Move(clickedTile.axialCoords))
                            {
                                selectedUnit = null;
                            }
                        }
                        else
                        {
                            GameManager.Instance.MoveSelectedUnit(clickedTile.axialCoords);
                        }
                        //Debug.Log("hola caracola");
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
        if (unit == null || movedUnits.Contains(unit) || unit.Team != CurrentTurn)
        {
            return;
        }

        selectedUnit = unit;
        selectedUnit.OnSelected();
        //Debug.Log($"Selected {unit.GetType().Name} for {unit.Team}");
    }

    public void MoveSelectedUnit(Vector2Int targetPosition)
    {
        if (selectedUnit == null)
        {
            return;
        }

        //bool moveSuccess = selectedUnit.Move(targetPosition);

        if (selectedUnit.Move(targetPosition))
        {
            movedUnits.Add(selectedUnit); // ✅ Mark unit as moved

            // ✅ Only clear selection if it's NOT a Panchulinas OR if it has finished both moves
            if (!(selectedUnit is UnitPanchulina) || !((UnitPanchulina)selectedUnit).FirstMove)
            {
                
                CheckTurnEnd();
            }
        }
        if (!(selectedUnit is UnitPanchulina))
        {
            selectedUnit = null;
        }
        
    }



    private HexState CheckMoreColorTiles()
    {
        numberAntsTiles = hexGrid.GetCountStateTiles(HexState.Ants);
        totalTiles = hexGrid.totalNumberOfTiles() - hexGrid.CountNeutralTiles();
        numberTermitesTiles = totalTiles - numberAntsTiles;
        if (numberAntsTiles > numberTermitesTiles)
        {
            return HexState.Ants;
        }
        else if (numberAntsTiles < numberTermitesTiles)
        {
            return HexState.Termites;
        }
        else
        {
            return HexState.Neutral;
        }


    }
    private void CheckTurnEnd()
    {
        // If all units have moved, switch turn
        if (movedUnits.Count >= 1) // Since each team has 1 units
        {
            movedUnits.Clear();
            CurrentTurn = (CurrentTurn == Team.Ants) ? Team.Termites : Team.Ants;
            limitTurns--;
            UiManager.Instance.UpdateScroll();
            if (limitTurns <= 0)
            {
                string winner = "";
                HexState result = CheckMoreColorTiles();
                if (result == HexState.Neutral)
                {
                    winner = "Draw";
                }
                else
                {
                    winner = result.ToString();
                }

                endPanel.SetActive(true);
                UiManager.Instance.UpdateUiTurn("Result: " + winner.ToString() + " won");
                UiManager.Instance.TouchEnabled = false;
                //FIN DE PARTIDA
                //UiManager.Instance.UpdateUiTurn("Fin de partida\nGanador:" + winner.ToString() + "\nAnts Tiles: " + numberAntsTiles + "\nTermites Tiles:" + numberTermitesTiles + "\nTotal Tiles: " + totalTiles);


            }
            else
            {
                
                HexState result = CheckMoreColorTiles();
                Debug.Log($"Turn switched to {CurrentTurn}");
                //UiManager.Instance.UpdateUiTurn("Current Turn: " + currentTurn + "\nLimitTurns:" + limitTurns + "\nAnts Tiles: " + numberAntsTiles + "\nTermites Tiles:" + numberTermitesTiles + "\nTotal Tiles: " + totalTiles);

            }


        }
    }

    public int NumberOfAntTiles()
    {

        return numberAntsTiles;

    }

    public int NumberOfTermTiles()
    {

        return numberTermitesTiles;

    }



}


