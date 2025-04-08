using UnityEngine;
using UnityEngine.EventSystems;

public class LogoController : MonoBehaviour, IPointerDownHandler
{
    private Animator animator;
    public float animationDuration = 1.5f; // Ajusta al tiempo real del clip

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (animator.GetBool("ActivateScale"))
        {
            animator.Play("LogoScale");
            gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!animator.GetBool("ActivateScale"))
        {
            animator.SetBool("ActivateScale", true);
            Debug.Log("Pulso en el objeto");
            StartCoroutine(ResetBoolAfterAnimation());
        }
    }

    private System.Collections.IEnumerator ResetBoolAfterAnimation()
    {
        yield return new WaitForSeconds(animationDuration);
        animator.SetBool("ActivateScale", false);
    }
}