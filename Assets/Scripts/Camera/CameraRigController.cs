using UnityEngine;
using Unity.Cinemachine;

public class CameraRigController : MonoBehaviour
{
    public float normalSpeed = 10f;
    public float fastSpeed = 30f;
    public float movementTime = 5f;
    public float rotationAmount = 10f;
    public Vector3 zoomAmount = new Vector3(0, -5, 5);
    public float zoomSpeed = 2f; // Ajustar la velocidad del zoom
    public float rotationSpeed = 5f; // Velocidad de la rotación de la cámara con el ratón

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    private float currentPitch = 0f; // Ángulo de rotación en el eje X (vertical)
    private float currentYaw = 0f; // Ángulo de rotación en el eje Y (horizontal)

    public static CameraRigController instance;

    [HideInInspector]
    public bool inputActivo = true; // 🔑 Permite bloquear el control de la cámara

    private CinemachineCamera thirdPersonVirtualCamera; // Referencia a la cámara virtual de tercera persona

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = new Vector3(0, 0, 0);

        // Buscar la cámara virtual de tercera persona por su Tag
        GameObject thirdPersonCameraObject = GameObject.FindGameObjectWithTag("VCam_TerceraPersona");

        if (thirdPersonCameraObject != null)
        {
            thirdPersonVirtualCamera = thirdPersonCameraObject.GetComponent<CinemachineCamera>();
        }

        if (thirdPersonVirtualCamera == null)
        {
            Debug.LogError("No se ha encontrado una cámara virtual con el tag 'VCam_TerceraPersona'.");
        }
    }

    private void Update()
    {
        if (!inputActivo || thirdPersonVirtualCamera == null) return; // 🔐 Bloquea entrada si no está activa

        HandleMouseInput();
        HandleMovementInput();

        // Aplicar las transformaciones a la cámara y a la posición
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }

    void HandleMouseInput()
    {
        // Manejo del zoom con la rueda del ratón
        if (Input.mouseScrollDelta.y != 0)
        {
            // Control de zoom: desplazamiento rueda del ratón
            newZoom += Input.mouseScrollDelta.y * zoomAmount; 
        }

        // Manejo de la rotación de la cámara con el ratón (horizontales y verticales)
        if (Input.GetMouseButton(1)) // Botón derecho del ratón para girar la cámara
        {
            // Calcular el desplazamiento del ratón
            float mouseX = Input.GetAxis("Mouse X"); 
            float mouseY = Input.GetAxis("Mouse Y"); 

            
            currentYaw += mouseX * rotationSpeed; 
            currentPitch -= mouseY * rotationSpeed; 

            // Limitar la rotación vertical para evitar que la cámara gire demasiado
            currentPitch = Mathf.Clamp(currentPitch, -80f, 80f);

            // Aplicar la rotación a la cámara
            newRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        }
    }

    void HandleMovementInput()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;

        // Movimiento de la cámara
        if (Input.GetKey(KeyCode.W)) newPosition += transform.forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) newPosition -= transform.forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) newPosition += transform.right * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) newPosition -= transform.right * speed * Time.deltaTime;

        // Rotación de la cámara con las teclas Q y E
        if (Input.GetKey(KeyCode.Q)) newRotation *= Quaternion.Euler(Vector3.up * rotationAmount * Time.deltaTime);
        if (Input.GetKey(KeyCode.E)) newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount * Time.deltaTime);
    }

    public void Enfocar(Vector3 posicionObjetivo)
    {
        newPosition = new Vector3(posicionObjetivo.x, newPosition.y, posicionObjetivo.z);
    }

    //Método para activar/desactivar control de la cámara táctica
    public void ActivarInput(bool activo)
    {
        inputActivo = activo; // Controla si el input está activo
    }
}
