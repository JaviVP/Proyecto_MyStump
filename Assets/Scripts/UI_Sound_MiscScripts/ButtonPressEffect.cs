using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Color normalColor = new Color32(227, 255, 209, 255);  // Color normal del texto
    [SerializeField] private Color pressedColor = new Color32(254, 209, 255, 255); // Color del texto cuando está presionado


    private TextMeshProUGUI buttonText;   // Referencia al texto del botón

    void Start()
    {
        // Intentar obtener el componente TextMeshProUGUI dentro del botón
        buttonText = GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            // Establecer el color inicial del texto
            buttonText.color = normalColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Cambiar el color y aplicar el contorno cuando se presiona el botón, solo si tiene texto
        if (buttonText != null)
        {
            buttonText.color = pressedColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Restaurar el color original y quitar el contorno al soltar el botón
        if (buttonText != null)
        {
            buttonText.color = normalColor;
        }
    }
}