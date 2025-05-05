using UnityEngine;
using UnityEngine.AI;

public class TraficoNavMesh : MonoBehaviour
{
    public GameObject cochePrefab;
    public int cantidadDeCoches = 4;
    public Transform[] puntosInicio;
    public Transform[] waypoints;

    private GameObject[] coches;
    private NavMeshAgent[] agents;

    void Start()
    {
        if (cochePrefab == null || puntosInicio.Length == 0 || waypoints.Length == 0)
        {
            Debug.LogError("Faltan asignaciones en el inspector.");
            return;
        }

        coches = new GameObject[cantidadDeCoches];
        agents = new NavMeshAgent[cantidadDeCoches];
        CrearCoches();
    }

    void CrearCoches()
    {
        for (int i = 0; i < cantidadDeCoches; i++)
        {
            Transform puntoInicio = puntosInicio[Random.Range(0, puntosInicio.Length)];
            coches[i] = Instantiate(cochePrefab, puntoInicio.position, Quaternion.identity);
            coches[i].name = "Coche_" + i;

            NavMeshAgent agent = coches[i].GetComponent<NavMeshAgent>();
            if (agent == null)
            {
                Debug.LogError("No se encontró NavMeshAgent en " + coches[i].name);
                continue;
            }

            agent.speed = 5f;
            agent.acceleration = 10f;
            agent.angularSpeed = 120f;
            agents[i] = agent;

            // Asignar ruta única para cada coche
            int inicioIndex = Random.Range(0, waypoints.Length);
            bool direccion = Random.Range(0, 2) == 0; // true = adelante, false = reversa
            StartCoroutine(SeguimientoRuta(i, inicioIndex, direccion));
        }
    }

    System.Collections.IEnumerator SeguimientoRuta(int cocheIndex, int waypointIndex, bool isGoingForward)
    {
        while (true)
        {
            if (waypoints.Length == 0) yield break;

            agents[cocheIndex].destination = waypoints[waypointIndex].position;

            while (Vector3.Distance(coches[cocheIndex].transform.position, waypoints[waypointIndex].position) > 0.5f)
                yield return null;

            // Espera un poco para simular parada, semáforo o tráfico
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));

            if (isGoingForward)
            {
                waypointIndex++;
                if (waypointIndex >= waypoints.Length)
                {
                    waypointIndex = waypoints.Length - 2;
                    isGoingForward = false;
                }
            }
            else
            {
                waypointIndex--;
                if (waypointIndex < 0)
                {
                    waypointIndex = 1;
                    isGoingForward = true;
                }
            }
        }
    }
}
