﻿using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using System;
using static GameManager;
using System.Linq;
using NUnit.Framework;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{



    [SerializeField] private GameObject endPanel;
    [SerializeField] private GameObject inputPanel;
    [SerializeField] private List<TextMeshProUGUI> player1Texts;
    [SerializeField] private List<TextMeshProUGUI> player2Texts;
    [SerializeField] private TextMeshProUGUI winnerName;
    [SerializeField] private GameObject roundsText;
    [SerializeField] private LogoController logoController;
    [SerializeField] private float animationSpeed;
    [SerializeField] private Animator TurnRotationUi;
    
        
public enum Team { Ants, Termites }
    
    private CinemachineBrain brain;
    private Camera mainCamera;
    private HexGrid hexGrid;
    private List<HexTile> highlightedTiles = new List<HexTile>();
    private bool lockTiles = false;

    public int totalPartidasCampeonato { get; private set; }
    public int partidasSeleccionadas
    {
        get { return PlayerPrefs.GetInt("NumeroRondasCampeonato", totalPartidasCampeonato); }
        set { PlayerPrefs.SetInt("NumeroRondasCampeonato", value); }
    }
    private string player1;
    private string player2;
    private int player1RoundsWon;
    private int player2RoundsWon;
    private int neededWins;
    private bool puedeColocarPieza = true;
    //Update Stats variables
    //======================================
    //- Hacer get del Pref actual
    //- Igualar el total con la variable local
    //- Sumar el total con total mas el pref actual (tot += actual)
    //- Hacer set del total en el Pref
    //======================================
    private int actualGamesWon1;
    private int actualGamesWon2;
    private int totalGamesWon1;
    private int totalGamesWon2;
    private int actualAntTiles1;
    private int actualAntTiles2;
    private int totalAntTiles1;
    private int totalAntTiles2;
    private int actualTermTiles1;
    private int actualTermTiles2;
    private int totalTermTiles1;
    private int totalTermTiles2;
    private int actualAntsKilled1;
    private int actualAntsKilled2;
    private int totalAntsKilled1;
    private int totalAntsKilled2;
    private int actualTermsKilled1;
    private int actualTermsKilled2;
    private int totalTermsKilled1;
    private int totalTermsKilled2;
    //=====================================


    //Limit of turns
    [SerializeField] private int limitTurns;
    public int numericCurrentTurn = 1;
    private int numberAntsTiles; //At the end of the match, number of ants tiles
    private int numberTermitesTiles; //At the end of the match, number of termites tiles
    private int totalTiles;  //total number of tiles in the grid
    private string winner = "";
    public static GameManager Instance { get; private set; }
    public HexGrid HexGrid { get => hexGrid; set => hexGrid = value; }
    public int LimitTurns { get => limitTurns; set => limitTurns = value; }
    public Team CurrentTurn { get => currentTurn; set => currentTurn = value; }
    public bool DisableTouchInputDuringTransition { get => disableTouchInputDuringTransition; set => disableTouchInputDuringTransition = value; }
    public bool LockTiles { get => lockTiles; set => lockTiles = value; }
    public float AnimationSpeed { get => animationSpeed; set => animationSpeed = value; }

    private Unit selectedUnit = null;


    [Header("Draft Unit Placement Config")]
    public int numRunners = 2;
    public int numTerraformers = 2;
    public int numPanchulinas = 2;

    public List<UnitType> unitDraftList = new List<UnitType>();
    private int draftUnitIndex;
    public bool isDraftPhase;

    bool selected = false;
    HexTile previousClickTile;
    public int hazardDurationLeft = 0;

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
    public HashSet<Unit> movedUnits = new HashSet<Unit>(); // Track units that have moved
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
        draftUnitIndex = 0;
        isDraftPhase = true;
        totalPartidasCampeonato = PlayerPrefs.GetInt("NumeroPartidasCampeonato");
        SoundManager.instance.PlaySoundFromCategory("Ambient");
        if (!PlayerPrefs.HasKey("NumeroRondasCampeonato"))
        {
            partidasSeleccionadas = totalPartidasCampeonato;
        }

        player1 = PlayerPrefs.GetString("PlayerName1", "Jugador 1");
        player2 = PlayerPrefs.GetString("PlayerName2", "Jugador 2");
        PlayerPrefs.SetInt("TermsKilled", 0);
        PlayerPrefs.SetInt("AntsKilled", 0);
        UpdateRoundsWonText();
       // Debug.Log(PlayerPrefs.GetInt($"RondasGanadas_{player1}"));
        //Debug.Log(PlayerPrefs.GetInt($"RondasGanadas_{player2}"));


        brain = Camera.main.GetComponent<CinemachineBrain>();
        mainCamera = Camera.main;
        HexGrid = FindAnyObjectByType<HexGrid>(); // Get reference to HexGrid
        //hexGrid.CreateTerraMallaProve(); 
        HexState result = CheckMoreColorTiles();
        hexGrid.SelectTeam(Team.Ants);
        //UiManager.Instance.UpdateUiTurn("Current Turn: " + currentTurn + "\nLimitTurns:" + limitTurns + "\nAnts Tiles: " + numberAntsTiles + "\nTermites Tiles:" + numberTermitesTiles + "\nTotal Tiles: " + totalTiles);
        
        
        //hexGrid.RemoveTile(new Vector2Int(0, 0));
        GenerateUnitDraftList();
        FindAnyObjectByType<LogoController>().AsignarSpritesPorTipo(unitDraftList);
        
        /// Probablemente seria mejor hacer un metodo para iniciar DRAFT
        hexGrid.ResetTeamHalfHighlights();
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
            HandleInput();
        }
        if (currentTurn == Team.Ants)
        {
            TurnRotationUi.SetInteger("Turn", 0);
        }
        else
        {
            TurnRotationUi.SetInteger("Turn", 1);
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




    private Vector2 startPos;
    private bool isDragging = false;
    private bool gameOver;

    private void HandleInput()
    {
        // ---- PC (Mouse) ----
        if (Input.mousePresent && !lockTiles)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPos = Input.mousePosition;
                isDragging = false;
            }

            if (Input.GetMouseButton(0))
            {
                if (Vector2.Distance(Input.mousePosition, startPos) > 10f)
                    isDragging = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isDragging) return;

                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    HexTile clickedTile = hit.collider.GetComponent<HexTile>();

                    if (clickedTile == null)
                    {
                        Unit clickedUnit = hit.collider.GetComponentInParent<Unit>();
                        if (clickedUnit != null)
                            clickedTile = hexGrid.GetHexTile(clickedUnit.AxialCoords);
                    }

                    if (clickedTile != null)
                        ProcessTileClick(clickedTile);
                }
            }
        }
        // ---- Tablet (Touch) ----
        else if (Input.touchCount > 0 && !lockTiles)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startPos = touch.position;
                    isDragging = false;
                    break;

                case TouchPhase.Moved:
                    if (Vector2.Distance(touch.position, startPos) > 10f)
                        isDragging = true;
                    break;

                case TouchPhase.Ended:
                    if (isDragging) return;

                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        HexTile clickedTile = hit.collider.GetComponent<HexTile>();

                        if (clickedTile == null)
                        {
                            Unit clickedUnit = hit.collider.GetComponentInParent<Unit>();
                            if (clickedUnit != null)
                                clickedTile = hexGrid.GetHexTile(clickedUnit.AxialCoords);
                        }

                        if (clickedTile != null)
                            ProcessTileClick(clickedTile);
                    }
                    break;
            }
        }
    }

   
    

    private void ProcessTileClick(HexTile clickedTile)
    {
        if (isDraftPhase)
        {

            Unit unitOnTile = hexGrid.GetUnitInTile(clickedTile.axialCoords);

            if (currentTurn == Team.Ants)
            {

                if (unitOnTile == null && hexGrid.antDraftTiles.Contains(clickedTile))
                {
                    hexGrid.SpawnUnit(clickedTile.axialCoords, unitDraftList[draftUnitIndex], CurrentTurn, HexGrid.EnumHelper.ConvertToHexState(currentTurn));
                    hexGrid.RemoveAllHighlights();
                    //hexGrid.HighlightOne(clickedTile);
                    //selected = true;
                    //previousClickTile = clickedTile;
                    CurrentTurn = (CurrentTurn == Team.Ants) ? Team.Termites : Team.Ants;
                    selected = false;
                    hexGrid.ResetTeamHalfHighlights();
                    draftUnitIndex++;
                    SoundManager.instance.PlaySound("PlaceUnit");
                    logoController.ColocarPieza();
                    //return;
                }
            }
            else if (currentTurn == Team.Termites)
            {
              
                if (unitOnTile == null && hexGrid.termiteDraftTiles.Contains(clickedTile))
                {
                    hexGrid.SpawnUnit(clickedTile.axialCoords, unitDraftList[draftUnitIndex], CurrentTurn, HexGrid.EnumHelper.ConvertToHexState(currentTurn));
                    hexGrid.RemoveAllHighlights();
                    //hexGrid.HighlightOne(clickedTile);
                    //selected = true;
                    //previousClickTile = clickedTile;
                    CurrentTurn = (CurrentTurn == Team.Ants) ? Team.Termites : Team.Ants;
                    selected = false;
                    hexGrid.ResetTeamHalfHighlights();
                    //draftUnitIndex++;
                    SoundManager.instance.PlaySound("PlaceUnit");
                    logoController.ColocarPieza();
                    //return;
                }


            }

            if (draftUnitIndex >= unitDraftList.Count)
            {
                isDraftPhase = false;
                hexGrid.RemoveAllHighlights();
                //hexGrid.ResetTeamHalfHighlights();
            }
            return;

        }
        else
        {
            Unit clickedUnitOnTile = GameManager.Instance.HexGrid.GetUnitInTile(clickedTile.axialCoords);

            if (selectedUnit == null)
            {

                if (clickedUnitOnTile!=null && clickedUnitOnTile.Team == CurrentTurn)
                {
                    clickedUnitOnTile.SelectedSound();
                }
                GameManager.Instance.SelectUnit(clickedUnitOnTile);
            }
            else if (selectedUnit is UnitPanchulina panchulinas)
            {
                if (!panchulinas.FirstMove)
                {
                    if (!selectedUnit.Move(clickedTile.axialCoords))
                    {
                        selectedUnit = null;
                        SoundManager.instance.PlaySound("UnSelectedUnit");
                    }
                }
                else
                {
                    SoundManager.instance.PlaySound("PlaceUnit");
                    GameManager.Instance.MoveSelectedUnit(clickedTile.axialCoords);
                }
            }
            else
            {
                SoundManager.instance.PlaySound("UnSelectedUnit");
                GameManager.Instance.MoveSelectedUnit(clickedTile.axialCoords);
            }
        }
    }

    public void SelectUnit(Unit unit)
    {
        if (unit == null || !unit.IsAvailableThisTurn() || unit.Team != CurrentTurn)
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
                selectedUnit.MarkAsUsed(); // ✅ Apply cooldown
                //movedUnits.Add(selectedUnit);
                //CheckTurnEnd();
                
                selectedUnit = null;
            }
        }
        if (!(selectedUnit is UnitPanchulina))
        {
            selectedUnit = null;
        }

    }

    private void RefreshUnitsForTeam(Team team)
    {
        foreach (var unit in hexGrid.GetAllUnits())
        {
            if (unit.Team == team)
            {
                unit.ReduceCooldown(); // ✅ Refresh cooldown
            }
        }
    }

    public void GenerateUnitDraftList()
    {
        
        for (int i = 0; i < numRunners; i++)
            unitDraftList.Add(UnitType.Runner);

        for (int i = 0; i < numTerraformers; i++)
            unitDraftList.Add(UnitType.Terraformer);

        for (int i = 0; i < numPanchulinas; i++)
            unitDraftList.Add(UnitType.Panchulina);

        // Failsafe: ensure at least one Runner if all are zero
        if (unitDraftList.Count == 0)
        {
            Debug.LogWarning("No units selected in inspector. Adding one Runner as default.");
            unitDraftList.Add(UnitType.Runner);
        }

        System.Random rng = new System.Random();

        int n = unitDraftList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            UnitType value = unitDraftList[k];
            unitDraftList[k] = unitDraftList[n];
            unitDraftList[n] = value;
        }

        Debug.Log("Shuffled list:");
        foreach (var unit in unitDraftList)
        {
            Debug.Log(unit.ToString());
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
    public void CheckTurnEnd()
    {
        // If all units have moved, switch turn
        Debug.Log("--3---"+ movedUnits.Count);

        if (movedUnits.Count >= 1) // Since each team has 1 units
        {
            Debug.Log("--4");
            movedUnits.Clear();
            CurrentTurn = (CurrentTurn == Team.Ants) ? Team.Termites : Team.Ants;

            if (!AnyUnitCanMove(currentTurn))
            {
                DeclareWinner((CurrentTurn == Team.Ants) ? Team.Termites : Team.Ants);
            }


            Debug.Log("Current: " + currentTurn);
            //this.GetComponent<HazardEventsManager>().CheckHazardEvents((int) numericCurrentTurn);
            RefreshUnitsForTeam(CurrentTurn);

            hexGrid.SelectTeam(CurrentTurn);
            hexGrid.CheckDestroyUnity(CurrentTurn);

            /// Lo de arriba es lo mismo
            /// que lo de abajo pero
            /// mas organizado. Si algo
            /// no funciona comentar lo 
            /// de arriba y descomentar
            /// lo de abajo
            /// 


            
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
            
            
            numericCurrentTurn++;
            limitTurns--;

            //HazardManager.Instance.LaunchHazard(numericCurrentTurn);
            HazardManager.Instance.LaunchHazardUI();
            hazardDurationLeft--;
            if (hazardDurationLeft < 0)
            {
                HazardManager.Instance.ResetTemporaryTiles();
            }


            UiManager.Instance.UpdateScroll();
            if (PlayerPrefs.GetInt("ModoCampeonato") == 1)
            {

                WinCondition();
                if (limitTurns <= 0 && partidasSeleccionadas != 0)
                {
                  
                    HexState result = CheckMoreColorTiles();
                    if (result == HexState.Neutral)
                    {
                        winner = "Draw";
                        UiManager.Instance.UpdateUiTurn("Result: " + winner.ToString());
                        PlayerPrefs.SetInt("NumeroRondasCampeonato", partidasSeleccionadas);
                        Debug.Log(PlayerPrefs.GetInt("NumeroRondasCampeonato"));
                        CheckRounds();
                      
                        if (gameOver == false) { endPanel.SetActive(true); } else if (gameOver == true) { endPanel.SetActive(false); }
                        if (inputPanel.activeSelf || endPanel.activeSelf)
                        {
                            roundsText.SetActive(false);
                        }
                        else
                        {
                            roundsText.SetActive(true);
                        }
                    }
                    else
                    {
                        winner = result.ToString();
                        UiManager.Instance.UpdateUiTurn("Result: " + winner.ToString() + " won");
                        partidasSeleccionadas--;
                        PlayerPrefs.SetInt("NumeroRondasCampeonato", partidasSeleccionadas);
                        AddWins();
                        Debug.Log(PlayerPrefs.GetInt("NumeroRondasCampeonato"));
                        CheckRounds();
                        
                        if (gameOver == false) { endPanel.SetActive(true); } else if (gameOver == true) { endPanel.SetActive(false); }
                        if (inputPanel.activeSelf || endPanel.activeSelf)
                        {
                            roundsText.SetActive(false);
                        }
                        else
                        {
                            roundsText.SetActive(true);
                        }
                    }

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
            else if (PlayerPrefs.GetInt("ModoCampeonato") == 0)
            {
                if (gameOver) return; // Stop if already over

                // ✅ FIRST: Check if base has been destroyed (Term or Ant count = 1)
                if (PlayerPrefs.GetInt("AntCount") == 1)
                {
                    winner = "Termites";
                }
                else if (PlayerPrefs.GetInt("TermCount") == 1)
                {
                    winner = "Ants";
                }

                // ✅ If a winner was found through base kill, stop here
                if (!string.IsNullOrEmpty(winner))
                {
                    inputPanel.SetActive(true);
                    UiManager.Instance.UpdateUiTurn("Result: " + winner + " won");
                    endPanel.SetActive(true);
                    UiManager.Instance.TouchEnabled = false;
                    gameOver = true;
                    return;
                }

                // ✅ SECOND: Check turn limit condition + tile control
                if (limitTurns <= 0)
                {
                    HexState result = CheckMoreColorTiles();

                    if (result == HexState.Neutral)
                    {
                        winner = "Draw";
                        UiManager.Instance.UpdateUiTurn("Result: Draw");
                    }
                    else
                    {
                        winner = result.ToString();
                        UiManager.Instance.UpdateUiTurn("Result: " + winner + " won");
                        inputPanel.SetActive(true);

                    }

                    endPanel.SetActive(true);
                    UiManager.Instance.TouchEnabled = false;
                    gameOver = true;
                }
                else
                {
                    // Game continues – just update info or debug
                    HexState result = CheckMoreColorTiles();
                    Debug.Log($"Turn switched to {CurrentTurn}");
                    // Optional: Update turn info here
                }
            }

        }
    }

    public bool AnyUnitCanMove(Team team)
    {
        List<Unit> units = hexGrid.GetUnitsByTeam(team);
        foreach (var unit in units)
        {
            if (unit.CanMove())
            {
                return true;
            }
        }
        return false;
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

    public bool DraftActive()
    {

        return isDraftPhase;

    }

    public int NumberOfTermTiles()
    {

        return numberTermitesTiles;

    }
    private void CheckRounds()
    {
        neededWins = totalPartidasCampeonato / 2 + 1;
        if (player1RoundsWon >= neededWins)
        {
            Debug.Log(player1 + " has won championship");
            winnerName.text = player1.ToString();
            PlayerPrefs.SetInt("ModoCampeonato", 0);
            inputPanel.SetActive(true);
            UiManager.Instance.TouchEnabled = false;
            gameOver = true;
        }
        else if (player2RoundsWon >= neededWins)
        {
            Debug.Log(player2 + " has won championship");
            winnerName.text = player2.ToString();
            PlayerPrefs.SetInt("ModoCampeonato", 0);
            inputPanel.SetActive(true);
            UiManager.Instance.TouchEnabled = false;
            gameOver = true;
        }
        else if (player1RoundsWon >= neededWins && player2RoundsWon >= neededWins)
        {
            Debug.Log("Result = Draw");
            PlayerPrefs.SetInt("ModoCampeonato", 0);
            inputPanel.SetActive(true);
            UiManager.Instance.TouchEnabled = false;
            gameOver = true;
        }
    }

    private void AddWins()
    {

        if (winner == "Termites" || PlayerPrefs.GetInt("AntCount") == 1)
        {
            player1RoundsWon = PlayerPrefs.GetInt($"RondasGanadas_{player1}");
            player1RoundsWon++;
            PlayerPrefs.SetInt($"RondasGanadas_{player1}", player1RoundsWon);
            PlayerPrefs.SetInt($"PartidasGanadas_{player1}", PlayerPrefs.GetInt($"PartidasGanadas_{player1}") + 1);
            Debug.Log(player1 + " Won " + player1RoundsWon + " Rounds");
            if (inputPanel.activeSelf || endPanel.activeSelf)
            {
                roundsText.SetActive(false);
            }
            else
            {
                roundsText.SetActive(true);
            }
            foreach (var txt in player1Texts) txt.text = player1RoundsWon.ToString();

        }
        else if (winner == "Ants" || PlayerPrefs.GetInt("TermCount") == 1)
        {
            player2RoundsWon = PlayerPrefs.GetInt($"RondasGanadas_{player2}");
            player2RoundsWon++;
            PlayerPrefs.SetInt($"RondasGanadas_{player2}", player2RoundsWon);
            PlayerPrefs.SetInt($"PartidasGanadas_{player2}", PlayerPrefs.GetInt($"PartidasGanadas_{player2}") + 1);
            Debug.Log(player2 + " Won " + player2RoundsWon + " Rounds");
            if (inputPanel.activeSelf || endPanel.activeSelf)
            {
                roundsText.SetActive(false);
            }
            else
            {
                roundsText.SetActive(true);
            }
            foreach (var txt in player2Texts) txt.text = player2RoundsWon.ToString();
        }
        else
        {
            
        }

    }
    private void WinCondition()
    {
        if (PlayerPrefs.GetInt("AntCount") == 1) { winner = "Termites"; AddWins(); CheckRounds();
            if (inputPanel.activeSelf || endPanel.activeSelf)
            {
                roundsText.SetActive(false);
            }
            else
            {
                roundsText.SetActive(true);
            }
        }
        else if (PlayerPrefs.GetInt("TermCount") == 1) { winner = "Ants"; AddWins(); CheckRounds();
            if (inputPanel.activeSelf || endPanel.activeSelf)
            {
                roundsText.SetActive(false);
            }
            else
            {
                roundsText.SetActive(true);
            }
        }
        if (!string.IsNullOrEmpty(winner))
        {
           
            UiManager.Instance.UpdateUiTurn("Result: " + winner + " won");
            if (gameOver == false) { endPanel.SetActive(true); } else if (gameOver == true) { endPanel.SetActive(false); }
            UiManager.Instance.TouchEnabled = false;
            gameOver = true;
          
            return;
        }
    }

   
    public void UpdatePlayerStats()
    {
        TilesAndKills();
    }

    private void TilesAndKills()
    {
        if (winner == "Termites" || winner == "Ants" || winner == "Draw")
        {
            // === PLAYER 1 ===
            // Parcela conquistada esta ronda
            actualTermTiles1 = numberTermitesTiles;

            // Cargar total anterior y sumar lo nuevo
            totalTermTiles1 = PlayerPrefs.GetInt($"ActualTermTiles_{player1}", 0);
            totalTermTiles1 += actualTermTiles1;
            PlayerPrefs.SetInt($"ActualTermTiles_{player1}", totalTermTiles1);

            // Sumar al total de parcelas conquistadas
            int parcelasTermitas = PlayerPrefs.GetInt($"ParcelasTermitas_{player1}", 0);
            parcelasTermitas += actualTermTiles1;
            PlayerPrefs.SetInt($"ParcelasTermitas_{player1}", parcelasTermitas);

            // Hormigas eliminadas esta ronda
            actualAntsKilled1 = PlayerPrefs.GetInt("AntsKilled", 0);
            totalAntsKilled1 = PlayerPrefs.GetInt($"HormigasEliminadas_{player1}", 0);
            totalAntsKilled1 += actualAntsKilled1;
            PlayerPrefs.SetInt($"HormigasEliminadas_{player1}", totalAntsKilled1);

            // === PLAYER 2 ===
            actualAntTiles2 = numberAntsTiles;

            totalAntTiles2 = PlayerPrefs.GetInt($"ActualAntTiles_{player2}", 0);
            totalAntTiles2 += actualAntTiles2;
            PlayerPrefs.SetInt($"ActualAntTiles_{player2}", totalAntTiles2);

            int parcelasHormigas = PlayerPrefs.GetInt($"ParcelasHormigas_{player2}", 0);
            parcelasHormigas += actualAntTiles2;
            PlayerPrefs.SetInt($"ParcelasHormigas_{player2}", parcelasHormigas);

            actualTermsKilled2 = PlayerPrefs.GetInt("TermsKilled", 0);
            totalTermsKilled2 = PlayerPrefs.GetInt($"TermitasEliminadas_{player2}", 0);
            totalTermsKilled2 += actualTermsKilled2;
            PlayerPrefs.SetInt($"TermitasEliminadas_{player2}", totalTermsKilled2);

        }

    }

    public void GetPlayerStats()
    {
        Debug.Log(player1 + "Stats: ");
        Debug.Log(PlayerPrefs.GetInt($"PartidasGanadas_{player1}"));
        Debug.Log(PlayerPrefs.GetInt($"HormigasEliminadas_{player1}"));
        Debug.Log(PlayerPrefs.GetInt($"TermitasEliminadas_{player1}"));
        Debug.Log(PlayerPrefs.GetInt($"ParcelasHormigas_{player1}"));
        Debug.Log(PlayerPrefs.GetInt($"ParcelasTermitas_{player1}"));
        Debug.Log(player2 + "Stats: ");
        Debug.Log(PlayerPrefs.GetInt($"PartidasGanadas_{player2}"));
        Debug.Log(PlayerPrefs.GetInt($"HormigasEliminadas_{player2}"));
        Debug.Log(PlayerPrefs.GetInt($"TermitasEliminadas_{player2}"));
        Debug.Log(PlayerPrefs.GetInt($"ParcelasHormigas_{player2}"));
        Debug.Log(PlayerPrefs.GetInt($"ParcelasTermitas_{player2}"));



    }
    private IEnumerator EsperarUIYDevolverControl()
    {
        // Aquí deberías esperar el tiempo que tarda la animación de la UI, por ejemplo:
        float tiempoEsperar = 1.5f;  // O el tiempo que dure la animación de la UI y la colocación física
        yield return new WaitForSeconds(tiempoEsperar);

        puedeColocarPieza = true;  // Rehabilitamos el clic después de esperar
    }

    private void UpdateRoundsWonText()
    {
        player1RoundsWon = PlayerPrefs.GetInt($"RondasGanadas_{player1}");
        player2RoundsWon = PlayerPrefs.GetInt($"RondasGanadas_{player2}");

        foreach (var txt in player1Texts) txt.text = player1RoundsWon.ToString();
        foreach (var txt in player2Texts) txt.text = player2RoundsWon.ToString();


    }
   

}



