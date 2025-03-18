using UnityEngine;


public enum HexState { Neutral, Ants, Termites }
public class HexTile : MonoBehaviour
{
    public Vector2Int axialCoords;
    public HexState state = HexState.Neutral;
    private Renderer tileRenderer;

    [SerializeField] private Material highlightMaterial; // Assign in Inspector
    [SerializeField] private Material baseMaterial;

    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
        UpdateTileAppearance();
        baseMaterial = GetComponent<Material>();
    }

    public void SetState(HexState newState)
    {
        state = newState;
        UpdateTileAppearance();
    }

    private void UpdateTileAppearance()
    {
        Color tileColor;
        switch (state)
        {
            case HexState.Ants:
                tileColor = Color.red;
                break;
            case HexState.Termites:
                tileColor = Color.green;
                break;
            default:
                tileColor = Color.grey;
                break;
        }
        tileRenderer.material.color = tileColor;
    }

    public void HighlightTile()
    {


        //tileRenderer.material = highlightMaterial; // Apply the highlight shader
        //highlightMaterial.SetColor(ColorProperty, new Color(1f, 1f, 0f, 1f)); // Set highlight color (yellow)
        var materials = tileRenderer.materials;
        Material[] newMaterials = new Material[materials.Length + 1];

        for (int i = 0; i < materials.Length; i++)
        {
            newMaterials[i] = materials[i];
        }
        newMaterials[newMaterials.Length - 1] = highlightMaterial;

        tileRenderer.materials = newMaterials;

    }

    public void ResetTileColor()
    {
        //tileRenderer.material = baseMaterial;
        var materials = tileRenderer.materials;
        if (materials.Length > 1)
        {
            tileRenderer.materials = new Material[] { materials[0] };
        }
       

        UpdateTileAppearance(); // Restore original tile material
    }
}
