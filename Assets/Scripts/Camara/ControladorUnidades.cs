using UnityEngine;

public class ControladorUnidades : MonoBehaviour
{
    public static ControladorUnidades Instance;

    public Camera camTactica;
    private UnidadSeleccionable unidadActual;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SeleccionarUnidad(UnidadSeleccionable unidad)
    {
        if (unidadActual != null && unidadActual.camTercera != null)
        {
            // Solo desactivamos la cámara de la unidad anterior si no es null
            unidadActual.camTercera.gameObject.SetActive(false);
        }

        unidadActual = unidad;

        if (camTactica != null)
        {
            // Desactivamos la cámara táctica solo si no es null
            camTactica.gameObject.SetActive(false);
        }

        if (unidadActual != null && unidadActual.camTercera != null)
        {
            // Activamos la cámara de la nueva unidad seleccionada
            unidadActual.camTercera.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (unidadActual != null && unidadActual.camTercera != null && Input.GetKeyDown(KeyCode.C))
        {
            bool estaEnUnidad = unidadActual.camTercera.gameObject.activeSelf;
            unidadActual.camTercera.gameObject.SetActive(!estaEnUnidad);

            if (camTactica != null)
            {
                // Solo activamos o desactivamos la cámara táctica si no es null
                camTactica.gameObject.SetActive(estaEnUnidad);
            }
        }
    }
}
