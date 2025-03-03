using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject[] smallBuildings;
    public GameObject[] bigBuildings;

    private List<Vector2> roadPositions;
    private Transform buildingsParent;

    public void GenerateBuildings(List<Vector2> roadPositions)
    {
        this.roadPositions = roadPositions;
        buildingsParent = new GameObject("Buildings").transform;

        StartCoroutine(PlaceBuildings());
    }

    IEnumerator PlaceBuildings()
    {
        foreach (var road in roadPositions)
        {
            PlaceBuildingNearRoad(road);
            yield return null;
        }
    }

    void PlaceBuildingNearRoad(Vector2 roadPosition)
    {
        Vector2[] possiblePositions = new Vector2[]
        {
            roadPosition + new Vector2(10, 0),
            roadPosition + new Vector2(-10, 0),
            roadPosition + new Vector2(0, 10),
            roadPosition + new Vector2(0, -10)
        };

        foreach (var pos in possiblePositions)
        {
            if (!roadPositions.Contains(pos)) // Solo colocar edificios fuera de la carretera
            {
                GameObject buildingPrefab = GetRandomBuilding();
                GameObject building = Instantiate(buildingPrefab, new Vector3(pos.x, 0, pos.y), Quaternion.identity);
                building.transform.parent = buildingsParent;
                return;
            }
        }
    }

    GameObject GetRandomBuilding()
    {
        if (Random.value > 0.5f)
            return smallBuildings[Random.Range(0, smallBuildings.Length)];
        else
            return bigBuildings[Random.Range(0, bigBuildings.Length)];
    }
}
