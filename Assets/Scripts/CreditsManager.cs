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
    [SerializeField] private float fastMoveSpeedMultiplier = 2f;  // Factor multiplicador para mover m�s r�pido cuando se mantiene presionado
    [SerializeField] private float heightOffset;  // Factor multiplicador para mover m�s r�pido cuando se mantiene presionado

    private CanvasGroup creditsCanvasGroup;
    private RectTransform creditsRectTransform;
    private bool isTouching = false;  // Para controlar si el usuario est� manteniendo presionado

    public static CreditsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        creditsCanvasGroup = creditsPanel.GetComponent<CanvasGroup>();
        creditsRectTransform = creditsPanel.GetComponent<RectTransform>();
        creditsPanel.SetActive(false);  // Aseg�rate de que el panel de cr�ditos est� oculto al inicio
    }

    void Update()
    {
        // Detectar si el jugador mantiene presionada la pantalla
        if (Input.touchCount > 0)  // Para dispositivos t�ctiles
        {
            isTouching = true;
        }
        else if (Input.GetMouseButton(0))  // Para PC (clic del rat�n)
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

            // Mover los cr�ditos hacia arriba, ajustando la velocidad si se est� tocando la pantalla
            float currentMoveSpeed = isTouching ? moveSpeed * fastMoveSpeedMultiplier : moveSpeed;
            creditsRectTransform.anchoredPosition += new Vector2(0, currentMoveSpeed * Time.deltaTime);

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

            // Mover los cr�ditos hacia arriba, ajustando la velocidad si se est� tocando la pantalla
            float currentMoveSpeed = isTouching ? moveSpeed * fastMoveSpeedMultiplier : moveSpeed;
            creditsRectTransform.anchoredPosition += new Vector2(0, currentMoveSpeed * Time.deltaTime);

            // Comprobar si los cr�ditos han salido completamente de la pantalla
            if (creditsRectTransform.anchoredPosition.y > Screen.height + heightOffset)
            {
                creditsPanel.SetActive(false); // Desactivar el panel de cr�ditos
                settingsMenu.SetActive(true);  // Activar el men� de ajustes
                yield break;  // Salir de la coroutine inmediatamente
            }

            yield return null;
        }

        // Si no salieron de la pantalla antes, realizar el fade-out completo
        creditsPanel.SetActive(false);
        settingsMenu.SetActive(true);
    }
}