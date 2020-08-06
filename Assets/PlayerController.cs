using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public float scrollSpeed = 20f;
    public float minY = 10f;
    public float maxY = 20f;

    private Transform camTransform;
    private Quaternion GetCameraTurn() {
        return Quaternion.AngleAxis(camTransform.rotation.eulerAngles.y, Vector3.up);
    }
    void Start() {
        camTransform = Camera.main.transform;
    }

    void Update() {
        Vector3 position = transform.position;

        if(Input.GetKey("w") /*|| Input.mousePosition.y >= Screen.height - panBorderThickness*/) {
            //transform.position += transform.forward * panSpeed * Time.deltaTime;
            position += transform.forward * panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("a")  /*|| Input.mousePosition.x <= panBorderThickness*/) {
            //transform.position -= transform.right * panSpeed * Time.deltaTime;
            position -= transform.right * panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("s") /*|| Input.mousePosition.y <= panBorderThickness*/) {
            //transform.position -= transform.forward * panSpeed * Time.deltaTime;
            position -= transform.forward * panSpeed * Time.deltaTime;
        }
        if(Input.GetKey("d")  /*|| Input.mousePosition.x >= Screen.width - panBorderThickness*/) {
            //transform.position += transform.right * panSpeed * Time.deltaTime;
            position += transform.right * panSpeed * Time.deltaTime;
        }
        
        // float scroll = mouse.scroll;
        // position.y += scroll * scrollSpeed * 100f * Time.deltaTime;
        position.x = Mathf.Clamp(position.x, -panLimit.x, panLimit.x);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        position.z = Mathf.Clamp(position.z, -panLimit.y, panLimit.y);

//        Debug.Log(mouse.scroll);


        transform.position = position;
    }
}
