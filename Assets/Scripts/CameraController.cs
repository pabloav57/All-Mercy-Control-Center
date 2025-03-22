using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // Velocidad de movimiento
    public float zoomSpeed = 5f; // Velocidad de zoom
    public float minZoom = 10f; // Zoom mínimo
    public float maxZoom = 50f; // Zoom máximo
    public float rotationSpeed = 3f; // Velocidad de rotación
    public float minRotationX = 10f; // Rotación mínima en el eje X (ángulo)
    public float maxRotationX = 50f; // Rotación máxima en el eje X (ángulo)

    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.position;
    }

    private void Update()
    {
        // Movimiento de la cámara con las teclas W, A, S, D (y Q, E para subir/bajar)
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // A/D para moverse en el eje X
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime; // W/S para moverse en el eje Z
        float moveY = 0f;

        // Subir y bajar con Q y E
        if (Input.GetKey(KeyCode.Q))
            moveY = -moveSpeed * Time.deltaTime; // Subir
        else if (Input.GetKey(KeyCode.E))
            moveY = moveSpeed * Time.deltaTime; // Bajar

        // Actualiza la posición de la cámara
        transform.Translate(moveX, moveY, moveZ);

        // Control de rotación con el ratón (mantén el botón derecho para rotar)
        if (Input.GetMouseButton(1)) // Botón derecho para rotar
        {
            float rotationX = -Input.GetAxis("Mouse Y") * rotationSpeed; // Rotación vertical
            float rotationY = Input.GetAxis("Mouse X") * rotationSpeed; // Rotación horizontal

            // Limitar la rotación en el eje X para evitar la rotación extrema
            Vector3 currentRotation = transform.eulerAngles;
            currentRotation.x = Mathf.Clamp(currentRotation.x + rotationX, minRotationX, maxRotationX); // Limita la rotación en el eje X
            transform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y + rotationY, currentRotation.z);
        }

        // Control de zoom con la rueda del ratón
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - scroll * zoomSpeed, minZoom, maxZoom);
    }
}
