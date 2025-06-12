using UnityEngine;

public class FadeDemo : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float duration = 3f;
    [SerializeField] private float delay = 3f;

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    System.Collections.IEnumerator FadeOut()
    {
        // Espera antes de empezar el fade
        yield return new WaitForSeconds(delay);

        float time = 0f;
        float startAlpha = canvasGroup.alpha;

        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        this.gameObject.SetActive(false);    
    }
}
