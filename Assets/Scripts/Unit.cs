using Unity.VisualScripting;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{


    public Vector2Int axialCoords;
    private Renderer unitRenderer;
    private float speed;


    private bool isMoving = false;

    public abstract void move();
    public abstract void push();


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    

    private void Update()
    {
        

    }
    private void Start()
    {
        
        
    }

}
