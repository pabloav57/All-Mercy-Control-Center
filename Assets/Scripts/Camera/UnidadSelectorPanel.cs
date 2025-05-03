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

    public void CrearUnidad(string tipo)
    {
        Vector3 posicion = new Vector3(Random.Range(-20, 20), 0, Random.Range(-20, 20));
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
        }
    }
}
