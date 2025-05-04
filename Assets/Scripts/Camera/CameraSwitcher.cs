using UnityEngine;
using Unity.Cinemachine;
using System.Collections.Generic;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera camTactica;

    private List<GameObject> unidades = new List<GameObject>();
    private List<CinemachineCamera> camarasUnidad = new List<CinemachineCamera>();
    private List<CarController> controladoresUnidad = new List<CarController>();

    private int indiceUnidadActual = 0;
    private bool enCamaraTactica = true;

    private const int prioridadActiva = 20;
    private const int prioridadBaja = 10;

    private string nombreUnidadActual = "";
    private string modoCamaraActual = "Vista táctica";

    public static bool EsVistaTactica { get; private set; } = true;

    void Start()
    {
        // Llamada a la función para cargar las unidades inicialmente
        ActualizarUnidades();
        ActivarCamaraTactica();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (enCamaraTactica)
                ActivarCamaraUnidad();
            else
                ActivarCamaraTactica();
        }

        if (!enCamaraTactica)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift))
            {
                CambiarUnidadSiguiente();
            }
            else if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                CambiarUnidadAnterior();
            }
        }
    }

    void ActivarCamaraTactica()
    {
        camTactica.Priority = prioridadActiva;

        for (int i = 0; i < camarasUnidad.Count; i++)
        {
            camarasUnidad[i].Priority = prioridadBaja;
            controladoresUnidad[i].inputActivo = false;
        }

        enCamaraTactica = true;
        EsVistaTactica = true;

        modoCamaraActual = "Vista táctica";
        nombreUnidadActual = "";

        if (CameraRigController.instance != null)
            CameraRigController.instance.ActivarInput(true);
    }

    void ActivarCamaraUnidad()
    {
        camTactica.Priority = prioridadBaja;

        for (int i = 0; i < camarasUnidad.Count; i++)
        {
            camarasUnidad[i].Priority = prioridadBaja;
            controladoresUnidad[i].inputActivo = false;
        }

        camarasUnidad[indiceUnidadActual].Priority = prioridadActiva;
        controladoresUnidad[indiceUnidadActual].inputActivo = true;

        enCamaraTactica = false;
        EsVistaTactica = false;

        nombreUnidadActual = unidades[indiceUnidadActual].name;
        modoCamaraActual = "Vista de unidad";

        if (CameraRigController.instance != null)
            CameraRigController.instance.ActivarInput(false);
    }

    void CambiarUnidadSiguiente()
    {
        camarasUnidad[indiceUnidadActual].Priority = prioridadBaja;
        controladoresUnidad[indiceUnidadActual].inputActivo = false;

        indiceUnidadActual = (indiceUnidadActual + 1) % unidades.Count;

        camarasUnidad[indiceUnidadActual].Priority = prioridadActiva;
        controladoresUnidad[indiceUnidadActual].inputActivo = true;
        nombreUnidadActual = unidades[indiceUnidadActual].name;

        Debug.Log("Cambiada a unidad: " + nombreUnidadActual);
    }

    void CambiarUnidadAnterior()
    {
        camarasUnidad[indiceUnidadActual].Priority = prioridadBaja;
        controladoresUnidad[indiceUnidadActual].inputActivo = false;

        indiceUnidadActual = (indiceUnidadActual - 1 + unidades.Count) % unidades.Count;

        camarasUnidad[indiceUnidadActual].Priority = prioridadActiva;
        controladoresUnidad[indiceUnidadActual].inputActivo = true;
        nombreUnidadActual = unidades[indiceUnidadActual].name;

        Debug.Log("Cambiada a unidad: " + nombreUnidadActual);
    }

    void ActualizarUnidades()
    {
        // Encuentra todas las unidades con la etiqueta "Unidad"
        GameObject[] encontrados = GameObject.FindGameObjectsWithTag("Unidad");
        Debug.Log("Unidades encontradas: " + encontrados.Length);

        // Agregar unidades a las listas
        foreach (var unidad in encontrados)
        {
            if (!unidades.Contains(unidad)) // Evitar duplicados
            {
                var camTransform = unidad.transform.Find("VCam_TerceraPersona");

                if (camTransform != null)
                {
                    var camara = camTransform.GetComponent<CinemachineCamera>();
                    var controlador = unidad.GetComponent<CarController>();

                    if (camara != null && controlador != null)
                    {
                        unidades.Add(unidad);
                        camarasUnidad.Add(camara);
                        controladoresUnidad.Add(controlador);
                        camara.Priority = prioridadBaja;
                    }
                    else
                    {
                        Debug.LogWarning($"Unidad '{unidad.name}' no tiene cámara o controlador válidos.");
                    }
                }
                else
                {
                    Debug.Log($"No se encontró 'VCam_TerceraPersona' en: {unidad.name}");
                }
            }
        }

        if (unidades.Count == 0)
        {
            Debug.LogError("No se encontraron unidades con cámaras válidas.");
        }
    }
    public void ActualizarUnidadesYCamaras()
{
    // Limpiar las listas
    unidades.Clear();
    camarasUnidad.Clear();
    controladoresUnidad.Clear();

    // Buscar todas las unidades de nuevo
    GameObject[] encontrados = GameObject.FindGameObjectsWithTag("Unidad");
    Debug.Log("Unidades encontradas: " + encontrados.Length);

    foreach (var unidad in encontrados)
    {
        var camTransform = unidad.transform.Find("VCam_TerceraPersona");

        if (camTransform != null)
        {
            var camara = camTransform.GetComponent<CinemachineCamera>();
            var controlador = unidad.GetComponent<CarController>();

            if (camara != null && controlador != null)
            {
                unidades.Add(unidad);
                camarasUnidad.Add(camara);
                controladoresUnidad.Add(controlador);
                camara.Priority = prioridadBaja;
            }
            else
            {
                Debug.LogWarning($"Unidad '{unidad.name}' no tiene cámara o controlador válidos.");
            }
        }
        else
        {
            Debug.Log($"No se encontró 'VCam_TerceraPersona' en: {unidad.name}");
        }
    }

    if (unidades.Count == 0)
    {
        Debug.LogError("No se encontraron unidades con cámaras válidas.");
    }
}


    void OnGUI()
    {
        GUIStyle estilo = new GUIStyle(GUI.skin.label);
        estilo.fontSize = 20;
        estilo.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 20, 400, 30), "Modo actual: " + modoCamaraActual, estilo);

        if (!enCamaraTactica && !string.IsNullOrEmpty(nombreUnidadActual))
        {
            GUI.Label(new Rect(20, 50, 400, 30), "Unidad actual: " + nombreUnidadActual, estilo);

            if (controladoresUnidad[indiceUnidadActual] != null)
            {
                float velocidadMps = controladoresUnidad[indiceUnidadActual].GetComponent<Rigidbody>().linearVelocity.magnitude;
                float velocidadKmh = velocidadMps * 3.6f;
                GUI.Label(new Rect(20, 80, 400, 30), "Velocidad: " + velocidadKmh.ToString("F2") + " km/h", estilo);
            }
        }
    }
}
