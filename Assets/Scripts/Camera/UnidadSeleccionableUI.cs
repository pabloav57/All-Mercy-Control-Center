using UnityEngine;

public class UnidadSeleccionableUI : MonoBehaviour
{
    public UnidadSeleccionable unidad;

    public void SeleccionarUnidad()
    {
        ControladorUnidades.Instance.SeleccionarUnidad(unidad);
        CameraRigController.instance.Enfocar(unidad.transform.position); // ENFOCA
    }
}