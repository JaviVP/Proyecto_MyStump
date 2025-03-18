using UnityEngine;
using UnityEngine.UIElements;


public enum HexState { Neutral, Ants, Termites }
public class HexTile : MonoBehaviour
{
    public Vector2Int axialCoords;
    public HexState state = HexState.Neutral;
    private Renderer tileRenderer;

    [SerializeField] private Material highlightMaterial; // Assign in Inspector
    [SerializeField] private Material baseMaterial;
    [SerializeField] private Material antTiles;
    [SerializeField] private Material termTiles;



    //private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    private void Awake()
    {
        tileRenderer = GetComponent<Renderer>();
        UpdateTileAppearance();
        //baseMaterial = GetComponent<Material>();
    }

    public void SetState(HexState newState)
    {
        state = newState;
        UpdateTileAppearance();
    }
    public void ChangeColor(Color c)
    {
        tileRenderer.material.color = c;
    }
    private void UpdateTileAppearance()
    {
       

        switch (state)
        {
            case HexState.Ants:
                tileRenderer.material = antTiles;
                break;
            case HexState.Termites:
                tileRenderer.material = termTiles;
                break;
            default:
                tileRenderer.material = baseMaterial;
                break;
        }
        
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
