using UnityEngine;
using TMPro;
using System.Collections;

public class CreditsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenu;  // Menú de ajustes
    [SerializeField] private GameObject creditsPanel;  // Panel donde aparecerán los créditos
    [SerializeField] private TextMeshProUGUI creditsText;  // Referencia al texto de los créditos
    [SerializeField] private float fadeInDuration;  // Duración del fade-in
    [SerializeField] private float fadeOutDuration;  // Duración del fade-out
    [SerializeField] private float displayTime;  // Tiempo que los créditos se mostrarán
    [SerializeField] private float moveSpeed;  // Velocidad de desplazamiento hacia arriba
    [SerializeField] private float fastMoveSpeedMultiplier = 2f;  // Factor multiplicador para mover más rápido cuando se mantiene presionado

    private CanvasGroup creditsCanvasGroup;
    private RectTransform creditsRectTransform;
    private bool isTouching = false;  // Para controlar si el usuario está manteniendo presionado

    void Start()
    {
        creditsCanvasGroup = creditsPanel.GetComponent<CanvasGroup>();
        creditsRectTransform = creditsPanel.GetComponent<RectTransform>();
        creditsPanel.SetActive(false);  // Asegúrate de que el panel de créditos esté oculto al inicio
    }

    void Update()
    {
        // Detectar si el jugador mantiene presionada la pantalla
        if (Input.touchCount > 0)  // Para dispositivos táctiles
        {
            isTouching = true;
        }
        else if (Input.GetMouseButton(0))  // Para PC (clic del ratón)
        {
            isTouching = true;
        }
        else
        {
            isTouching = false;
        }
    }

    public void ShowCredits()
    {
        // Ocultar el menú de ajustes
        settingsMenu.SetActive(false);

        // Activar el panel de créditos
        creditsPanel.SetActive(true);

        // Restablecer la posición Y del panel de créditos fuera de la pantalla
        creditsRectTransform.anchoredPosition = new Vector2(creditsRectTransform.anchoredPosition.x, -Screen.height);

        // Iniciar la coroutine para mostrar los créditos
        StartCoroutine(ShowAndMoveCredits());
    }

    private IEnumerator ShowAndMoveCredits()
    {
        // Tiempo transcurrido para fade-in y desplazamiento
        float elapsedTime = 0f;

        // Mostrar créditos (fade-in) y moverlos hacia arriba
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;

            // Fade-in (aumentando alpha de 0 a 1)
            creditsCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);

            // Mover los créditos hacia arriba, ajustando la velocidad si se está tocando la pantalla
            float currentMoveSpeed = isTouching ? moveSpeed * fastMoveSpeedMultiplier : moveSpeed;
            creditsRectTransform.anchoredPosition += new Vector2(0, currentMoveSpeed * Time.deltaTime);

            yield return null;
        }

        // Los créditos ahora son completamente visibles, mantén el movimiento
        yield return new WaitForSeconds(displayTime);

        // Continuar moviendo y hacer fade-out
        StartCoroutine(FadeOutAndContinueMoving());
    }

    private IEnumerator FadeOutAndContinueMoving()
    {
        float elapsedTime = 0f;

        // Continuar moviendo los créditos y hacer fade-out (de 1 a 0)
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;

            // Fade-out (disminuyendo alpha de 1 a 0)
            creditsCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeOutDuration));

            // Mover los créditos hacia arriba, ajustando la velocidad si se está tocando la pantalla
            float currentMoveSpeed = isTouching ? moveSpeed * fastMoveSpeedMultiplier : moveSpeed;
            creditsRectTransform.anchoredPosition += new Vector2(0, currentMoveSpeed * Time.deltaTime);

            yield return null;
        }

        // Al terminar el fade-out, desactivar el panel de créditos
        creditsPanel.SetActive(false);

        // Reactivar el menú de ajustes
        settingsMenu.SetActive(true);
    }
}