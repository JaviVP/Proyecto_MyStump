using UnityEngine;

public class EventHazard : MonoBehaviour
{

    private Hazard hazard;
    private int turn;
     
    public Hazard Hazard { get => hazard; set => hazard = value; }
    public int Turn { get => turn; set => turn = value; }
}
