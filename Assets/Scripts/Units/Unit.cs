using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class Unit : MonoBehaviour
{

    private bool usedPreviusTurn;
    private Vector2Int axialCoords;
    private GameObject unitRenderer;
    private float speed;
    private GameManager.Team team;



    //private bool isMoving = false;

    public Vector2Int AxialCoords { get => axialCoords; set => axialCoords = value; }
    public GameObject UnitRenderer { get => unitRenderer; set => unitRenderer = value; }
    public bool UsedPreviusTurn { get => usedPreviusTurn; set => usedPreviusTurn = value; }
    public GameManager.Team Team { get => team; set => team = value; }

    public abstract bool Move(Vector2Int targetPosition);
    public abstract void OnSelected();

    public abstract void ClearHighlights();

    public void Update()
    {
        
        string[] partes = GetComponent<Unit>().ToString().Split("(");
        if (partes.Length > 0 )
        {
            if (partes[0].Contains("Panchulina"))
            {
                transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Pan";
            }
            else if (partes[0].Contains("Terra"))
            {
                transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Terra";
            }
            else
            {
                transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = "Runner";
            }
            //transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = partes[0];
            transform.GetChild(0).transform.LookAt(new Vector3(0,0,25.0f));
        }
    }










}
