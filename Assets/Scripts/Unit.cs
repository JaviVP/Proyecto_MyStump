using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{

    private Vector3 init;
    private Vector3 end;
    private float speed;

    private bool move = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void MoveUnit(Vector3 init, Vector3 end, float speed)
    {
        this.init = init;
        this.end = end;
        this.speed = speed;


    }

}
