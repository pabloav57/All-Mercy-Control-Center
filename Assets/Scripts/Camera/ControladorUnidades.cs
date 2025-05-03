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
        if (unidadActual != null)
        {
            unidadActual.camTercera.gameObject.SetActive(false);
        }

        unidadActual = unidad;
        camTactica.gameObject.SetActive(false);
        unidadActual.camTercera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (unidadActual != null && Input.GetKeyDown(KeyCode.C))
        {
            bool estaEnUnidad = unidadActual.camTercera.gameObject.activeSelf;
            unidadActual.camTercera.gameObject.SetActive(!estaEnUnidad);
            camTactica.gameObject.SetActive(estaEnUnidad);
        }
    }
}