using System.Collections;
using UnityEngine;

public class CamaraVR : MonoBehaviour
{

    void Start()
    {
        Input.gyro.enabled = true;
    }

    void Update()
    {
        transform.Rotate(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, 0);
    }
}
