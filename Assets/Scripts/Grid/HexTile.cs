using UnityEngine;

public enum HexState { Neutral, Ants, Termites }

public class HexTile : MonoBehaviour
{
    public Vector2Int axialCoords;
    public HexState state = HexState.Neutral;
    private Renderer tileRenderer;

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
        UpdateTileAppearance();
    }

    public void SetState(HexState newState)
    {
        state = newState;
        UpdateTileAppearance();
    }

    private void UpdateTileAppearance()
    {
        switch (state)
        {
            case HexState.Ants:
                tileRenderer.material.color = Color.red;
                break;
            case HexState.Termites:
                tileRenderer.material.color = Color.green;
                break;
            default:
                tileRenderer.material.color = Color.grey;
                break;
        }
    }
}
