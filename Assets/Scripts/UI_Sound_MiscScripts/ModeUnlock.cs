using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class ModeUnlock : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Referencias internas")]
    [SerializeField] private TextMeshProUGUI textChampionship; // Texto principal del botón  
    [SerializeField] private GameObject lockIcon;              // Icono de candado con animación

    [Header("Colores")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color pressedColor = new Color(231f / 255f, 203f / 255f, 51f / 255f);
    [SerializeField] private Color blockedPressedColor = Color.gray;

    private Animator lockAnimator;
    private Button button;
    private bool isLocked;

    void Start()
    {
        // Si no existe FINISHGAME, se considera bloqueado
        if (!PlayerPrefs.HasKey("FINISHGAME"))
        {
            PlayerPrefs.SetInt("FINISHGAME", 1);
        }

        isLocked = PlayerPrefs.GetInt("FINISHGAME") == 1;

        button = GetComponent<Button>();
        lockAnimator = lockIcon != null ? lockIcon.GetComponent<Animator>() : null;

        if (PlayerPrefs.GetInt("FINISHGAME") == 0)
        {
           lockIcon.SetActive(false);
        }
        // Preparar botón
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);

        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        // Cambiar opacidad del texto principal
        if (textChampionship != null)
        {
            Color c = normalColor;
            if (isLocked) c.a = 0.5f;
            textChampionship.color = c;
        }

        if (lockIcon != null)
            lockIcon.SetActive(isLocked); // Solo activo si está bloqueado
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (textChampionship != null)
        {
            textChampionship.color = isLocked ? blockedPressedColor : pressedColor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (textChampionship != null)
        {
            Color c = normalColor;
            if (isLocked) c.a = 0.5f;
            textChampionship.color = c;
        }
    }

    private void OnClick()
    {
        if (isLocked)
        {
            ShowLockFeedback();
        }
        else
        {
            // Aquí va la lógica real del modo Championship
            Debug.Log("Championship mode activated!");
            SceneManager.LoadScene(6);
           
        }
    }

    private void ShowLockFeedback()
    {
       

        if (lockAnimator != null)
        {
            lockIcon.SetActive(true);
            lockAnimator.SetTrigger("Lock");
            StartCoroutine(TriggerReset(1.0f));
        }

       
    }

   
    private IEnumerator TriggerReset(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        lockAnimator.SetTrigger("Lock");

    }

    // Método opcional para desbloquear desde fuera
    public void UnlockMode()
    {
        PlayerPrefs.SetInt("FINISHGAME", 0);
        PlayerPrefs.Save();
        isLocked = false;
        UpdateVisuals();
    }
}