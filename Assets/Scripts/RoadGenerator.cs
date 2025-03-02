using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public GameObject roadStraight;
    public GameObject roadCurve;
    public GameObject fieldPrefab; // Prefab para el campo

    public int roadCount = 30;     // Número total de carreteras
    public int segmentLength = 10; // Longitud de cada segmento

    private Vector2 currentPos;
    private Vector2 currentDirection;

    private Vector2[] directions = new Vector2[]
    {
        new Vector2(0, 1),  // Arriba
        new Vector2(1, 0),  // Derecha
        new Vector2(0, -1), // Abajo
        new Vector2(-1, 0)  // Izquierda
    };

    // Lista para almacenar las posiciones de los segmentos de carretera generados
    private List<Vector2> roadPositions = new List<Vector2>();

    void Start()
    {
        StartCoroutine(GenerateRoads());
    }

    IEnumerator GenerateRoads()
    {
        currentPos = Vector2.zero;
        currentDirection = directions[Random.Range(0, directions.Length)];

        for (int i = 0; i < roadCount; i++)
        {
            if (i % 3 == 0) // Cada 3 segmentos (30 metros) pone una curva
            {
                ChangeDirection();
                PlaceRoad(currentPos, roadCurve, GetRotationCurve(currentDirection));
            }
            else
            {
                PlaceRoad(currentPos, roadStraight, GetRotation(currentDirection));
            }

            roadPositions.Add(currentPos); // Almacenar la posición del segmento
            currentPos += currentDirection * segmentLength;
            yield return null; // Generar instantáneamente
        }

        CheckUnconnectedRoads(); // Verificar si hay carreteras no unidas

        // Coloca el campo alrededor de las intersecciones
        PlaceFieldAroundIntersections();
    }

    void ChangeDirection()
    {
        Vector2 oldDirection = currentDirection;
        while (currentDirection == oldDirection || currentDirection == -oldDirection) 
        {
            currentDirection = directions[Random.Range(0, directions.Length)];
        }
    }

    void PlaceRoad(Vector2 position, GameObject roadPrefab, Quaternion rotation)
    {
        Instantiate(roadPrefab, new Vector3(position.x, 0, position.y), rotation);
    }

    Quaternion GetRotation(Vector2 direction)
    {
        if (direction == new Vector2(0, 1)) return Quaternion.identity;
        if (direction == new Vector2(1, 0)) return Quaternion.Euler(0, 90, 0);
        if (direction == new Vector2(0, -1)) return Quaternion.Euler(0, 180, 0);
        if (direction == new Vector2(-1, 0)) return Quaternion.Euler(0, 270, 0);
        return Quaternion.identity;
    }

    Quaternion GetRotationCurve(Vector2 direction)
    {
        if (direction == new Vector2(1, 0)) return Quaternion.Euler(0, 90, 0);   // Curva derecha
        if (direction == new Vector2(-1, 0)) return Quaternion.Euler(0, -90, 0); // Curva izquierda
        if (direction == new Vector2(0, 1)) return Quaternion.identity;         // Curva arriba
        if (direction == new Vector2(0, -1)) return Quaternion.Euler(0, 180, 0); // Curva abajo
        return Quaternion.identity;
    }

    // Comprobar carreteras no unidas
    void CheckUnconnectedRoads()
    {
        List<Vector2> roadsToConnect = new List<Vector2>(roadPositions);

        // Recorre todas las posiciones generadas
        foreach (var position in roadPositions)
        {
            // Comprueba si hay una conexión válida alrededor de cada posición
            bool isConnected = false;

            foreach (var direction in directions)
            {
                Vector2 neighbor = position + direction * segmentLength;
                if (roadPositions.Contains(neighbor))
                {
                    isConnected = true;
                    break;
                }
            }

            if (!isConnected)
            {
                Debug.LogWarning("Camino sin conectar en la posición: " + position);
                roadsToConnect.Remove(position); // Eliminar la carretera no conectada
            }
        }

        // Intentar conectar los caminos no conectados
        ConnectUnconnectedRoads(roadsToConnect);
    }

    // Método para conectar caminos no conectados
    void ConnectUnconnectedRoads(List<Vector2> roadsToConnect)
    {
        foreach (var road in roadsToConnect)
        {
            // Busca el punto más cercano de la carretera que está conectada
            Vector2 closestConnectedPoint = FindClosestConnectedPoint(road);

            if (closestConnectedPoint != Vector2.zero)
            {
                // Conectar caminos generando una nueva carretera entre ellos
                Vector2 directionToConnect = closestConnectedPoint - road;
                Vector2 newDirection = directionToConnect.normalized; // Normalizar

                // Coloca la nueva carretera para conectar ambos puntos
                PlaceRoad(road, roadStraight, GetRotation(newDirection));
                roadPositions.Add(road); // Añadir la nueva carretera conectada
            }
        }
    }

    // Encuentra el punto más cercano que está conectado
    Vector2 FindClosestConnectedPoint(Vector2 road)
    {
        Vector2 closest = Vector2.zero;
        float minDistance = float.MaxValue;

        foreach (var position in roadPositions)
        {
            float distance = Vector2.Distance(road, position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = position;
            }
        }

        return closest;
    }

    // Colocar el prefab del campo alrededor de las intersecciones
    void PlaceFieldAroundIntersections()
    {
        foreach (var position in roadPositions)
        {
            // Comprobamos si la posición tiene carreteras adyacentes
            foreach (var direction in directions)
            {
                Vector2 neighbor = position + direction * segmentLength;
                if (roadPositions.Contains(neighbor)) // Si hay una carretera adyacente
                {
                    // Colocamos el campo alrededor de esta intersección
                    PlaceFieldAtPosition(position, direction);
                    break; // Solo colocar una vez por intersección
                }
            }
        }
    }

    // Colocar el campo en las posiciones alrededor de la carretera
    void PlaceFieldAtPosition(Vector2 roadPosition, Vector2 roadDirection)
    {
        // Desplazamos el campo alrededor de la carretera con más espacio
        Vector2 offset = roadDirection.normalized * (segmentLength * 2); // Aumentamos el espacio entre el campo y la carretera

        // Colocamos 4 campos alrededor de la intersección, ajustados por un offset
        Vector2[] offsets = new Vector2[]
        {
            roadPosition + offset,                     // Campo en la dirección de la carretera
            roadPosition - offset,                     // Campo en la dirección opuesta
            roadPosition + new Vector2(offset.y, -offset.x), // Campo a la izquierda
            roadPosition - new Vector2(offset.y, -offset.x)  // Campo a la derecha
        };

        foreach (var offsetPos in offsets)
        {
            // Verificar si la posición ya está ocupada por una carretera
            if (!roadPositions.Contains(offsetPos))
            {
                // Instanciar el prefab del campo en las posiciones calculadas
                Instantiate(fieldPrefab, new Vector3(offsetPos.x, 0, offsetPos.y), Quaternion.identity);
            }
        }
    }
}
