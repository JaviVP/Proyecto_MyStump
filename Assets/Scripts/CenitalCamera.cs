using UnityEngine;
using Unity.Cinemachine;

public class CamaraCenital : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 10f;  // Velocidad de zoom
    [SerializeField] private float minZoom = 20f;    // Zoom m�nimo
    [SerializeField] private float maxZoom = 40f;    // Zoom m�ximo

    private CinemachineCamera TopcinemachineCamera;  // Referencia a la c�mara de Cinemachine

    void Start()
    {
        TopcinemachineCamera = GetComponent<CinemachineCamera>(); // Obtener la c�mara Cinemachine
    }

    void Update()
    {
        if (FindAnyObjectByType<CamerasController>().GetActiveCamera() == TopcinemachineCamera)
        {
            ZoomCamera();
        }

    }

    void ZoomCamera()
    {
        if (TopcinemachineCamera == null) return;

        // Control del zoom con la rueda del rat�n
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            LensSettings lens = TopcinemachineCamera.Lens;
            lens.FieldOfView -= scrollInput * zoomSpeed;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView, minZoom, maxZoom);
            TopcinemachineCamera.Lens = lens; // Aplicar cambios
        }


        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            LensSettings lens = TopcinemachineCamera.Lens;
            lens.FieldOfView -= difference * zoomSpeed * 0.01f;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView, minZoom, maxZoom);
            TopcinemachineCamera.Lens = lens; // Aplicar cambios
        }
    }
}
