using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoController : MonoBehaviour
{
    [SerializeField] private List<GameObject> logosTermitas; // De izquierda a derecha
    [SerializeField] private List<GameObject> logosHormigas; // De derecha a izquierda
    [SerializeField] private Transform centroPantalla;
    [SerializeField] private float moveDuration = 0.1f;

    [SerializeField] private Sprite runnerHormiga;
    [SerializeField] private Sprite terraformerHormiga;
    [SerializeField] private Sprite panchulinaHormiga;

    [SerializeField] private Sprite runnerTermita;
    [SerializeField] private Sprite terraformerTermita;
    [SerializeField] private Sprite panchulinaTermita;

    private bool procesandoLogo = false;  // Para evitar que se inicien varias corrutinas
    private enum Turno { Termita, Hormiga }
    private Turno turnoActual = Turno.Termita;

    private void Start()
    {
        InicializarLogos();
    }


    public void ColocarPieza()
    {
        if (procesandoLogo) return;  // Si ya estamos procesando, no hacer nada más

        if (turnoActual == Turno.Termita)
        {

            StartCoroutine(ProcesarLogo(logosTermitas, logosHormigas)); // Hormigas serán las siguientes
            turnoActual = Turno.Hormiga;           
   
        }
        else
        {

            StartCoroutine(ProcesarLogo(logosHormigas, logosTermitas)); // Termitas serán las siguientes
            turnoActual = Turno.Termita;

            
        }
    }

    public void ProcesarLogosRestantes()
    {
        StartCoroutine(ProcesarLogosHastaTerminar());
    }

    private IEnumerator ProcesarLogosHastaTerminar()
    {
        while (HayLogosActivos())
        {
            if (procesandoLogo)
            {
                yield return null; // Espera a que termine el logo actual
                continue;
            }

            // Lógica alterna entre turnos
            if (turnoActual == Turno.Termita && HayLogosActivosEn(logosTermitas))
            {
                yield return StartCoroutine(ProcesarLogo(logosTermitas, logosHormigas));
                turnoActual = Turno.Hormiga;
            }
            else if (turnoActual == Turno.Hormiga && HayLogosActivosEn(logosHormigas))
            {
                yield return StartCoroutine(ProcesarLogo(logosHormigas, logosTermitas));
                turnoActual = Turno.Termita;
            }
            else
            {
                // Si el turno actual no tiene logos, salta al otro
                turnoActual = (turnoActual == Turno.Termita) ? Turno.Hormiga : Turno.Termita;
                yield return null;
            }
        }

       
    }

    private bool HayLogosActivos()
    {
        return HayLogosActivosEn(logosTermitas) || HayLogosActivosEn(logosHormigas);
    }

    private bool HayLogosActivosEn(List<GameObject> logos)
    {
        foreach (var logo in logos)
        {
            if (logo.activeSelf) return true;
        }
        return false;
    }

    IEnumerator ProcesarLogo(List<GameObject> logos, List<GameObject> siguienteTurnoLogos)
    {
        procesandoLogo = true;  // Marcamos que estamos procesando una corrutina

        // 1. Poner todos los logos activos en modo apagado (al final de la animación)
        ApagarLogos(logosTermitas);
        ApagarLogos(logosHormigas);

        // 2. Encontrar el logo más cercano al centro
        GameObject logoCercano = null;
        float distanciaMinima = Mathf.Infinity;

        foreach (var logo in logos)
        {
            if (!logo.activeSelf) continue;
            float distancia = Mathf.Abs(logo.transform.position.x - centroPantalla.position.x);
            if (distancia < distanciaMinima)
            {
                distanciaMinima = distancia;
                logoCercano = logo;
            }
        }

        if (logoCercano == null)
        {
            procesandoLogo = false;
            yield break;  // Si no se encuentra ningún logo activo, terminamos la corrutina
        }

        // 3. Encender el logo que va a animarse
        EncenderLogo(logoCercano);

        // 4. Ejecutar animación
        Animator anim = logoCercano.GetComponent<Animator>();
        anim.SetTrigger("Scale");
        yield return new WaitForSeconds(1.2f);  // Esperamos que termine la animación

        // 5. Desactivar logo
        logoCercano.SetActive(false);

        // 6. Reorganizar los logos
        Vector3[] posiciones = new Vector3[logos.Count];
        for (int i = 0; i < logos.Count; i++)
        {
            posiciones[i] = logos[i].transform.position;
        }

        // Esperamos que cada logo se mueva antes de pasar al siguiente
        for (int i = 0; i < logos.Count - 1; i++)
        {
            if (!logos[i].activeSelf)
            {
                for (int j = i + 1; j < logos.Count; j++)
                {
                    if (logos[j].activeSelf)
                    {
                        // Esperamos que el logo termine de moverse antes de continuar
                        yield return StartCoroutine(MoverLogo(logos[j].transform, posiciones[i], moveDuration/2));

                        // Intercambiamos las posiciones de los logos en la lista
                        var temp = logos[i];
                        logos[i] = logos[j];
                        logos[j] = temp;
                        break;
                    }
                }
            }
        }

        // 7. Apagar todos los logos por si alguno quedó encendido
        ApagarLogos(logosTermitas);
        ApagarLogos(logosHormigas);

        // 8. Encender el primero del siguiente turno
        EncenderPrimeroActivo(siguienteTurnoLogos);

        procesandoLogo = false;  // Ya terminamos, ahora se puede iniciar otra corrutina si es necesario
    }

    IEnumerator MoverLogo(Transform logo, Vector3 targetPos, float duration)
    {
        Vector3 startPos = logo.position;
        float time = 0f;

        while (time < duration)
        {
            logo.position = Vector3.Lerp(startPos, targetPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        logo.position = targetPos;
    }

    private void ApagarLogos(List<GameObject> logos)
    {
        foreach (var logo in logos)
        {
            if (!logo.activeSelf) continue;

            var image = logo.GetComponent<Image>();
            var sprite = logo.GetComponent<SpriteRenderer>();

            Color originalColor = Color.white;
            float grayScale = 0.3f;

            if (image != null)
            {
                originalColor = image.color;
                Color gris = new Color(grayScale, grayScale, grayScale, originalColor.a); // Mantiene alpha
                image.color = gris;
            }

            if (sprite != null)
            {
                originalColor = sprite.color;
                Color gris = new Color(grayScale, grayScale, grayScale, originalColor.a); // Mantiene alpha
                sprite.color = gris;
            }
        }
    }

    private void InicializarLogos()
    {
        // Apagar todos los logos activos
        ApagarLogos(logosTermitas);
        ApagarLogos(logosHormigas);

        // Solo encender el primer logo de la facción que empieza
        if (turnoActual == Turno.Termita)
        {
            EncenderPrimeroActivo(logosTermitas);
        }
        else
        {
            EncenderPrimeroActivo(logosHormigas);
        }
    }

    private void EncenderPrimeroActivo(List<GameObject> logos)
    {
        foreach (var logo in logos)
        {
            if (logo.activeSelf)
            {
                EncenderLogo(logo);
                break; // Solo el primero activo
            }
        }
    }

    private void EncenderLogo(GameObject logo)
    {
        var image = logo.GetComponent<Image>();
        var sprite = logo.GetComponent<SpriteRenderer>();

        if (image != null) image.color = Color.white;
        if (sprite != null) sprite.color = Color.white;
    }

    public void AsignarSpritesPorTipo(List<UnitType> unidades)
    {
        for (int i = 0; i < unidades.Count; i++)
        {
            UnitType tipo = unidades[i];

            // Asegurar que hay suficientes logos en ambas listas
            if (i < logosHormigas.Count)
            {
                Sprite spriteHormiga = ObtenerSprite(tipo, true);
                AsignarSpriteALogo(logosHormigas[i], spriteHormiga);
            }

            if (i < logosTermitas.Count)
            {
                Sprite spriteTermita = ObtenerSprite(tipo, false);
                AsignarSpriteALogo(logosTermitas[i], spriteTermita);
            }
        }
    }

    private Sprite ObtenerSprite(UnitType tipo, bool esHormiga)
    {
        switch (tipo)
        {
            case UnitType.Runner: return esHormiga ? runnerHormiga : runnerTermita;
            case UnitType.Terraformer: return esHormiga ? terraformerHormiga : terraformerTermita;
            case UnitType.Panchulina: return esHormiga ? panchulinaHormiga : panchulinaTermita;
            default: return null;
        }
    }


    private void AsignarSpriteALogo(GameObject logo, Sprite nuevoSprite)
    {
        var image = logo.GetComponent<Image>();
        var spriteRenderer = logo.GetComponent<SpriteRenderer>();

        if (image != null)
            image.sprite = nuevoSprite;

        if (spriteRenderer != null)
            spriteRenderer.sprite = nuevoSprite;
    }
}