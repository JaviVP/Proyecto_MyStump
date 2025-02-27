using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Color normalColor = Color.white;  // Color normal del texto
    [SerializeField] private Color pressedColor = Color.red;   // Color del texto cuando está presionado
    [SerializeField] private Color outlineColor = Color.black; // Color del contorno
    [SerializeField] private float outlineWidth = 2f;          // Ancho del contorno

    private TextMeshProUGUI buttonText;   // Referencia al texto del botón
    private Material buttonTextMaterial;  // Material del texto para manipular el contorno

    void Start()
    {
        // Intentar obtener el componente TextMeshProUGUI dentro del botón
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            // Obtener el material del texto
            buttonTextMaterial = buttonText.material;

            // Establecer el color inicial del texto
            buttonText.color = normalColor;

            // Inicializar el contorno del material (sin contorno por defecto)
            buttonTextMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);
            buttonTextMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);

            // Asegurarse de que el outline está habilitado
            buttonTextMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, 0f); // Hacer el contorno nítido (puedes ajustar esto)
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Cambiar el color y aplicar el contorno cuando se presiona el botón, solo si tiene texto
        if (buttonText != null)
        {
            buttonText.color = pressedColor;

            // Activar el contorno con el ancho configurado
            buttonTextMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, outlineWidth);  // Aplica el contorno
            buttonTextMaterial.SetColor(ShaderUtilities.ID_OutlineColor, outlineColor);  // Asegurarse de que el contorno tiene color
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Restaurar el color original y quitar el contorno al soltar el botón
        if (buttonText != null)
        {
            buttonText.color = normalColor;
            buttonTextMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0f);  // Eliminar contorno
        }
    }
}

