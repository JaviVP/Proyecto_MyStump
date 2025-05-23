using UnityEngine;
using Unity.Cinemachine;

public class CamaraCenital : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 10f;  // Velocidad de zoom
    [SerializeField] private float minZoom = 20f;    // Zoom m�nimo
    [SerializeField] private float maxZoom = 40f;    // Zoom m�ximo

    private CinemachineCamera TopcinemachineCamera;  // Referencia a la c�mara de Cinemachine
    private CinemachineBrain brain;   // Referencia a Cinemachine Brain

    private bool disableTouchInputDuringTransition = false;
    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        TopcinemachineCamera = GetComponent<CinemachineCamera>(); // Obtener la c�mara Cinemachine
    }

    void Update()
    {
#if UNITY_WEBGL || UNITY_STANDALONE
        ZoomCameraPC(); // Solo PC/WebGL
#endif

        // Comprobamos si est� en transici�n (si es true, desactivamos las entradas t�ctiles)
        if (brain.IsBlending)
        {
            disableTouchInputDuringTransition = true;
        }
        else
        {
            disableTouchInputDuringTransition = false;
        }

        // Si las entradas t�ctiles est�n bloqueadas, no procesamos el movimiento t�ctil
        if (disableTouchInputDuringTransition)
            return;

#if UNITY_ANDROID
    // Solo m�viles Android
    if (FindAnyObjectByType<CamerasController>().GetActiveCamera() == TopcinemachineCamera &&
        UiManager.Instance.TouchesEnabled())
    {
        ZoomCamera(); // Zoom por gesto de pinza
    }
#endif
    }


    private void ZoomCameraPC()
    {
        if (TopcinemachineCamera == null) return;
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0f)
        {
            LensSettings lens = TopcinemachineCamera.Lens;
            lens.FieldOfView -= scrollInput * zoomSpeed;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView, minZoom, maxZoom);
            TopcinemachineCamera.Lens = lens; 
        }


    }
    void ZoomCamera()
    {
        if (TopcinemachineCamera == null) return;

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
