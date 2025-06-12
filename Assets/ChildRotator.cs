using UnityEngine;

public class ChildRotator : MonoBehaviour
{
    public float rotationSpeed = 90f;
    public float pulseSpeed = 1f;
    public float pulseSize = 0.2f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Rotate
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // Pulse scale
        float scaleOffset = Mathf.PingPong(Time.time * pulseSpeed, pulseSize);
        transform.localScale = originalScale + Vector3.one * scaleOffset;
    }
}
