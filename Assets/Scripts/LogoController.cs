using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogoController : MonoBehaviour
{
    public List<GameObject> logosTermitas; // De izquierda a derecha
    public List<GameObject> logosHormigas; // De derecha a izquierda
    public Transform centroPantalla;
    public float moveDuration = 0.3f;

    private enum Turno { Termita, Hormiga }
    private Turno turnoActual = Turno.Termita;

    private void Start()
    {
        InicializarLogos();
    }

    public void ColocarPieza()
    {
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

    IEnumerator ProcesarLogo(List<GameObject> logos, List<GameObject> siguienteTurnoLogos)
    {
        // 1. Poner todos los logos activos en modo apagado
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

        if (logoCercano == null) yield break;

        // 3. Encender el logo que va a animarse
        EncenderLogo(logoCercano);

        // 4. Ejecutar animación
        Animator anim = logoCercano.GetComponent<Animator>();
        anim.SetTrigger("Scale");
        yield return new WaitForSeconds(1.5f);

        // 5. Desactivar logo
        logoCercano.SetActive(false);

        // 6. Reorganizar
        Vector3[] posiciones = new Vector3[logos.Count];
        for (int i = 0; i < logos.Count; i++)
        {
            posiciones[i] = logos[i].transform.position;
        }

        for (int i = 0; i < logos.Count - 1; i++)
        {
            if (!logos[i].activeSelf)
            {
                for (int j = i + 1; j < logos.Count; j++)
                {
                    if (logos[j].activeSelf)
                    {
                        StartCoroutine(MoverLogo(logos[j].transform, posiciones[i], moveDuration));
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
}