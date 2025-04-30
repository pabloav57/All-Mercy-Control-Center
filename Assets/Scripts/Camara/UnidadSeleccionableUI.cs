using UnityEngine;
using UnityEngine.UI;

public class UnidadSeleccionableUI : MonoBehaviour
{
    public UnidadSeleccionable unidad;

    public void SeleccionarUnidad()
    {
        ControladorUnidades.Instance.SeleccionarUnidad(unidad);
    }
}