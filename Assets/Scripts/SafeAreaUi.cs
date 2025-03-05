using UnityEngine;
using UnityEngine.UI;
public class SafeAreaUi : MonoBehaviour
{
    private Image image;
    private RectTransform rectTransform;

    void Start()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        AdjustPositionToSafeArea();
    }

    void AdjustPositionToSafeArea()
    {
        // Obtener el área segura en píxeles
        Rect safeArea = Screen.safeArea;

        // Obtener las coordenadas de los bordes del área segura
        Vector2 safeAreaMin = safeArea.position;
        Vector2 safeAreaMax = safeArea.position + safeArea.size;

        // Obtener el tamaño del RectTransform (imagen)
        Vector2 imageSize = rectTransform.rect.size;

        // Obtener la posición actual del RectTransform (con los offsets ya establecidos)
        Vector2 imagePos = rectTransform.anchoredPosition;

        // Obtener los offsets (distancia al borde)
        Vector2 offsetMin = rectTransform.offsetMin;
        Vector2 offsetMax = rectTransform.offsetMax;

        // Verificamos si la imagen está completamente dentro del área segura
        bool isInsideSafeArea = IsInsideSafeArea(imagePos, imageSize, safeAreaMin, safeAreaMax);

        // Si la imagen no está dentro del área segura, ajustamos la posición respetando los offsets
        if (!isInsideSafeArea)
        {
            Vector2 newPos = imagePos;

            // Ajustar la posición X
            if (imagePos.x - imageSize.x / 2 + offsetMin.x < safeAreaMin.x)
            {
                newPos.x = safeAreaMin.x + imageSize.x / 2 - offsetMin.x;
            }
            else if (imagePos.x + imageSize.x / 2 + offsetMax.x > safeAreaMax.x)
            {
                newPos.x = safeAreaMax.x - imageSize.x / 2 - offsetMax.x;
            }

            // Ajustar la posición Y
            if (imagePos.y - imageSize.y / 2 + offsetMin.y < safeAreaMin.y)
            {
                newPos.y = safeAreaMin.y + imageSize.y / 2 - offsetMin.y;
            }
            else if (imagePos.y + imageSize.y / 2 + offsetMax.y > safeAreaMax.y)
            {
                newPos.y = safeAreaMax.y - imageSize.y / 2 - offsetMax.y;
            }

            // Asegurarnos de que la posición de la imagen se mantenga dentro del Canvas
            Vector2 canvasSize = rectTransform.parent.GetComponent<RectTransform>().rect.size;
            newPos.x = Mathf.Clamp(newPos.x, 0, canvasSize.x - imageSize.x);
            newPos.y = Mathf.Clamp(newPos.y, 0, canvasSize.y - imageSize.y);

            // Aplicamos la nueva posición
            rectTransform.anchoredPosition = newPos;
        }
    }

    // Verificar si la imagen está completamente dentro del área segura
    bool IsInsideSafeArea(Vector2 imagePos, Vector2 imageSize, Vector2 safeAreaMin, Vector2 safeAreaMax)
    {
        // Las esquinas de la imagen (usamos su tamaño y posición)
        float left = imagePos.x - imageSize.x / 2 + rectTransform.offsetMin.x;
        float right = imagePos.x + imageSize.x / 2 + rectTransform.offsetMax.x;
        float bottom = imagePos.y - imageSize.y / 2 + rectTransform.offsetMin.y;
        float top = imagePos.y + imageSize.y / 2 + rectTransform.offsetMax.y;

        // Verificamos si la imagen está completamente dentro del área segura
        return left >= safeAreaMin.x && right <= safeAreaMax.x && bottom >= safeAreaMin.y && top <= safeAreaMax.y;
    }
}
