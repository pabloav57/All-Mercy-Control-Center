using UnityEngine;

public class NPCMover : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 5f;
    private int currentWaypoint = 0;

    void Update()
    {
        if (waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypoint];
        Vector3 direction = (target.position - transform.position).normalized;

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 5f * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length; // ruta en bucle
        }
    }
}
