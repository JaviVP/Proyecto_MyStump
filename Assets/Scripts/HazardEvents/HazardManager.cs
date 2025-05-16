using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = UnityEngine.Random;
public class HazardEventsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Dictionary<int, EventHazard> hazardDictionary = new Dictionary<int, EventHazard>();

    [SerializeField]
    private List<Hazard> hazardPool = new List<Hazard>();

    private Dictionary<int, List<Hazard>> HazardByTurn = new Dictionary<int, List<Hazard>>();



    private void Start()
    {
        ShuffleHazardPool();
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





    public void TurnAssignation()
    {
        /// Write those out ffrom here
        // int maxTurns = 40; //No se usa de momento lo comento
        // int basicProbability = 25; //No se usa de momento lo comento
        int currentProbability = 25;
        int maxProbability = 70;
        int basicAdditive = 5;
        int currentAdditive = 5;
        int maxAdditive = 10;
        int additiveAdditive = 2;
        int additiveUsed = 0;



        int rnd = Random.Range(1, 101);
        if (rnd <= currentProbability)
        {
            //save event turn position
            currentAdditive = basicAdditive;
        }
        else
        {
            currentProbability = currentProbability + currentAdditive;
            additiveUsed++;
            if (currentProbability >= maxProbability)
            {
                currentProbability = maxProbability;
            }

            if (additiveUsed >= 2)
            {
                currentAdditive = currentAdditive + additiveAdditive;
                additiveUsed = 0;
                additiveAdditive++;
                if (currentAdditive >= maxAdditive)
                {
                    currentAdditive = maxAdditive;
                }
            }
        }

    }

}
