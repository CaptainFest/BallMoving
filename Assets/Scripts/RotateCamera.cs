using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour {

    public Transform target;         // target ball
    public Vector3 offset;           // camera offset
    public float sensitivity = 0.3f; // mouse sensitivity
    public float limit = 80;         // base Y rotate limit
    public float zoom = 0.25f;       // zoom sensitivity using mouse scroll
    public float zoomMax = 20;       // max distance to zoom from object
    public float zoomMin = 3;        // min distance to zoom to object
    private float X, Y;              // x, y coordinates

    void Start() // set base camera position and some limits
    {
        limit = Mathf.Abs(limit);
        if (limit > 90) limit = 90;
        offset = new Vector3(offset.x, offset.y, -Mathf.Abs(zoomMax) / 2);
        transform.position = target.position + offset;
        transform.eulerAngles = new Vector3(0, -90, 0);
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)                                 //   
            offset.z += zoom;                                                       //
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)                            // zooming 
            offset.z -= zoom;                                                       //
        offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin)); // zoom limits

        X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;  //
        Y += Input.GetAxis("Mouse Y") * sensitivity;                                // camera rotations
        Y = Mathf.Clamp(Y, -limit, limit);                                          // Y limits 
        transform.localEulerAngles = new Vector3(-Y, X, 0);                         //
        transform.position = transform.localRotation * offset + target.position;    //
    }
}
