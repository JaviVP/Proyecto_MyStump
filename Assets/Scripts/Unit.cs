using Unity.VisualScripting;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{

    private bool usedPreviusTurn;
    private Vector2Int axialCoords;
    private GameObject unitRenderer;
    private float speed;


    private bool isMoving = false;

    public Vector2Int AxialCoords { get => axialCoords; set => axialCoords = value; }
    public GameObject UnitRenderer { get => unitRenderer; set => unitRenderer = value; }
    public bool UsedPreviusTurn { get => usedPreviusTurn; set => usedPreviusTurn = value; }

    public abstract void Move();
    public abstract void OnSelected();
    public void CaptureTiles()
    {
        //implementation 
    }

    


   

}
