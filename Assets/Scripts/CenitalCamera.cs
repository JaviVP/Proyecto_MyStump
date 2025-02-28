using UnityEngine;
using Unity.Cinemachine;

public class CamaraCenital : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 10f;  // Velocidad de zoom
    [SerializeField] private float minZoom = 20f;    // Zoom mínimo
    [SerializeField] private float maxZoom = 40f;    // Zoom máximo

    private CinemachineCamera TopcinemachineCamera;  // Referencia a la cámara de Cinemachine
    private CinemachineBrain brain;   // Referencia a Cinemachine Brain

    private bool disableTouchInputDuringTransition = false;
    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        TopcinemachineCamera = GetComponent<CinemachineCamera>(); // Obtener la cámara Cinemachine
    }

    void Update()
    {
        // Comprobamos si está en transición (si es true, desactivamos las entradas táctiles)
        if (brain.IsBlending)
        {
            disableTouchInputDuringTransition = true;
        }
        else
        {
            disableTouchInputDuringTransition = false;
        }

        // Si las entradas táctiles están bloqueadas, no procesamos el movimiento táctil
        if (disableTouchInputDuringTransition)
            return;

        if (FindAnyObjectByType<CamerasController>().GetActiveCamera() == TopcinemachineCamera && UiButtons.Instance.TouchesEnabled() == true)
        {
            ZoomCamera();
        }

    }

    void ZoomCamera()
    {
        if (TopcinemachineCamera == null) return;

        // Control del zoom con la rueda del ratón
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
