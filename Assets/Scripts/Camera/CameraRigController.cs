using UnityEngine;
using Unity.Cinemachine;

public class CameraRigController : MonoBehaviour
{
    public float normalSpeed = 10f;
    public float fastSpeed = 30f;
    public float movementTime = 5f;
    public float rotationAmount = 10f;
    public Vector3 zoomAmount = new Vector3(0, -5, 5);

    private Vector3 newPosition;
    private Quaternion newRotation;
    private Vector3 newZoom;

    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private Vector3 rotateStartPosition;
    private Vector3 rotateCurrentPosition;

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
        if (Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }
        }

        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (plane.Raycast(ray, out float entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);
                newPosition += dragStartPosition - dragCurrentPosition;
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(2))
        {
            rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;
            rotateStartPosition = rotateCurrentPosition;
            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));
        }
    }

    void HandleMovementInput()
    {
        float speed = Input.GetKey(KeyCode.LeftShift) ? fastSpeed : normalSpeed;

        if (Input.GetKey(KeyCode.W)) newPosition += transform.forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) newPosition -= transform.forward * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) newPosition += transform.right * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A)) newPosition -= transform.right * speed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q)) newRotation *= Quaternion.Euler(Vector3.up * rotationAmount * Time.deltaTime);
        if (Input.GetKey(KeyCode.E)) newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount * Time.deltaTime);

        if (Input.GetKey(KeyCode.R)) newZoom += zoomAmount * Time.deltaTime;
        if (Input.GetKey(KeyCode.F)) newZoom -= zoomAmount * Time.deltaTime;
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
