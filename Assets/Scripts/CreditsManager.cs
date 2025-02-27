using UnityEngine;
using TMPro;
using System.Collections; 

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;  // Men� de ajustes
    [SerializeField] private GameObject creditsPanel;  // Panel donde aparecer�n los cr�ditos
    [SerializeField] private TextMeshProUGUI creditsText;  // Referencia al texto de los cr�ditos
    [SerializeField] private float fadeInDuration;  // Duraci�n del fade-in
    [SerializeField] private float fadeOutDuration;  // Duraci�n del fade-out
    [SerializeField] private float displayTime;  // Tiempo que los cr�ditos se mostrar�n
    [SerializeField] private float moveSpeed;  // Velocidad de desplazamiento hacia arriba

    private CanvasGroup creditsCanvasGroup;
    private RectTransform creditsRectTransform;

    void Start()
    {
        creditsCanvasGroup = creditsPanel.GetComponent<CanvasGroup>();
        creditsRectTransform = creditsPanel.GetComponent<RectTransform>();
        creditsPanel.SetActive(false);  // Aseg�rate de que el panel de cr�ditos est� oculto al inicio
    }

    public void ShowCredits()
    {
        // Ocultar el men� de ajustes
        settingsMenu.SetActive(false);

        // Activar el panel de cr�ditos
        creditsPanel.SetActive(true);

        // Restablecer la posici�n Y del panel de cr�ditos fuera de la pantalla
        creditsRectTransform.anchoredPosition = new Vector2(creditsRectTransform.anchoredPosition.x, -Screen.height);

        // Iniciar la coroutine para mostrar los cr�ditos
        StartCoroutine(ShowAndMoveCredits());
    }

    private IEnumerator ShowAndMoveCredits()
    {
        // Tiempo transcurrido para fade-in y desplazamiento
        float elapsedTime = 0f;

        // Mostrar cr�ditos (fade-in) y moverlos hacia arriba
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;

            // Fade-in (aumentando alpha de 0 a 1)
            creditsCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);

            // Mover los cr�ditos hacia arriba
            creditsRectTransform.anchoredPosition += new Vector2(0, moveSpeed * Time.deltaTime);

            yield return null;
        }

        // Los cr�ditos ahora son completamente visibles, mant�n el movimiento
        yield return new WaitForSeconds(displayTime);

        // Continuar moviendo y hacer fade-out
        StartCoroutine(FadeOutAndContinueMoving());
    }

    private IEnumerator FadeOutAndContinueMoving()
    {
        float elapsedTime = 0f;

        // Continuar moviendo los cr�ditos y hacer fade-out (de 1 a 0)
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;

            // Fade-out (disminuyendo alpha de 1 a 0)
            creditsCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeOutDuration));

            // Mover los cr�ditos hacia arriba
            creditsRectTransform.anchoredPosition += new Vector2(0, moveSpeed * Time.deltaTime);

            yield return null;
        }

        // Al terminar el fade-out, desactivar el panel de cr�ditos
        creditsPanel.SetActive(false);

        // Reactivar el men� de ajustes
        settingsMenu.SetActive(true);
    }
}
