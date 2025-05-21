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

    private CinemachineBrain brain;   // Referencia a Cinemachine Brain
    private CinemachineCamera virtualCamera;
    private Vector2 lastTouchPosition;
    private bool isTouching = false;
    private float touchDragThreshold = 5f;
    private Vector3 mouseStart;
    private bool mousePressed = false;
    private float dragThreshold = 1f; // píxeles
    // Nueva variable para bloquear las entradas táctiles durante la transición
    private bool disableTouchInputDuringTransition = false;

    public static MainCamera Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        virtualCamera = GetComponent<CinemachineCamera>();
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

        // Movimiento con mouse
       

        // Movimiento con táctil solo si está habilitado
        HandlePlatformInputs();
    }
    void HandlePlatformInputs()
    {
#if UNITY_WEBGL || UNITY_STANDALONE
        ZoomCamera();
        MoveCameraPC();
#elif UNITY_ANDROID
    if (UiManager.Instance.TouchesEnabled())
    {
        MoveCameraTouch();
        ZoomCameraTouch();
    }
#endif
    }

    void MoveCameraPC()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStart = Input.mousePosition;
            mousePressed = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
        }

        if (Input.GetMouseButton(0) && mousePressed)
        {
            if (Vector3.Distance(Input.mousePosition, mouseStart) > dragThreshold)
            {
                Vector3 newPosition = transform.position;

                float mouseX = Input.GetAxis("Mouse X") * moveSpeed;
                float mouseY = Input.GetAxis("Mouse Y") * moveSpeed;

                newPosition.x -= mouseX;
                newPosition.z -= mouseY;

                newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

                transform.position = newPosition;
            }
        }
    }


    void MoveCameraTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastTouchPosition = touch.position;
                isTouching = true;
            }
            else if (touch.phase == TouchPhase.Moved && isTouching)
            {
                if ((touch.position - lastTouchPosition).magnitude > touchDragThreshold)
                {
                    Vector2 delta = touch.deltaPosition * moveSpeed;
                    Vector3 newPosition = transform.position;
                    newPosition.x -= delta.x * 0.01f;
                    newPosition.z -= delta.y * 0.01f;

                    newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
                    newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

                    transform.position = newPosition;

                    lastTouchPosition = touch.position;
                }
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                isTouching = false;
            }
        }
    }

    void ZoomCameraTouch()
    {
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