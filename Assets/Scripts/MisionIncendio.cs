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

    public float radioApagado = 5f;
    public float tiempoParaPropagar = 30f;
    public float radioPropagacion = 15f;

    private GameObject fuegoInstanciado;
    private GameObject humoInstanciado;
    private GameObject marcadorInstanciado;
    private Transform casaObjetivo;
    private bool incendioActivo = false;
    private bool yaSePropago = false;

    void Start()
    {
        StartCoroutine(GenerarIncendioCada(60f));
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
    }

    void ApagarFuego()
    {
        if (fuegoInstanciado) Destroy(fuegoInstanciado);
        if (humoInstanciado) Destroy(humoInstanciado);
        if (marcadorInstanciado) Destroy(marcadorInstanciado);
        textoAviso.text = "Incendio apagado.";
        incendioActivo = false;
    }
}
