using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;
public class HazardEventsManager : MonoBehaviour
{
    /// Write those out ffrom here
    private int maxTurns;
    // int basicProbability = 25; //No se usa de momento lo comento
    [SerializeField]
    private bool useTierSystem;
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

    private Dictionary<int, Hazard> HazardByTurn = new Dictionary<int, Hazard>();



    private void Start()
    {
        ShuffleHazardPool();
        maxTurns = GameManager.Instance.LimitTurns;
        TurnAssignation();
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
                turn += cooldownBetweenHazards + hazard.duration; // look into duration
                hazardPoolIndex++;
                Debug.Log($"<color=orange><b>On turn {turn}</b></color>, event <color=cyan><b>{hazard.name}</b></color> has been assigned.");

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

    }

}
