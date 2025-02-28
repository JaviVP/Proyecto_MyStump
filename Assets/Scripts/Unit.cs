using Unity.VisualScripting;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{


    private Vector2Int axialCoords;
    private GameObject unitRenderer;
    private float speed;


    private bool isMoving = false;

    public Vector2Int AxialCoords { get => axialCoords; set => axialCoords = value; }
    public GameObject UnitRenderer { get => unitRenderer; set => unitRenderer = value; }

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
