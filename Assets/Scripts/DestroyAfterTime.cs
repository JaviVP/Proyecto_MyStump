using UnityEngine;
using UnityEngine.VFX;

public class DestroyAfterTime : MonoBehaviour
{
    public float minTime = 5f;
    public float maxTime = 10f;

    private VisualEffect vfx;

    void Start()
    {
        // Validación anticipada para evitar GetChild cada vez
        if (transform.childCount > 0)
        {
            vfx = transform.GetChild(0).GetComponent<VisualEffect>();
        }

        StartCoroutine(DestroyAfterDelay());
    }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        float delay = Random.Range(minTime, maxTime);
        yield return new WaitForSeconds(delay);

        if (vfx != null)
        {
            vfx.Stop(); // Detén el efecto
            yield return new WaitForSeconds(0.5f); // Espera a que finalice internamente
        }

        // Desactiva solo si todo fue bien
        gameObject.SetActive(false);
    }
}