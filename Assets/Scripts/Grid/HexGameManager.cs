using UnityEngine;

public class HexGameManager : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField]
    private HexGrid grid;


    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                HexTile clickedTile = hit.collider.GetComponent<HexTile>();
                if (clickedTile != null)
                {
                    grid.ClearHexGrid();
                    clickedTile.SetState(HexState.Ants); // Debug: Capture for Ants
                }
            }
        }
    }
}
