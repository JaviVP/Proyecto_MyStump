using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialSwipeBasicoConBotones : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private float anchoPagina = 1920f;
    [SerializeField] private float velocidad = 10f;
    [SerializeField] private Button botonIzquierda;
    [SerializeField] private Button botonDerecha;

    private RectTransform rt;
    private int paginaActual = 0;
    private int totalPaginas;
    private Vector2 inicioDrag;
    private Vector2 destino;
    private bool moviendo = false;
    private float umbral = 300f;

    void Start()
    {
        rt = GetComponent<RectTransform>();
        totalPaginas = rt.childCount; 
        destino = rt.anchoredPosition;

        PosicionarPaginas();

        if (botonIzquierda != null)
            botonIzquierda.onClick.AddListener(() => CambiarPagina(-1));
        if (botonDerecha != null)
            botonDerecha.onClick.AddListener(() => CambiarPagina(1));
    }

    void PosicionarPaginas()
    {
        int paginaIndex = 0;
        for (int i = 0; i < rt.childCount; i++)
        {
            RectTransform hijo = rt.GetChild(i).GetComponent<RectTransform>();
            if (hijo.GetComponent<Button>()) continue;

            hijo.anchoredPosition = new Vector2(paginaIndex * anchoPagina, 0);
            paginaIndex++;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        inicioDrag = eventData.position;
        moviendo = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 delta = eventData.position - inicioDrag;
        rt.anchoredPosition = -Vector2.right * (paginaActual * anchoPagina) + new Vector2(delta.x, 0);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float deltaX = eventData.position.x - inicioDrag.x;

        if (Mathf.Abs(deltaX) > umbral)
        {
            if (deltaX < 0 && paginaActual < totalPaginas - 1) paginaActual++;
            else if (deltaX > 0 && paginaActual > 0) paginaActual--;
        }

        destino = -Vector2.right * (paginaActual * anchoPagina);
        moviendo = true;
    }

    void Update()
    {
        if (!moviendo) return;

        rt.anchoredPosition = Vector2.Lerp(rt.anchoredPosition, destino, Time.deltaTime * velocidad);

        if (Vector2.Distance(rt.anchoredPosition, destino) < 0.1f)
        {
            rt.anchoredPosition = destino;
            moviendo = false;
        }
    }

    void CambiarPagina(int direccion)
    {
        int nuevaPagina = paginaActual + direccion;

        if (nuevaPagina >= 0 && nuevaPagina < totalPaginas)
        {
            paginaActual = nuevaPagina;
            destino = -Vector2.right * (paginaActual * anchoPagina);
            moviendo = true;
        }
    }
}