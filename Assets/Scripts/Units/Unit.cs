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
    public int TurnsUntilAvailable { get; private set; } = 0;



    public abstract bool Move(Vector2Int targetPosition);
    public abstract void OnSelected();

    public abstract void ClearHighlights();

    
    public bool IsAvailableThisTurn()
    {
        return TurnsUntilAvailable <= 0;
    }

    public void MarkAsUsed()
    {
        TurnsUntilAvailable = 2;
        //SetCooldownVisual(true); 
    }


    public void ReduceCooldown()
    {
        if (TurnsUntilAvailable > 0)
        {
            TurnsUntilAvailable--;

            if (TurnsUntilAvailable == 0)
            {
                SetCooldownVisual(false); 
            }
        }
    }


    public virtual void SetCooldownVisual(bool isOnCooldown)
    {
        if (UnitRenderer == null) return;

        if (isOnCooldown)
        {
            UnitRenderer.transform.rotation = Quaternion.Euler(-90f, UnitRenderer.transform.rotation.eulerAngles.y, 0f);
            Vector3 pos = transform.position;
            pos.y += 0.5f;
            //pos.x += 0.8f;
            transform.position = pos;
        }

        else
        {
            UnitRenderer.transform.rotation = Quaternion.Euler(0f, UnitRenderer.transform.rotation.eulerAngles.y, 0f);
            Vector3 pos = transform.position;
            pos.y = 0.1f; // or whatever your normal Y height is
            //pos.x = 0f;
            transform.position = pos;
        }
    }





    public void Update()
    {
        
        string[] partes = GetComponent<Unit>().ToString().Split("(");
        if (partes.Length > 0 && transform.childCount > 0)
        {
            TMP_Text label = transform.GetChild(0).GetComponent<TMP_Text>();

            if (label != null)
            {
                if (partes[0].Contains("Panchulina"))
                {
                    label.text = "Pan";
                }
                else if (partes[0].Contains("Terra"))
                {
                    label.text = "Terra";
                }
                else if (partes[0].Contains("Runner"))
                {
                    label.text = "Runner";
                }
                else
                {
                    // Do nothing or optionally clear the text
                    // label.text = "";
                }

                // Optional: only LookAt if we changed the text
                label.transform.LookAt(new Vector3(0, 0, 25.0f));
            }
        }

    }










}
