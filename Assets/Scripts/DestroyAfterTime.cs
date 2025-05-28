using UnityEngine;
using UnityEngine.VFX;

public class DestroyAfterTime : MonoBehaviour
{
    public float minTime = 5f;
    public float maxTime = 10f;

    void Start()
    {
        StartCoroutine(DestroyAfterDelay());
        
    }

    private System.Collections.IEnumerator DestroyAfterDelay()
    {
        float delay = Random.Range(minTime, maxTime);
        yield return new WaitForSeconds(delay);
        GetComponent<VisualEffect>().Stop();
        yield return new WaitForSeconds(delay+.5f);
        Destroy(gameObject);
    }
}
