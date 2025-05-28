using UnityEngine;
using UnityEngine.UIElements;


public enum HexState { Neutral, Ants, Termites }
public class HexTile : MonoBehaviour
{
    public Vector2Int axialCoords;
    public HexState state = HexState.Neutral;
    private Renderer tileRenderer;

    [SerializeField] private Material highlightMaterial; // Assign in Inspector
    [SerializeField] private Material[] baseMaterial;
    [SerializeField] private Material antTiles;
    [SerializeField] private Material termTiles;

    public Renderer TileRenderer { get => tileRenderer; set => tileRenderer = value; }
    private int pos;


    //private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    private void Awake()
    {
        float[] rotaciones = { 0f, 60f, 120f, 180f, 240f, 300f };
        int posRot = Random.Range(0, rotaciones.Length);
        this.gameObject.transform.rotation = Quaternion.Euler(0.0f, rotaciones[posRot], 0.0f);

        pos = Random.Range(0, baseMaterial.Length);
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
               
                TileRenderer.material = baseMaterial[pos];
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
