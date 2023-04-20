using UnityEngine;

public class Utils : MonoBehaviour
{
    public static Vector3 ScreenToWorld(Camera camera, Vector3 position)
    {
        //position.z = camera.nearClipPlane;
        position.z = -Camera.main.transform.position.z;
        return camera.ScreenToWorldPoint(position);
    }

}
