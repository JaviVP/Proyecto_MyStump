using UnityEngine;

public class HazardUI_Manager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LaunchHazard()
    {
        HazardManager.Instance.LaunchHazard(GameManager.Instance.numericCurrentTurn);
        HazardManager.Instance.hazardPanel.SetActive(false);
        UiManager.Instance.TouchEnabled = true;
        //UiManager.Instance.UpdateTiles();

    }
}
