using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;
public class HazardManager : MonoBehaviour
{
    /// Write those out ffrom here
    private int maxTurns;
    // int basicProbability = 25; //No se usa de momento lo comento
    
    private int currentProbability = 25;
    [SerializeField]
    private int maxProbability = 70;
    [SerializeField]
    private int basicAdditive = 5;
    private int currentAdditive = 5;
    [SerializeField]
    private int maxAdditive = 10;
    [SerializeField]
    private int additiveAdditive = 2;
    private int additiveUsed = 0;
    [SerializeField]
    private int startingEventHazardTurn = 2;
    [SerializeField]
    private int cooldownBetweenHazards  = 3;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private List<Hazard> hazardPool = new List<Hazard>();

    [Header("Tier Related")]
    [SerializeField]
    private bool useTierSystem;

    [SerializeField]
    [Tooltip("Max probability for T1 and T3. T1 happens at the beginning of the game, T3 happens at the end of the game")]
    private float tier1And3MaxProbability = 0.75f;
    [SerializeField]
    [Tooltip("Max probability for T2 Happens in the middle of the game")]
    private float tier2MaxProbability = 0.6f;
    [SerializeField]
    [Tooltip("Min probability for T2. Happens in the beggining and end of the game")]
    private float tier2MinProbability = 0.2f;


    private Dictionary<int, Hazard> HazardByTurn = new Dictionary<int, Hazard>();

    public static HazardManager Instance { get; private set; }
    private HexGrid hexGrid;

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

    private void Start()
    {
        ShuffleHazardPool();
        maxTurns = GameManager.Instance.LimitTurns;
        TurnAssignation();
        hexGrid = FindAnyObjectByType<HexGrid>();
    }
    public void ShuffleHazardPool()
    {
        System.Random rng = new System.Random();

        int n = hazardPool.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Hazard temp = hazardPool[k];
            hazardPool[k] = hazardPool[n];
            hazardPool[n] = temp;
        }
    }





    private void TurnAssignation()
    {
        int hazardPoolIndex = 0;
        int turn = startingEventHazardTurn;
        while (turn <= maxTurns){

            if (hazardPoolIndex >= hazardPool.Count)
                break;

            int rnd = Random.Range(1, 101);
            if (rnd <= currentProbability)
            {

                var hazard = hazardPool[hazardPoolIndex];
                HazardByTurn.Add(turn, hazard);
                currentAdditive = basicAdditive;
                Debug.Log($"<color=orange><b>On turn {turn}</b></color>, event <color=cyan><b>{hazard.name}</b></color> has been assigned.");
                turn += cooldownBetweenHazards + hazard.duration; // look into duration
                hazardPoolIndex++;

            }
            else
            {
                currentProbability += currentAdditive;
                additiveUsed++;
                if (currentProbability >= maxProbability)
                {
                    currentProbability = maxProbability;
                }

                if (additiveUsed >= 2)
                {
                    currentAdditive += additiveAdditive;
                    additiveUsed = 0;
                    additiveAdditive++;
                    if (currentAdditive >= maxAdditive)
                    {
                        currentAdditive = maxAdditive;
                    }
                }

                turn++;

            }

        }
        foreach (int key in HazardByTurn.Keys)
        {
            print(key + ".-"+ HazardByTurn[key]);
            
        }

    }
    public void LaunchHazardUI()
    {
        int currentTurn = GameManager.Instance.numericCurrentTurn;

        if (HazardByTurn.TryGetValue(currentTurn, out Hazard hazard))
        {
            hazard.LaunchUIHazard();
        }
        else
        {
            Debug.Log($"<color=grey>No hazard assigned for turn {currentTurn}.</color>");
        }
    }
    public void LaunchHazard(int currentTurn)
    {



        int tier = GetWeightedRandomNumber(
            currentTurn,
            maxTurns, // max number
            tier2MaxProbability, // max prob for 2
            tier2MinProbability, // min prob for 2
            tier1And3MaxProbability // max prob for 1 and 3
        );

        if (HazardByTurn.TryGetValue(currentTurn, out Hazard hazard))
        {
            Debug.Log($"<color=red><b>Turn {currentTurn}</b></color>: Launching <color=cyan>{hazard.name}</color> hazard.");

            hazard.ExecuteHazard(useTierSystem, tier);
            GameManager.Instance.hazardDurationLeft = hazard.duration;
        }
        else
        {
            Debug.Log($"<color=grey>No hazard assigned for turn {currentTurn}.</color>");
        }
    }

    public void ResetTemporaryTiles()
    {
        if (hexGrid.TemporaryInactiveTiles != null)
        {
            foreach (HexTile tile in hexGrid.TemporaryInactiveTiles)
            {
                //tile.TileRenderer.gameObject.SetActive(true);
                hexGrid.hexMap.Add(tile.axialCoords, tile);
                tile.SetState(HexState.Neutral);
            }
            hexGrid.TemporaryInactiveTiles.Clear();
        }
        
    }


    public static int GetWeightedRandomNumber(
        float currentNumber,
        float maxNumber,
        float maxProbFor2,
        float minProbFor2,
        float maxProbFor1And3)
    {
        // Clamp values to avoid accidental overshooting
        currentNumber = Mathf.Clamp(currentNumber, 0f, maxNumber);

        // Normalize current number to a 0-1 range
        float t = currentNumber / maxNumber;

        // Interpolate probability for number 2 (peaks at middle)
        float prob2 = Mathf.Lerp(minProbFor2, maxProbFor2, 1f - Mathf.Abs(t - 0.5f) * 2f);

        // Remaining probability to split between 1 and 3
        float remaining = 1f - prob2;

        // Interpolate max possible values for 1 and 3 based on position
        float prob1Max = Mathf.Lerp(maxProbFor1And3, 0f, t);
        float prob3Max = Mathf.Lerp(0f, maxProbFor1And3, t);

        float totalMax = prob1Max + prob3Max;

        // Actual probabilities for 1 and 3 scaled from remaining probability
        float prob1 = (prob1Max / totalMax) * remaining;
        float prob3 = (prob3Max / totalMax) * remaining;

        // Random roll
        float rand = Random.value;

        if (rand < prob1)
            return 1;
        else if (rand < prob1 + prob2)
            return 2;
        else
            return 3;
    }

}
