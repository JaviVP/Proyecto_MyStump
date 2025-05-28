using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("References")]
    [SerializeField] private RectTransform viewportTransform;  // El RectTransform del Viewport
    [SerializeField] private RectTransform contentTransform;   // El RectTransform del Content

    [Header("Scroll Settings")]
    [SerializeField] private float baseMoveSpeed = 100f;
    [SerializeField] private float fastScrollMultiplier = 2f;

    private Vector2 startPosition;
    

    public void ShowCredits()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(true);

        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform); // Asegura medidas actualizadas

        float contentHeight = contentTransform.rect.height;
        float viewportHeight = viewportTransform.rect.height;

        // Posición inicial: completamente fuera del viewport (abajo)
        startPosition = new Vector2(0, -contentHeight);
        contentTransform.anchoredPosition = startPosition;

        StartCoroutine(ScrollCredits(contentHeight, viewportHeight));
    }

    private IEnumerator ScrollCredits(float contentHeight, float viewportHeight)
    {
      

        while (contentTransform.anchoredPosition.y < contentHeight + viewportHeight)
        {
            float currentSpeed = baseMoveSpeed;

            // Detectar si se mantiene pulsado (mouse o touch)
            if (Input.GetMouseButton(0) || Input.touchCount > 0)
            {
                currentSpeed *= fastScrollMultiplier;
            }

            contentTransform.anchoredPosition += new Vector2(0, currentSpeed * Time.deltaTime);
            yield return null;
        }

      
        EndCredits();
    }

    private void EndCredits()
    {
        creditsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }
}