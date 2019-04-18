using UnityEngine;

public class CameraController : MonoBehaviour
{

    public float panSpeed = 20f;
    public float panBorderThickness = 30f;

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = transform.position;

        if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
            pos.x += panSpeed * Time.deltaTime;
        }

        if (Input.GetKey("s") || Input.mousePosition.y <= panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
            pos.x -= panSpeed * Time.deltaTime;

        }

        if (Input.GetKey("a") || Input.mousePosition.x <= panBorderThickness)
        {
            pos.z += panSpeed * Time.deltaTime;
            pos.x -= panSpeed * Time.deltaTime;

        }

        if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - panBorderThickness)
        {
            pos.z -= panSpeed * Time.deltaTime;
            pos.x += panSpeed * Time.deltaTime;

        }


        transform.position = pos;
        
    }
}
