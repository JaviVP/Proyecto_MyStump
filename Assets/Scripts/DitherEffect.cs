using UnityEngine;
using static GameManager;

public class DitherEffect : MonoBehaviour
{
    [Header("Configuraci�n Breath")]
    [SerializeField] private string shaderProperty = "_Brigthness"; // Par�metro correcto en el shader
    [SerializeField] private string shaderProperty2 = "_Dithering"; // Par�metro correcto en el shader
    [SerializeField] private float minValue = 1.1f;
    [SerializeField] private float maxValue = 1.8f;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Team myTeam; // Asignado a cada unidad en Inspector

    [Header("Referencias")]
    [SerializeField] private GameManager gameManager; // Asignado en Inspector o encontrado autom�ticamente

    [Header("Exclusiones")]
    [SerializeField] private Renderer[] excludedRenderers; // Renderers a ignorar

    private Renderer[] renderers;
    private float time;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    void Update()
    {
       

       
    }

    private void ApplyBrigthnessValue(float value)
    {
        foreach (Renderer rend in renderers)
        {
            if (IsExcluded(rend)) continue;

            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty(shaderProperty))
                {
                    mat.SetFloat(shaderProperty, value);
                }
            }
        }
    }

    private void ApplyDitherValue(float value)
    {
        foreach (Renderer rend in renderers)
        {
            if (IsExcluded(rend)) continue;

            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty(shaderProperty2))
                {
                    mat.SetFloat(shaderProperty2, value);
                }
            }
        }
    }

    private bool IsExcluded(Renderer rend)
    {
        foreach (Renderer excluded in excludedRenderers)
        {
            if (rend == excluded) return true;
        }
        return false;
    }
}