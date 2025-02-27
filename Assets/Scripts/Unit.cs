using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{


    private Vector3 init;
    private Vector3 end;
    private float speed;
    private float distance;

    private bool move = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void MoveUnit(Vector3 init, Vector3 end, float speed)
    {
        this.init = init;
        this.end = end;
        this.speed = speed;
        move = true;
    }

    private void Update()
    {
        if (move)
        {

            this.gameObject.transform.position = Vector3.Lerp(transform.position, end, speed * Time.deltaTime);
            distance = Vector3.Distance(init, end);
            if (distance <= 0.1f)
            {
                move = false;
            }

        }

    }
    private void Start()
    {
        
        MoveUnit(transform.position, new Vector3(10,transform.position.y,0),1.0f);
    }

}
