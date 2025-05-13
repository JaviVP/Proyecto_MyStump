using UnityEngine;

public class EventHazard : MonoBehaviour
{

    private HazardOLD hazard;
    private int turn;
     
    public HazardOLD Hazard { get => hazard; set => hazard = value; }
    public int Turn { get => turn; set => turn = value; }
}
