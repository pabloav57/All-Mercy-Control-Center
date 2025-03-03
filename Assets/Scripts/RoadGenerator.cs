using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public GameObject roadStraight;
    public GameObject roadCurve;
    public GameObject fieldPrefab;
    public GameObject[] smallBuildings;

    public GameObject[] bigBuildings;

    public GameObject sidewalkPrefab; // Prefab de la acera

    public int roadCount = 30;
    public int segmentLength = 10;

    private Vector2 currentPos;
    private Vector2 currentDirection;

    private List<Vector2> roadPositions = new List<Vector2>();
    private Transform roadsParent;
    private Transform fieldsParent;

    private Vector2[] directions = new Vector2[]
    {
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(0, -1),
        new Vector2(-1, 0)
    };

    void Start()
    {
        roadsParent = new GameObject("Roads").transform;
        fieldsParent = new GameObject("Fields").transform;
        StartCoroutine(GenerateRoads());
    }

IEnumerator GenerateRoads()
{
    currentPos = Vector2.zero;
    currentDirection = directions[Random.Range(0, directions.Length)];

    for (int i = 0; i < roadCount; i++)
    {
        if (i % 3 == 0)
        {
            ChangeDirection();
            PlaceRoad(currentPos, roadCurve, GetRotationCurve(currentDirection));
        }
        else
        {
            PlaceRoad(currentPos, roadStraight, GetRotation(currentDirection));
        }

        roadPositions.Add(currentPos);
        currentPos += currentDirection * segmentLength;
        yield return null;
    }

    CheckUnconnectedRoads();
    PlaceFieldAroundIntersections();

    // Aquí llamamos a la generación de edificios
    BuildingGenerator buildingGen = gameObject.AddComponent<BuildingGenerator>();
    buildingGen.smallBuildings = smallBuildings; // Asegúrate de asignar los prefabs
    buildingGen.bigBuildings = bigBuildings;
    buildingGen.GenerateBuildings(roadPositions); // Llama a la función correcta
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
        GameObject road = Instantiate(roadPrefab, new Vector3(position.x, 0, position.y), rotation);
        road.transform.parent = roadsParent; // Agrupar dentro de "Roads"
        
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
        if (direction == new Vector2(1, 0)) return Quaternion.Euler(0, 90, 0);
        if (direction == new Vector2(-1, 0)) return Quaternion.Euler(0, -90, 0);
        if (direction == new Vector2(0, 1)) return Quaternion.identity;
        if (direction == new Vector2(0, -1)) return Quaternion.Euler(0, 180, 0);
        return Quaternion.identity;
    }

    void CheckUnconnectedRoads()
    {
        List<Vector2> roadsToConnect = new List<Vector2>(roadPositions);

        foreach (var position in roadPositions)
        {
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
                roadsToConnect.Remove(position);
            }
        }

        ConnectUnconnectedRoads(roadsToConnect);
    }

    void ConnectUnconnectedRoads(List<Vector2> roadsToConnect)
    {
        foreach (var road in roadsToConnect)
        {
            Vector2 closestConnectedPoint = FindClosestConnectedPoint(road);

            if (closestConnectedPoint != Vector2.zero)
            {
                Vector2 directionToConnect = closestConnectedPoint - road;
                Vector2 newDirection = directionToConnect.normalized;

                PlaceRoad(road, roadStraight, GetRotation(newDirection));
                roadPositions.Add(road);
            }
        }
    }

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

void PlaceFieldAroundIntersections()
{
    if (roadPositions.Count == 0) return;

    float minX = float.MaxValue, maxX = float.MinValue;
    float minY = float.MaxValue, maxY = float.MinValue;

    foreach (var position in roadPositions)
    {
        if (position.x < minX) minX = position.x;
        if (position.x > maxX) maxX = position.x;
        if (position.y < minY) minY = position.y;
        if (position.y > maxY) maxY = position.y;
    }

    minX -= segmentLength;
    maxX += segmentLength;
    minY -= segmentLength;
    maxY += segmentLength;

    for (float x = minX; x <= maxX; x += segmentLength)
    {
        for (float y = minY; y <= maxY; y += segmentLength)
        {
            Vector2 pos = new Vector2(x, y);

            if (!roadPositions.Contains(pos))
            {
                GameObject field = Instantiate(fieldPrefab, new Vector3(x, 0, y), Quaternion.identity);
                field.transform.parent = fieldsParent; // Agrupar dentro de "Fields"
            }
        }
    }
}

    void PlaceFieldAtPosition(Vector2 roadPosition, Vector2 roadDirection)
    {
        Vector2 offset = roadDirection.normalized * (segmentLength * 2);

        Vector2[] offsets = new Vector2[]
        {
            roadPosition + offset,
            roadPosition - offset,
            roadPosition + new Vector2(offset.y, -offset.x),
            roadPosition - new Vector2(offset.y, -offset.x)
        };

        foreach (var offsetPos in offsets)
        {
            if (!roadPositions.Contains(offsetPos))
            {
                GameObject field = Instantiate(fieldPrefab, new Vector3(offsetPos.x, 0, offsetPos.y), Quaternion.identity);
                field.transform.parent = fieldsParent; // Agrupar dentro de "Fields"
            }
        }
    }

    void PlaceSidewalks(Vector2 position, Vector2 roadDirection)
{
    // Calculamos los vectores perpendiculares a la dirección de la carretera
    Vector2 perpendicular1 = new Vector2(-roadDirection.y, roadDirection.x) * (segmentLength / 2);
    Vector2 perpendicular2 = new Vector2(roadDirection.y, -roadDirection.x) * (segmentLength / 2);

    // Calculamos las posiciones a izquierda y derecha de la carretera
    Vector2 leftSidewalkPos = position + perpendicular1;
    Vector2 rightSidewalkPos = position + perpendicular2;

    // Instanciamos las aceras a ambos lados
    Instantiate(sidewalkPrefab, new Vector3(leftSidewalkPos.x, 0, leftSidewalkPos.y), Quaternion.identity, roadsParent);
    Instantiate(sidewalkPrefab, new Vector3(rightSidewalkPos.x, 0, rightSidewalkPos.y), Quaternion.identity, roadsParent);
}
}
