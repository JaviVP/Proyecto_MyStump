using UnityEngine;

public class DitheringPeanas : MonoBehaviour
{
    [Header("Shader Property")]
    [SerializeField] private string shaderProperty = "_DitherSize";

    [Header("Renderer del hijo")]
    [SerializeField] private Renderer targetRenderer;

    [Header("Índice del material a modificar")]
    [SerializeField] private int materialIndex = 0;

    void Start()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }

        // Asegurarse de instanciar los materiales si están compartidos
        if (targetRenderer != null)
        {
            Material[] materials = targetRenderer.materials; // esto ya instancia una copia local
            targetRenderer.materials = materials; // aseguramos que esté desvinculado del sharedMaterial
        }
    }

    public void ApplyDitherValueInd(float value)
    {
        if (targetRenderer == null) return;

        Material[] materials = targetRenderer.materials;

        if (materialIndex >= 0 && materialIndex < materials.Length)
        {
            Material mat = materials[materialIndex];

            if (mat.HasProperty(shaderProperty))
            {
                mat.SetFloat(shaderProperty, value);
            }
        }
    }
}
