using UnityEngine;
using Unity.Cinemachine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.1f;  // Sensibilidad del movimiento
    [SerializeField] private float zoomSpeed = 10f;   // Velocidad de zoom
    [SerializeField] private float minZoom = 10f;     // Zoom mínimo
    [SerializeField] private float maxZoom = 60f;     // Zoom máximo

    [SerializeField] private float minX = -10f;       // Límite mínimo del eje X
    [SerializeField] private float maxX = 10f;        // Límite máximo del eje X
    [SerializeField] private float minZ = -10f;       // Límite mínimo del eje Z
    [SerializeField] private float maxZ = 10f;        // Límite máximo del eje Z

    private CinemachineCamera virtualCamera;
    private Vector2 lastTouchPosition;
    private bool isTouching = false;

    void Start()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        MoveCameraPC();   // Movimiento con mouse
        MoveCameraTouch(); // Movimiento con táctil
        ZoomCameraTouch();
        ZoomCamera();
    }

    void MoveCameraPC()
    {
        Vector3 newPosition = transform.position;

        // Movimiento en el eje X (derecha e izquierda)
        if (Input.GetMouseButton(1)) // Botón derecho del ratón
        {
            float mouseX = Input.GetAxis("Mouse X") * moveSpeed;
            newPosition.x += -mouseX; // Invertir movimiento en el eje X
        }

        // Movimiento en el eje Z (arriba y abajo)
        if (Input.GetMouseButton(0)) // Botón izquierdo del ratón
        {
            float mouseY = Input.GetAxis("Mouse Y") * moveSpeed;
            newPosition.z += -mouseY; // Invertir movimiento en el eje Z
        }

        // Aplicar los límites a la posición
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        // Actualizar la posición de la cámara
        transform.position = newPosition;
    }

    void MoveCameraTouch()
    {
        if (Input.touchCount == 1) // Solo un dedo en pantalla
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                Vector2 delta = touch.deltaPosition * moveSpeed;

                Vector3 newPosition = transform.position;
                newPosition.x -= delta.x * 0.01f;
                newPosition.z -= delta.y * 0.01f; 

                // Aplicar límites
                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

                transform.position = newPosition;

                lastTouchPosition = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isTouching = false;
            }
        }
    }

    void ZoomCameraTouch()
    {
        // Control del zoom con la rueda del ratón


        // Zoom en móviles con gesto de "pinza"
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Posición anterior de los dedos
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Distancia anterior y actual entre los dedos
            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            // Diferencia de distancia entre los dos frames
            float difference = currentMagnitude - prevMagnitude;

            // Aplicar zoom con sensibilidad ajustada
            LensSettings lens = virtualCamera.Lens;
            lens.FieldOfView -= difference * zoomSpeed * 0.01f;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView, minZoom, maxZoom);
            virtualCamera.Lens = lens;
        }
    }

    public void ZoomCamera()
    {
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0f && virtualCamera != null)
        {
            LensSettings lens = virtualCamera.Lens;
            lens.FieldOfView -= scrollInput * zoomSpeed;
            lens.FieldOfView = Mathf.Clamp(lens.FieldOfView, minZoom, maxZoom);
            virtualCamera.Lens = lens;
        }

    }
}
