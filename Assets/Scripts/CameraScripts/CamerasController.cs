using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

public class CamerasController : MonoBehaviour
{
    [SerializeField] private CinemachineCamera mainCamera;       // Cámara principal del juego
    [SerializeField] private CinemachineCamera topDownCamera;    // Cámara cenital
    [SerializeField] private float perspectiveThreshold;   // Umbral de zoom para activar la cenital
    [SerializeField] private float topDownThreshold;       // Umbral para volver a la principal

    private CinemachineCamera activeCamera;
    private bool isTopDownActive = false;
    private float currentZoom;
    private Vector3 initialMainPosition, initialTopPosition;
    private Quaternion initialMainRotation, initialTopRotation;

    public static CamerasController Instance { get; private set; }

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
        initialMainPosition = mainCamera.transform.position;
        initialMainRotation = mainCamera.transform.rotation;
        initialTopPosition = topDownCamera.transform.position;
        initialTopRotation = topDownCamera.transform.rotation;

        ActivateCamera(mainCamera);

    }

    void Update()
    {
        CheckTopDownCamera();
     
    }

    public void ActivateCamera(CinemachineCamera newCamera)
    {
        if (activeCamera == newCamera) return;

        if (activeCamera != null)
        {
            activeCamera.gameObject.SetActive(false);
        }

        activeCamera = newCamera;
        activeCamera.gameObject.SetActive(true);
    }

    public void CheckTopDownCamera()
    {
        if (activeCamera == null) return;

        currentZoom = activeCamera.Lens.FieldOfView;

        if (activeCamera == mainCamera && currentZoom == perspectiveThreshold && !isTopDownActive)
        {
            Debug.Log("Activo camara cenital");
            mainCamera.transform.position = initialMainPosition;
            mainCamera.transform.rotation = initialMainRotation;
            mainCamera.Lens.FieldOfView = 60.0f;
            ActivateCamera(topDownCamera);
            isTopDownActive = true;
         
        }
        else if (activeCamera == topDownCamera && currentZoom == topDownThreshold && isTopDownActive)
        {
            topDownCamera.transform.position = initialTopPosition;
            topDownCamera.transform.rotation = initialTopRotation;
            topDownCamera.Lens.FieldOfView = 60.0f;
            ActivateCamera(mainCamera);
            isTopDownActive = false;
          
        }
    }

    public void ResetCamera()
    {
        mainCamera.transform.position = initialMainPosition;
        mainCamera.transform.rotation = initialMainRotation;
        mainCamera.Lens.FieldOfView = 60.0f;

        topDownCamera.transform.position = initialTopPosition;
        topDownCamera.transform.rotation = initialTopRotation;
        topDownCamera.Lens.FieldOfView = 60.0f;

        ActivateCamera(mainCamera);
    }

    public void ActivateCenital()
    {
        ActivateCamera(topDownCamera);
        isTopDownActive = true;
    }

    public void ActivatePerspective()
    {
        ActivateCamera(mainCamera);
        isTopDownActive = false;
    }

    public CinemachineCamera GetActiveCamera()
    {
        return activeCamera;
    }

    public bool IsTopDownActive()
    {
        return isTopDownActive;
    }
}
