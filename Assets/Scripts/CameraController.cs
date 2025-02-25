using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 10f; // Velocidad de movimiento
    public float zoomSpeed = 5f; // Velocidad de zoom
    public float minZoom = 10f; // Zoom mínimo
    public float maxZoom = 50f; // Zoom máximo

    void Update()
    {
        // Mover la cámara siguiendo la dirección del ratón
        Vector3 mousePosition = Input.mousePosition;
        Vector3 screenPoint = new Vector3(mousePosition.x, mousePosition.y, 0);
        
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        Vector3 worldPoint = ray.origin + ray.direction * 10f; // Ajusta la distancia según la escena

        // Ajusta solo el eje X y Z
        transform.position = Vector3.Lerp(transform.position, worldPoint, Time.deltaTime * panSpeed);

        // Control de zoom con la rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - scroll * zoomSpeed, minZoom, maxZoom);
    }
}
