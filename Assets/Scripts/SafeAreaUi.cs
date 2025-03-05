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
        // Obtener el �rea segura en p�xeles
        Rect safeArea = Screen.safeArea;

        // Obtener las coordenadas de los bordes del �rea segura
        Vector2 safeAreaMin = safeArea.position;
        Vector2 safeAreaMax = safeArea.position + safeArea.size;

        // Obtener el tama�o del RectTransform (imagen)
        Vector2 imageSize = rectTransform.rect.size;

        // Obtener la posici�n actual del RectTransform (con los offsets ya establecidos)
        Vector2 imagePos = rectTransform.anchoredPosition;

        // Obtener los offsets (distancia al borde)
        Vector2 offsetMin = rectTransform.offsetMin;
        Vector2 offsetMax = rectTransform.offsetMax;

        // Verificamos si la imagen est� completamente dentro del �rea segura
        bool isInsideSafeArea = IsInsideSafeArea(imagePos, imageSize, safeAreaMin, safeAreaMax);

        // Si la imagen no est� dentro del �rea segura, ajustamos la posici�n respetando los offsets
        if (!isInsideSafeArea)
        {
            Vector2 newPos = imagePos;

            // Ajustar la posici�n X
            if (imagePos.x - imageSize.x / 2 + offsetMin.x < safeAreaMin.x)
            {
                newPos.x = safeAreaMin.x + imageSize.x / 2 - offsetMin.x;
            }
            else if (imagePos.x + imageSize.x / 2 + offsetMax.x > safeAreaMax.x)
            {
                newPos.x = safeAreaMax.x - imageSize.x / 2 - offsetMax.x;
            }

            // Ajustar la posici�n Y
            if (imagePos.y - imageSize.y / 2 + offsetMin.y < safeAreaMin.y)
            {
                newPos.y = safeAreaMin.y + imageSize.y / 2 - offsetMin.y;
            }
            else if (imagePos.y + imageSize.y / 2 + offsetMax.y > safeAreaMax.y)
            {
                newPos.y = safeAreaMax.y - imageSize.y / 2 - offsetMax.y;
            }

            // Asegurarnos de que la posici�n de la imagen se mantenga dentro del Canvas
            Vector2 canvasSize = rectTransform.parent.GetComponent<RectTransform>().rect.size;
            newPos.x = Mathf.Clamp(newPos.x, 0, canvasSize.x - imageSize.x);
            newPos.y = Mathf.Clamp(newPos.y, 0, canvasSize.y - imageSize.y);

            // Aplicamos la nueva posici�n
            rectTransform.anchoredPosition = newPos;
        }
    }

    // Verificar si la imagen est� completamente dentro del �rea segura
    bool IsInsideSafeArea(Vector2 imagePos, Vector2 imageSize, Vector2 safeAreaMin, Vector2 safeAreaMax)
    {
        // Las esquinas de la imagen (usamos su tama�o y posici�n)
        float left = imagePos.x - imageSize.x / 2 + rectTransform.offsetMin.x;
        float right = imagePos.x + imageSize.x / 2 + rectTransform.offsetMax.x;
        float bottom = imagePos.y - imageSize.y / 2 + rectTransform.offsetMin.y;
        float top = imagePos.y + imageSize.y / 2 + rectTransform.offsetMax.y;

        // Verificamos si la imagen est� completamente dentro del �rea segura
        return left >= safeAreaMin.x && right <= safeAreaMax.x && bottom >= safeAreaMin.y && top <= safeAreaMax.y;
    }
}
