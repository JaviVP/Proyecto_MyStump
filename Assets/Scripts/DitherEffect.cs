using UnityEngine;
using static GameManager;

public class DitherEffect : MonoBehaviour
{
    [Header("Configuración Breath")]
    [SerializeField] private string shaderProperty = "_Dithering"; // Parámetro correcto en el shader
    [SerializeField] private float minValue = 1.1f;
    [SerializeField] private float maxValue = 1.8f;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Team myTeam; // Asignado a cada unidad en Inspector

    [Header("Referencias")]
    [SerializeField] private GameManager gameManager; // Asignado en Inspector o encontrado automáticamente

    private Renderer[] renderers;
    private float time;

    void Start()
    {
        renderers = GetComponentsInChildren<Renderer>();

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }

    void Update()
    {
        if (gameManager == null) return;

        if (gameManager.CurrentTurn != myTeam)
        {
            // Breath effect solo cuando NO es su turno
            time += Time.deltaTime * speed;

            float range = (maxValue - minValue) / 2f;
            float midpoint = (maxValue + minValue) / 2f;
            float value = Mathf.Sin(time) * range + midpoint;

            ApplyDitherValue(value);
        }
        else
        {
            // Valor fijo cuando es su turno
            ApplyDitherValue(2.0f); // Default value activo
        }
    }

    private void ApplyDitherValue(float value)
    {
        foreach (Renderer rend in renderers)
        {
            foreach (Material mat in rend.materials)
            {
                if (mat.HasProperty(shaderProperty))
                {
                    mat.SetFloat(shaderProperty, value);
                }
            }
        }
    }
}
