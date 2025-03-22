using UnityEngine;
using UnityEngine.AI;

public class CarController : MonoBehaviour
{
    public NavMeshAgent agent;
    private Vector3 randomDestination;
    public float range = 50f; // Rango de la búsqueda aleatoria
    public int walkableAreaMask; // Máscara para la zona Walkable (puedes asignarla desde el inspector)

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomDestination();
    }

    void Update()
    {
        // Si el coche ha llegado al destino o está cerca, cambia la ruta aleatoria
        if (Vector3.Distance(transform.position, randomDestination) < 2f)
        {
            SetRandomDestination();
        }

        // Actualizar el destino del NavMesh Agent
        agent.SetDestination(randomDestination);
    }

    void SetRandomDestination()
    {
        // Generar un punto aleatorio dentro del rango definido
        Vector3 randomPoint = transform.position + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range));

        // Asegurarse de que el destino esté dentro del NavMesh y en el área Walkable
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, range, walkableAreaMask))
        {
            randomDestination = hit.position;
        }
        else
        {
            SetRandomDestination(); // Si no es válido, prueba de nuevo
        }
    }
}
