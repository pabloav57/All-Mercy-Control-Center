using UnityEngine;

public class Unidad : MonoBehaviour
{
    public string tipoUnidad;  // Tipo de unidad: "Policia", "Bombero", "Ambulancia", ahora es estática

    void Start()
    {
        // Solo asignar el tipo de unidad si aún no está asignado
        if (string.IsNullOrEmpty(tipoUnidad))
        {
            AsignarTipoUnidad();
        }
    }

    private void AsignarTipoUnidad()
    {
        // Verificar variaciones del nombre usando el operador OR
        if (gameObject.name.Contains("Policia") || gameObject.name.Contains("Police"))
        {
            tipoUnidad = "Policia";
        }
        else if (gameObject.name.Contains("Bombero")|| gameObject.name.Contains("Fire"))
        {
            tipoUnidad = "Bombero";
        }
        else if (gameObject.name.Contains("Ambulancia")|| gameObject.name.Contains("Ambulance"))
        {
            tipoUnidad = "Ambulancia";
        }
        else
        {
            tipoUnidad = "Desconocido";  // Por si no coincide con ninguna unidad
        }

        Debug.Log("Tipo de unidad asignado: " + tipoUnidad);
    }
}