using UnityEngine;
using UnityEngine.VFX;
using System.Collections;
public class DestroyAfterTime2 : MonoBehaviour
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
        StartCoroutine(DestroyAfterDelay2());
    }

    private void Update()
    {
        StartCoroutine(DestroyAfterDelay2());
    }

   

    public IEnumerator DestroyAfterDelay2()
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