using UnityEngine;

public class HexGameManager : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField]
    private HexGrid grid;

    public static HexGameManager Instance { get; private set; }

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

    void Start()
    {
        mainCamera = Camera.main;
        grid.CreateTerraMallaProve();
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
