using UnityEngine;

public class CarController : MonoBehaviour
{
    public float acceleration = 1000f;
    public float steering = 200f;
    public float brakeForce = 2000f;
    public float handbrakeForce = 5000f;  // Fuerza del freno de mano
    public float maxSpeed = 20f;
    public Transform frontWheel;
    public Transform rearWheel;

    public bool inputActivo = false; // ✅ Control externo desde CameraSwitcher

    private Rigidbody rb;
    private float currentSteering = 0f;
    private float currentSpeed = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!inputActivo) return; // Bloquea la entrada si está desactivado

        HandleInput();
    }

    void FixedUpdate()
    {
        if (!inputActivo) return; // Evita mover el coche si no hay control

        MoveCar();
    }

    void HandleInput()
    {
        currentSpeed = Input.GetAxis("Vertical");
        currentSteering = Input.GetAxis("Horizontal");
    }

    void MoveCar()
    {
        // Movimiento normal del coche
        if (currentSpeed != 0)
        {
            if (rb.linearVelocity.magnitude < maxSpeed)
            {
                rb.AddForce(transform.forward * currentSpeed * acceleration * Time.deltaTime);
            }
        }

        // Giro del coche
        if (currentSteering != 0)
        {
            transform.Rotate(0, currentSteering * steering * Time.deltaTime, 0);
        }

        // Freno de mano
        if (Input.GetKey(KeyCode.Space))
        {
            // Aplicar una fuerza de frenado en la dirección opuesta al movimiento
            Vector3 handbrakeDirection = -rb.linearVelocity.normalized; // Dirección opuesta al movimiento
            rb.AddForce(handbrakeDirection * handbrakeForce * Time.deltaTime);
        }
        else
        {
            // No se aplica frenado cuando no se presiona el freno de mano
            rb.linearDamping = 1f; // Ajusta el arrastre si es necesario
        }
    }
}
