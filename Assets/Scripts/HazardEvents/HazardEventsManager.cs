using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
public class HazardEventsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Dictionary<int, Hazard> hazardDictionary = new Dictionary<int, Hazard>();
   
   
    public void ShuffleHazards()
    {
        List<Hazard> hazardList = new List<Hazard>
        {
            new HazardSequiaExtrema(),
            new HazardSequiaExtrema(),


        };

        // Mezclar la lista de hazards
        System.Random rng = new System.Random();
        List<Hazard> shuffledHazards = hazardList.OrderBy(h => rng.Next()).ToList();

        // Convertir la lista mezclada a un diccionario
        Dictionary<int, Hazard> hazardDictionary = shuffledHazards.ToDictionary(h => h.Id, h => h);

        // Mostrar el diccionario
        foreach (var kvp in hazardDictionary)
        {
            Console.WriteLine($"Key: {kvp.Key}, Value: {kvp.Value.Description}");
        }
    }

    public static Hazard GetHazardByKey(Dictionary<int, Hazard> hazardDictionary, int key)
    {
        // Intenta obtener el hazard usando TryGetValue
        if (hazardDictionary.TryGetValue(key, out Hazard hazard))
        {
            return hazard; // Retorna el hazard encontrado
        }
        else
        {
            return null; // Retorna null si no se encuentra el hazard
        }
    }


}
