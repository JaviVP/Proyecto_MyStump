using UnityEngine;

public class AddShader : MonoBehaviour
{
    [SerializeField] private Material outlineMaterial; // Material con el shader de outline
    private Renderer objRenderer;
    private bool isOutlineActive = false;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        ToggleOutline();
                    }
                }
            }
        }
    }

    void ToggleOutline()
    {
        if (isOutlineActive)
        {
            // Elimina el material de outline
            var materials = objRenderer.materials;
            if (materials.Length > 1)
            {
                objRenderer.materials = new Material[] { materials[0] };
            }
            isOutlineActive = false;
        }
        else
        {
            // Añade el material de outline
            var materials = objRenderer.materials;
            Material[] newMaterials = new Material[materials.Length + 1];

            for (int i = 0; i < materials.Length; i++)
            {
                newMaterials[i] = materials[i];
            }
            newMaterials[newMaterials.Length - 1] = outlineMaterial;

            objRenderer.materials = newMaterials;
            isOutlineActive = true;
        }
    }
}
