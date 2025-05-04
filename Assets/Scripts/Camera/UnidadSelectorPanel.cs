using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnidadSelectorPanel : MonoBehaviour
{
    public GameObject botonPrefab;

    public Transform contenedorPolicia;
    public Transform contenedorBomberos;
    public Transform contenedorAmbulancias;

    public GameObject prefabPolicia;
    public GameObject prefabBombero;
    public GameObject prefabAmbulancia;

    public GameObject puntoDestinoPrefab;  // Prefab del punto que el jugador puede seleccionar

    private GameObject puntoDestinoActual; // Para almacenar el punto de destino actual

    public CameraSwitcher cameraSwitcher;

    void Start()
    {
        MostrarUnidadesExistentes();
    }

    void MostrarUnidadesExistentes()
    {
        Debug.Log("→ Mostrando unidades existentes...");

        RellenarPanel("Policia", contenedorPolicia);
        RellenarPanel("Bombero", contenedorBomberos);
        RellenarPanel("Ambulancia", contenedorAmbulancias);
    }

    void RellenarPanel(string tipoUnidad, Transform contenedor)
    {
        foreach (Transform child in contenedor)
            Destroy(child.gameObject);

        GameObject[] unidades = GameObject.FindGameObjectsWithTag("Unidad");

        foreach (GameObject unidad in unidades)
        {
            Unidad unidadScript = unidad.GetComponent<Unidad>();
            if (unidadScript == null)
                continue;

            if (unidadScript.tipoUnidad == tipoUnidad)
            {
                GameObject nuevoBoton = Instantiate(botonPrefab, contenedor);
                nuevoBoton.GetComponentInChildren<TextMeshProUGUI>().text = unidad.name;

                UnidadSeleccionable seleccionable = unidad.GetComponent<UnidadSeleccionable>();
                UnidadSeleccionableUI uiScript = nuevoBoton.GetComponent<UnidadSeleccionableUI>();

                if (uiScript != null && seleccionable != null)
                {
                    uiScript.unidad = seleccionable;
                }
                else
                {
                    Debug.LogWarning($"[UI] Falta componente en {unidad.name}");
                }
            }
        }
    }

    public void LlamarUnidad(string tipo)
    {
        // Crear un punto de destino para que el jugador pueda seleccionar donde quiere que aparezca la unidad
        if (puntoDestinoActual != null)
        {
            Destroy(puntoDestinoActual);  // Eliminar el punto anterior si existiera
        }

        // Crear un nuevo punto de destino en el centro del mapa (por ejemplo)
        puntoDestinoActual = Instantiate(puntoDestinoPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        puntoDestinoActual.SetActive(true); // Asegurarse de que el punto está visible para el jugador

        // Esperar a que el jugador haga clic en el mapa
        StartCoroutine(WatingForClick(tipo));
    }

    private System.Collections.IEnumerator WatingForClick(string tipo)
    {
        // Mientras el jugador no haga clic, seguimos esperando
        while (true)
        {
            if (Input.GetMouseButtonDown(0)) // Detectar clic izquierdo
            {
                // Raycast desde la cámara hacia el mapa para obtener la posición del clic
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    // Obtener la posición donde el jugador hizo clic y mover el punto de destino a esa posición
                    Vector3 posicionDestino = hit.point;

                    // Crear la unidad en la posición seleccionada
                    CrearUnidad(tipo, posicionDestino);

                    // Destruir el punto de destino después de haber creado la unidad
                    Destroy(puntoDestinoActual);
                    break;
                }
            }
            yield return null;
        }
    }

private void CrearUnidad(string tipo, Vector3 posicion)
{
    GameObject nuevo = null;

    switch (tipo)
    {
        case "Policia":
            nuevo = Instantiate(prefabPolicia, posicion, Quaternion.identity);
            nuevo.GetComponent<Unidad>().tipoUnidad = "Policia"; // Forzamos asignación
            break;
        case "Bombero":
            nuevo = Instantiate(prefabBombero, posicion, Quaternion.identity);
            nuevo.GetComponent<Unidad>().tipoUnidad = "Bombero";
            break;
        case "Ambulancia":
            nuevo = Instantiate(prefabAmbulancia, posicion, Quaternion.identity);
            nuevo.GetComponent<Unidad>().tipoUnidad = "Ambulancia";
            break;
    }

    if (nuevo != null)
    {
        MostrarUnidadesExistentes();

        // Llamar a la actualización de las cámaras
        if (cameraSwitcher != null)
        {
            cameraSwitcher.ActualizarUnidadesYCamaras();
        }
    }
}
}
