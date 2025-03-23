using UnityEngine;

public class CarController : MonoBehaviour
{
    public void OnMouseDown()
    {
        CameraController.instance.followTransform = transform;
    }
}
