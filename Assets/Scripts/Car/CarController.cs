using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Movimiento")]
    public float acceleration = 1000f;
    public float steering = 100f;
    public float brakeForce = 2000f;
    public float handbrakeForce = 5000f;
    public float maxSpeed = 20f;

    [Header("Referencias de ruedas")]
    public Transform ruedaDelanteraIzquierda;
    public Transform ruedaDelanteraDerecha;
    public Transform ruedaTraseraIzquierda;
    public Transform ruedaTraseraDerecha;

    [Header("Control externo")]
    public bool inputActivo = false;

    [Header("Sonido del motor")]
    public AudioSource engineSound;
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;

    private Rigidbody rb;
    private float currentSteering = 0f;
    private float currentSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0, -0.5f, 0); // Ajustar el centro de masa para estabilidad

        if (engineSound && !engineSound.isPlaying)
        {
            engineSound.loop = true;
            engineSound.Play();
        }
    }

    void Update()
    {
        if (!inputActivo) return;

        HandleInput();
    }

    void FixedUpdate()
    {
        if (!inputActivo) return;

        MoveCar();
        UpdateEngineSound();
        RotateWheels(); // Llamada para girar las ruedas
    }

    void HandleInput()
    {
        currentSpeed = Input.GetAxis("Vertical");
        currentSteering = Input.GetAxis("Horizontal");
    }

    void MoveCar()
    {
        // Aceleración
        if (currentSpeed != 0 && rb.linearVelocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.forward * currentSpeed * acceleration * Time.deltaTime);
        }

        // Dirección (giro solo si hay velocidad)
        if (Mathf.Abs(currentSteering) > 0.1f && rb.linearVelocity.magnitude > 0.1f)
        {
            float turn = currentSteering * steering * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }

        // Freno de mano (aplicar fuerza en dirección contraria)
        if (Input.GetKey(KeyCode.Space))
        {
            Vector3 oppositeForce = -rb.linearVelocity.normalized * handbrakeForce * Time.deltaTime;
            rb.AddForce(oppositeForce, ForceMode.Acceleration);
        }
    }

    void UpdateEngineSound()
    {
        if (!engineSound) return;

        // Actualización de sonido del motor según la velocidad
        float speedPercent = rb.linearVelocity.magnitude / maxSpeed;
        engineSound.pitch = Mathf.Lerp(minPitch, maxPitch, speedPercent);
    }

    void RotateWheels()
    {
        // Rotar las ruedas delanteras según la dirección del volante
        if (ruedaDelanteraIzquierda != null && ruedaDelanteraDerecha != null)
        {
            float steeringAngle = currentSteering * steering;  // Ángulo de giro
            ruedaDelanteraIzquierda.localRotation = Quaternion.Euler(0f, steeringAngle, 0f); 
            ruedaDelanteraDerecha.localRotation = Quaternion.Euler(0f, steeringAngle, 0f); 
        }

        // Rotar las ruedas (delante y atrás) en función de la velocidad
        if (ruedaDelanteraIzquierda != null)
        {
            float wheelRotationSpeed = rb.linearVelocity.magnitude * 10f; 
            ruedaDelanteraIzquierda.Rotate(wheelRotationSpeed * Time.deltaTime, 0f, 0f); 
            ruedaDelanteraDerecha.Rotate(wheelRotationSpeed * Time.deltaTime, 0f, 0f);
        }

        if (ruedaTraseraIzquierda != null)
        {
            float wheelRotationSpeed = rb.linearVelocity.magnitude * 10f; 
            ruedaTraseraIzquierda.Rotate(wheelRotationSpeed * Time.deltaTime, 0f, 0f); 
            ruedaTraseraDerecha.Rotate(wheelRotationSpeed * Time.deltaTime, 0f, 0f);
        }
    }
}
