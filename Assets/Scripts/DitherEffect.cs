using UnityEngine;
using static GameManager;

public class DitherEffect : MonoBehaviour
{
    [Header("Configuración Breath")]
    [SerializeField] private string shaderProperty = "_Brigthness"; // Parámetro correcto en el shader
    [SerializeField] private string shaderProperty2 = "_Dithering"; // Parámetro correcto en el shader
    [SerializeField] private float minValue = 1.1f;
    [SerializeField] private float maxValue = 1.8f;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Team myTeam; // Asignado a cada unidad en Inspector

    [Header("Referencias")]
    [SerializeField] private GameManager gameManager; // Asignado en Inspector o encontrado automáticamente

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
        if (gameManager == null) return;

        if (gameManager.CurrentTurn != myTeam && GameManager.Instance.DraftActive() != true)
        {
            // Breath effect solo cuando NO es su turno
            time += Time.deltaTime * speed;

            float range = (maxValue - minValue) / 2f;
            float midpoint = (maxValue + minValue) / 2f;
            float value = Mathf.Sin(time) * range + midpoint;

            ApplyBrigthnessValue(value);
        }
        else
        {
            // Valor fijo cuando es su turno
            ApplyBrigthnessValue(0.09f); // Default value activo
        }
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

    public void ApplyDitherValue(float value)
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