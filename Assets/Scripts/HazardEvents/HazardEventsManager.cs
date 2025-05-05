using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class HazardEventsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Dictionary<int, EventHazard> hazardDictionary = new Dictionary<int, EventHazard>();

   

    private void Start()
    {
        //ShuffleHazards();
    }
    public void ShuffleHazards()
    {
        List<Hazard> hazardList = new List<Hazard>
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
        List<Hazard> shuffledHazards = hazardList.OrderBy(h => rng.Next()).ToList();
        int turnCounter = 2;
        int key = 0;
        foreach (Hazard h in shuffledHazards)
        {
            EventHazard eh = new EventHazard();
            eh.Hazard = h;
            eh.Turn = turnCounter;
            turnCounter+=2;
            key++;
            hazardDictionary.Add(key, eh);

        }
        // Mostrar el diccionario
        foreach (var kvp in hazardDictionary.Values)
        {
            Console.WriteLine("Value:"+ kvp.Hazard.Description);
        }
    }

    public void CheckHazardEvents(int turn)
    {
        Hazard h = GetHazardByTurn(turn);
        if (h!=null)
        {
            Debug.Log("Lanzo un Hazard ("+turn+")  Tipo: "+ h.GetType().ToString());
            
            h.Apply();
            
        }
    }

    public Hazard GetHazardByTurn(int turn)
    {
        Hazard h = null;
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


}
