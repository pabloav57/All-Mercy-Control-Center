using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GestorIncendios : MonoBehaviour
{
    public GameObject fuegoPrefab;
    public GameObject humoPrefab;
    public GameObject marcadorMapa;
    public TextMeshProUGUI textoAviso;

    public GameObject chorroAguaPrefab;
    public AudioClip sonidoChorroAgua;
    public AudioClip sonidoMisionSuperada;

    public float radioApagado = 10f;
    public float tiempoParaPropagar = 30f;
    public float radioPropagacion = 15f;

    private GameObject fuegoInstanciado;
    private GameObject humoInstanciado;
    private GameObject marcadorInstanciado;
    private Transform casaObjetivo;
    private bool incendioActivo = false;
    private bool yaSePropago = false;
    private List<GameObject> incendiosPropagados = new List<GameObject>();

    public Transform puntoDeSalida;

    private AudioSource audioSource;
    private GameObject chorroAgua;
    private bool estaUsandoChorro = false;
    private float tiempoPresionadoF = 0f;

    private Color colorVerdeGradiente = new Color(0.0f, 1.0f, 0.0f); // Color verde (para el gradiente)
    private Color colorRojizo = new Color(1.0f, 0.0f, 0.0f); // Color rojo para incendios

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(GenerarIncendioCada(60f));
        ActualizarColorTexto(false);  // Inicialmente, no hay incendios
    }

    IEnumerator GenerarIncendioCada(float tiempo)
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempo);
            GenerarIncendioAleatorio();
        }
    }

    void GenerarIncendioAleatorio()
    {
        GameObject[] casas = GameObject.FindGameObjectsWithTag("Casa");
        if (casas.Length == 0) return;

        casaObjetivo = casas[Random.Range(0, casas.Length)].transform;

        fuegoInstanciado = Instantiate(fuegoPrefab, casaObjetivo.position + Vector3.up * 2, Quaternion.identity);
        humoInstanciado = Instantiate(humoPrefab, casaObjetivo.position + Vector3.up * 4, Quaternion.identity);
        marcadorInstanciado = Instantiate(marcadorMapa, casaObjetivo.position + Vector3.up * 5, Quaternion.identity);

        Collider col = fuegoInstanciado.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        textoAviso.text = "¡Incendio en " + casaObjetivo.name + "!";
        incendioActivo = true;
        yaSePropago = false;

        ActualizarColorTexto(true);  // Cambiar color a rojo si hay incendio

        StartCoroutine(EsperarUnidadBomberos());
        StartCoroutine(EsperarPropagacion());
    }

    IEnumerator EsperarUnidadBomberos()
    {
        while (incendioActivo)
        {
            GameObject[] unidades = GameObject.FindGameObjectsWithTag("Unidad");

            foreach (GameObject unidad in unidades)
            {
                Unidad info = unidad.GetComponent<Unidad>();
                if (info != null && info.tipoUnidad == "Bombero")
                {
                    float distancia = Vector3.Distance(unidad.transform.position, casaObjetivo.position);
                    if (distancia < radioApagado && Input.GetKeyDown(KeyCode.F))
                    {
                        ApagarFuego();
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }

    IEnumerator EsperarPropagacion()
    {
        yield return new WaitForSeconds(tiempoParaPropagar);

        if (incendioActivo && !yaSePropago)
        {
            PropagarIncendio();
            yaSePropago = true;
        }
    }

    void PropagarIncendio()
    {
        GameObject[] casas = GameObject.FindGameObjectsWithTag("Casa");
        List<Transform> nuevasCasas = new List<Transform>();

        foreach (GameObject casa in casas)
        {
            if (casa.transform == casaObjetivo) continue;

            float distancia = Vector3.Distance(casaObjetivo.position, casa.transform.position);
            if (distancia <= radioPropagacion)
            {
                nuevasCasas.Add(casa.transform);
            }
        }

        foreach (Transform nuevaCasa in nuevasCasas)
        {
            StartCoroutine(EncenderCasaConRetraso(nuevaCasa, Random.Range(2f, 5f)));
        }

        textoAviso.text = "El fuego se ha propagado a casas cercanas.";
    }

    IEnumerator EncenderCasaConRetraso(Transform casa, float retraso)
    {
        yield return new WaitForSeconds(retraso);

        GameObject fuego = Instantiate(fuegoPrefab, casa.position + Vector3.up * 2, Quaternion.identity);
        GameObject humo = Instantiate(humoPrefab, casa.position + Vector3.up * 4, Quaternion.identity);
        Instantiate(marcadorMapa, casa.position + Vector3.up * 5, Quaternion.identity);

        Collider col = fuego.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        incendiosPropagados.Add(fuego); // Añadir el incendio propagado a la lista
    }

    void ApagarFuego()
    {
        // Apagar solo el fuego de la casa original
        if (fuegoInstanciado) Destroy(fuegoInstanciado);
        if (humoInstanciado) Destroy(humoInstanciado);
        if (marcadorInstanciado) Destroy(marcadorInstanciado);
        
        textoAviso.text = "Incendio apagado en " + casaObjetivo.name + ".";
        incendioActivo = false;

        // Apagar también los incendios propagados
        foreach (var incendio in incendiosPropagados)
        {
            Destroy(incendio);
        }
        incendiosPropagados.Clear(); // Limpiar la lista de incendios propagados

        ActualizarColorTexto(false);  // Restaurar el color verde cuando no hay incendios

        // Reproducir sonido de misión superada
        if (sonidoMisionSuperada != null)
        {
            audioSource.PlayOneShot(sonidoMisionSuperada);
        }
    }

    void ActualizarColorTexto(bool hayIncendio)
    {
        if (hayIncendio)
        {
            // Si hay incendios, ponemos el texto a rojo (puedes elegir un tono específico si lo deseas)
            textoAviso.color = colorRojizo;
        }
        else
        {
            // Si no hay incendios, ponemos un gradiente verde
            textoAviso.color = colorVerdeGradiente;
        }
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            if (!estaUsandoChorro)
            {
                chorroAgua = Instantiate(chorroAguaPrefab, puntoDeSalida.position, puntoDeSalida.rotation);
                estaUsandoChorro = true;

                if (sonidoChorroAgua != null)
                {
                    audioSource.PlayOneShot(sonidoChorroAgua, 0.5f);
                }
            }

            tiempoPresionadoF += Time.deltaTime;
            var ps = chorroAgua.GetComponent<ParticleSystem>();
            var main = ps.main;
            
            main.startSize = Mathf.Lerp(0.1f, 0.5f, tiempoPresionadoF);

            var velocityOverLifetime = ps.velocityOverLifetime;
            velocityOverLifetime.x = Mathf.Lerp(1f, 5f, tiempoPresionadoF);

        }
        else if (estaUsandoChorro)
        {
            Destroy(chorroAgua);
            estaUsandoChorro = false;
            tiempoPresionadoF = 0f;
        }
    }
}
