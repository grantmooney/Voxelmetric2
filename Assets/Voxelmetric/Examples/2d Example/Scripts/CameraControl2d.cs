using UnityEngine;

public class CameraControl2d : MonoBehaviour
{
    Vector2 rot;

    void Update()
    {
        transform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) * Time.deltaTime * 50;
    }
}
