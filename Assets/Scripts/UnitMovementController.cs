using UnityEngine;

public class UnitMovementController : MonoBehaviour
{

    public static UnitMovementController Instance { get; private set; }



    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void MoveUnit(GameObject unit, Vector3 init, Vector3 end, float speed)
    {

        


    }
}
