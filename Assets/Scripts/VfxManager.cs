using UnityEngine;

public class VfxManager : MonoBehaviour
{
    [SerializeField] private GameObject[] vfxs;
    public static VfxManager Instance { get; private set; }
    public GameObject[] Vfxs { get => vfxs; set => vfxs = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;

        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
