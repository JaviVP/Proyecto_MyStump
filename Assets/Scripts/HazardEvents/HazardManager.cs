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

    Dictionary<int, List<Hazard>> hazardByTurn = new Dictionary<int, List<Hazard>>();


    private Dictionary<int, List<Hazard>> HazardByTurn = new Dictionary<int, List<Hazard>>();



    private void Start()
    {
        ShuffleHazardPool();
    }
    public void ShuffleHazardPool()
    {

        List<HazardOLD> hazardList = new List<HazardOLD>
        {
            new HazardCarreteraFantasma(),
            new HazardCarreteraFantasma()
            /*new HazardSequiaExtrema(),
            new HazardDerramePetroleo(),
            new HazardDisputaPorElCaucho(),
            new HazardElPulsoDelRio(),
            new HazardExpansionUrbana(),
            new HazardMineriaMercurio(),
            new HazardTormentaDelTropico()*/
        };

        // Mezclar la lista de hazards
        System.Random rng = new System.Random();
        List<HazardOLD> shuffledHazards = hazardList.OrderBy(h => rng.Next()).ToList();
        int turnCounter = 2;
        int key = 0;
        foreach (HazardOLD h in shuffledHazards)
        {
            EventHazard eh = new EventHazard();
            eh.Hazard = h;
            eh.Turn = turnCounter;
            turnCounter+=2;
            key++;
            hazardDictionary.Add(key, eh);

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


    public void CheckHazardEvents(int turn)
    {
        HazardOLD h = GetHazardByTurn(turn);
        if (h!=null)
        {
            Debug.Log("Lanzo un Hazard ("+turn+")  Tipo: "+ h.GetType().ToString());
            
            h.Apply();
            
        }
    }

    public HazardOLD GetHazardByTurn(int turn)
    {
        HazardOLD h = null;
        foreach (EventHazard kvp in hazardDictionary.Values)
        {
            if (kvp.Turn ==turn)
            {
                return kvp.Hazard;
            }
            //Console.WriteLine("Value:" + kvp.Hazard.Description);
        }
        return h;
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
