using UnityEngine;
using UnityEngine.VFX;
using System.Collections;
public class DestroyAfterTime : MonoBehaviour
{
    public float minTime = 5f;
    public float maxTime = 10f;

    private VisualEffect vfx;

    void Start()
    {

        // Validaci�n anticipada para evitar GetChild cada vez
        if (transform.childCount > 0)
        {
            vfx = transform.GetChild(0).GetComponent<VisualEffect>();
        }
        vfx.Play();
        StartCoroutine(DestroyAfterDelay());
    }

    private void Update()
    {
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        float delay = Random.Range(minTime, maxTime);
        yield return new WaitForSeconds(delay);

        if (vfx != null)
        {
            vfx.Stop(); // Det�n el efecto
            yield return new WaitForSeconds(0.5f); // Espera a que finalice internamente
        }

        // Desactiva solo si todo fue bien

        gameObject.SetActive(false);
    }

  

}