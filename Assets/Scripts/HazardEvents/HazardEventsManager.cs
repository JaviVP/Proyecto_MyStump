using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class HazardEventsManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private List<Hazard> hazardList= new List<Hazard>();
    //private List<Hazard> hazardList = new List<Hazard>();

    private void FillHazardList()
    {
        // Add different types of hazards to the list
        hazardList.Add(new HazardSequiaExtrema());
       
        // You can add more hazards as needed
        // hazardList.Add(new AnotherHazardType());
    }
    private void ShuffleHazardList()
    {
        for (int i = 0; i < hazardList.Count; i++)
        {
            int randomIndex = Random.Range(i, hazardList.Count);
            // Swap the current element with the random element
            Hazard temp = hazardList[i];
            hazardList[i] = hazardList[randomIndex];
            hazardList[randomIndex] = temp;
        }
    }

    private Hazard GetRandomHazard()
    {
        if (hazardList.Count > 0)
        {
            // Return the first hazard from the shuffled list
            return hazardList[0];
        }
        return null; // Return null if the list is empty
    }



}
