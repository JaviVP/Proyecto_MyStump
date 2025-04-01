using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using System.Runtime.InteropServices.WindowsRuntime;
using System;
using static GameManager;


public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject inputPanel;

    [SerializeField] private float animationSpeed;

    public enum Team { Ants, Termites }
    private CinemachineBrain brain;
    private Camera mainCamera;
    private HexGrid hexGrid;
    private List<HexTile> highlightedTiles = new List<HexTile>();
    private bool lockTiles = false;
    private int partidasSeleccionadas;

    //Limit of turns
    [SerializeField] private float limitTurns;
    private int numberAntsTiles; //At the end of the match, number of ants tiles
    private int numberTermitesTiles; //At the end of the match, number of termites tiles
    private int totalTiles;  //total number of tiles in the grid
    private string winner = "";
    public static GameManager Instance { get; private set; }
    public HexGrid HexGrid { get => hexGrid; set => hexGrid = value; }
    public float LimitTurns { get => limitTurns; set => limitTurns = value; }
    public Team CurrentTurn { get => currentTurn; set => currentTurn = value; }
    public bool DisableTouchInputDuringTransition { get => disableTouchInputDuringTransition; set => disableTouchInputDuringTransition = value; }
    public bool LockTiles { get => lockTiles; set => lockTiles = value; }
    public float AnimationSpeed { get => animationSpeed; set => animationSpeed = value; }

    private Unit selectedUnit = null;

    /// 
    /// CAMBIAR ESTO LO DE ABAJO. NO ES LA MEJOT MANERA
    /// 
    private Team currentTurn = Team.Termites; // Start with Ants' turn
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
       
        brain = Camera.main.GetComponent<CinemachineBrain>();
        mainCamera = Camera.main;
        HexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
        //hexGrid.CreateTerraMallaProve(); 
        HexState result = CheckMoreColorTiles();
        hexGrid.SelectTeam(Team.Ants);
        //UiManager.Instance.UpdateUiTurn("Current Turn: " + currentTurn + "\nLimitTurns:" + limitTurns + "\nAnts Tiles: " + numberAntsTiles + "\nTermites Tiles:" + numberTermitesTiles + "\nTotal Tiles: " + totalTiles);
        hexGrid.RemoveTile(new Vector2Int(0, 0));
    }

    void Update()
    {
        //Debug.Log(Screen.safeArea);
        // Comprobamos si está en transición (si es true, desactivamos las entradas táctiles)
        if (brain.IsBlending)
        {
            DisableTouchInputDuringTransition = true;
        }
        else
        {
            DisableTouchInputDuringTransition = false;
        }

        // Si las entradas táctiles están bloqueadas, no procesamos el movimiento táctil
        if (DisableTouchInputDuringTransition)
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


    private Vector2 touchStartPos; // Stores where the touch began
    private bool isDragging = false; // Detects if the player is dragging
    private bool gameOver;

    private void RaycastTablet()
    {
        if (Input.touchCount > 0 && !lockTiles)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    touchStartPos = touch.position;
                    isDragging = false; // Reset dragging flag
                    break;

                case TouchPhase.Moved:
                    if (Vector2.Distance(touch.position, touchStartPos) > 10f) // 10 pixels threshold
                    {
                        isDragging = true; // Set dragging flag if moved significantly
                    }
                    break;

                case TouchPhase.Ended:
                    if (isDragging) return; // If dragging occurred, ignore selection

                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        HexTile clickedTile = hit.collider.GetComponent<HexTile>();

                        // If no tile was clicked, check if a unit was clicked
                        if (clickedTile == null)
                        {
                            Unit clickedUnit = hit.collider.GetComponentInParent<Unit>();
                            if (clickedUnit != null)
                            {
                                clickedTile = hexGrid.GetHexTile(clickedUnit.AxialCoords);
                            }
                        }

                        if (clickedTile == null) return;

                        Unit clickedUnitOnTile = GameManager.Instance.HexGrid.GetUnitInTile(clickedTile.axialCoords);

                        if (selectedUnit == null)
                        {
                            GameManager.Instance.SelectUnit(clickedUnitOnTile);
                        }
                        else if (selectedUnit is UnitPanchulina panchulinas)
                        {
                            if (!panchulinas.FirstMove)
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
                        }
                        else
                        {
                            //Error code
                            
                            if (clickedTile != null)
                            {
                                GameManager.Instance.MoveSelectedUnit(clickedTile.axialCoords);
                            }
                            
                        }
                    }
                    break;
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
                selectedUnit = null;
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


            hexGrid.SelectTeam(CurrentTurn);
            hexGrid.CheckDestroyUnity(CurrentTurn);
            /// Lo de arriba es lo mismo
            /// que lo de abajo pero
            /// mas organizado. Si algo
            /// no funciona comentar lo 
            /// de arriba y descomentar
            /// lo de abajo
            /// 

            /*
            if (CurrentTurn == Team.Ants)
            {
                hexGrid.SelectTeam(Team.Ants);
               

                hexGrid.CheckDestroyUnity(Team.Ants);
            }
            else if (CurrentTurn == Team.Termites)
            {

                hexGrid.SelectTeam(Team.Termites);
                hexGrid.CheckDestroyUnity(Team.Termites);


            }
            
            */

            limitTurns--;
            UiManager.Instance.UpdateScroll();
            if(PlayerPrefs.GetInt("ModoCampeonato") == 1) {
                partidasSeleccionadas = PlayerPrefs.GetInt("NumeroPartidasCampeonato");
                if (limitTurns <= 0 && partidasSeleccionadas != 0 )
            {
                
                HexState result = CheckMoreColorTiles();
                if (result == HexState.Neutral)
                {
                    winner = "Draw";
                    UiManager.Instance.UpdateUiTurn("Result: " + winner.ToString());
                        partidasSeleccionadas--;
                        PlayerPrefs.SetInt("NumeroPartidasCampeonato",partidasSeleccionadas);
                        Debug.Log(PlayerPrefs.GetInt("NumeroPartidasCampeonato"));
                        endPanel.SetActive(true);
                    }
                else
                {
                    winner = result.ToString();
                    inputPanel.SetActive(true);
                    UiManager.Instance.UpdateUiTurn("Result: " + winner.ToString() + " won");
                        partidasSeleccionadas--;
                        PlayerPrefs.SetInt("NumeroPartidasCampeonato", partidasSeleccionadas);
                        Debug.Log(PlayerPrefs.GetInt("NumeroPartidasCampeonato"));
                        endPanel.SetActive(true);
                    }
                    if (partidasSeleccionadas == 0)
                    {
                        PlayerPrefs.SetInt("ModoCampeonato", 0);
                        

                        UiManager.Instance.TouchEnabled = false;
                        //FIN DE PARTIDA
                        //UiManager.Instance.UpdateUiTurn("Fin de partida\nGanador:" + winner.ToString() + "\nAnts Tiles: " + numberAntsTiles + "\nTermites Tiles:" + numberTermitesTiles + "\nTotal Tiles: " + totalTiles);
                    }

            }
            else
            {
                
                HexState result = CheckMoreColorTiles();
                Debug.Log($"Turn switched to {CurrentTurn}");
                //UiManager.Instance.UpdateUiTurn("Current Turn: " + currentTurn + "\nLimitTurns:" + limitTurns + "\nAnts Tiles: " + numberAntsTiles + "\nTermites Tiles:" + numberTermitesTiles + "\nTotal Tiles: " + totalTiles);

            }

            }
            else if (PlayerPrefs.GetInt("ModoCampeonato") == 0)
            {
                if (gameOver) return; // Stop further execution if game ended

                if (limitTurns <= 0 || PlayerPrefs.GetInt("AntCount") == 1 || PlayerPrefs.GetInt("TermCount") == 1)
                {
                    if (PlayerPrefs.GetInt("AntCount") == 1) { winner = "Termites"; }
                    else if (PlayerPrefs.GetInt("TermCount") == 1) { winner = "Ants"; }
                    HexState result = CheckMoreColorTiles();

                    if (result == HexState.Neutral)
                    {
                        winner = "Draw";
                        UiManager.Instance.UpdateUiTurn("Result: " + winner.ToString());
                       
                    }
                    else
                    {
                        winner = result.ToString();
                        inputPanel.SetActive(true);
                        UiManager.Instance.UpdateUiTurn("Result: " + winner.ToString() + " won");
               

                    }        
                        endPanel.SetActive(true);

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
    }

    /// Actualizar para mas tarde
    /// Declarar Victoria
    public void DeclareWinner(Team winner)
    {
        gameOver = true;
        Debug.Log($"Game Over! {winner} wins!");

        inputPanel.SetActive(true);
        endPanel.SetActive(true);
        UiManager.Instance.UpdateUiTurn($"Result: {winner} won");
        UiManager.Instance.TouchEnabled = false;
    }



    public string Winner()
    {

        return winner;
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


