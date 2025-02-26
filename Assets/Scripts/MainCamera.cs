using UnityEngine;
using Unity.Cinemachine;

public class MainCamera : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 0.1f;  // Sensibilidad del movimiento
    [SerializeField] private float zoomSpeed = 10f;   // Velocidad de zoom
    [SerializeField] private float minZoom = 10f;     // Zoom m�nimo
    [SerializeField] private float maxZoom = 60f;     // Zoom m�ximo

    [SerializeField] private float minX = -10f;       // L�mite m�nimo del eje X
    [SerializeField] private float maxX = 10f;        // L�mite m�ximo del eje X
    [SerializeField] private float minZ = -10f;       // L�mite m�nimo del eje Z
    [SerializeField] private float maxZ = 10f;        // L�mite m�ximo del eje Z

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
        MoveCameraTouch(); // Movimiento con t�ctil
        ZoomCameraTouch();
        ZoomCamera();
    }

    void MoveCameraPC()
    {
        Vector3 newPosition = transform.position;

        // Movimiento en el eje X (derecha e izquierda)
        if (Input.GetMouseButton(1)) // Bot�n derecho del rat�n
        {
            float mouseX = Input.GetAxis("Mouse X") * moveSpeed;
            newPosition.x += -mouseX; // Invertir movimiento en el eje X
        }

        // Movimiento en el eje Z (arriba y abajo)
        if (Input.GetMouseButton(0)) // Bot�n izquierdo del rat�n
        {
            float mouseY = Input.GetAxis("Mouse Y") * moveSpeed;
            newPosition.z += -mouseY; // Invertir movimiento en el eje Z
        }

        // Aplicar los l�mites a la posici�n
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        // Actualizar la posici�n de la c�mara
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

                // Aplicar l�mites
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
        // Control del zoom con la rueda del rat�n


        // Zoom en m�viles con gesto de "pinza"
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Posici�n anterior de los dedos
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
