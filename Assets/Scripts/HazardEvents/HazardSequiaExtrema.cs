using UnityEngine;

public class HazardSequiaExtrema : Hazard
{
    public override void Apply()
    {
        //How it affects to the board.
        GameManager.Instance.HexGrid.GetRandomHexTile();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
