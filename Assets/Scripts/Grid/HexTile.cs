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

    public Renderer TileRenderer { get => tileRenderer; set => tileRenderer = value; }



    //private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    private void Awake()
    {
       TileRenderer = GetComponent<Renderer>();         
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
        TileRenderer.material.color = c;
    }
    private void UpdateTileAppearance()
    {
      
        switch (state)
        {
            case HexState.Ants:
                TileRenderer.material = antTiles;
                break;
            case HexState.Termites:
                TileRenderer.material = termTiles;
                break;
            default:
                TileRenderer.material = baseMaterial;
                break;
        }
        
    }

    public void HighlightTile()
    {


        //tileRenderer.material = highlightMaterial; // Apply the highlight shader
        //highlightMaterial.SetColor(ColorProperty, new Color(1f, 1f, 0f, 1f)); // Set highlight color (yellow)
        var materials = TileRenderer.materials;
        Material[] newMaterials = new Material[materials.Length + 1];

        for (int i = 0; i < materials.Length; i++)
        {
            newMaterials[i] = materials[i];
        }
        newMaterials[newMaterials.Length - 1] = highlightMaterial;

        TileRenderer.materials = newMaterials;

    }

    public void ResetTileColor()
    {
        //tileRenderer.material = baseMaterial;
        var materials = TileRenderer.materials;
        if (materials.Length > 1)
        {
            TileRenderer.materials = new Material[] { materials[0] };
        }
       

        UpdateTileAppearance(); // Restore original tile material
    }
    
}
