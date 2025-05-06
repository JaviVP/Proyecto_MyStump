using UnityEngine;

public class MaterialAdder : MonoBehaviour
{
    [Range(0f, 1f)]
    public float transparency = 0.0f; // Valor de transparencia (0 = invisible, 1 = opaco)

    private Renderer[] renderers;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();
       
    }

    public void SetTransparency(float value)
    {
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty("_Transparency"))
                {
                    mat.SetFloat("_Transparency", value); // Cambiar transparencia
                }

                // Si el shader usa _Color, actualizamos el alpha también
                if (mat.HasProperty("_Color"))
                {
                    Color col = mat.color;
                    col.a = value;
                    mat.color = col;
                }

                // Asegura que se aplique el modo blend de transparencia
                EnableAlphaBlending(mat);
            }
        }
    }

    private void EnableAlphaBlending(Material mat)
    {
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}