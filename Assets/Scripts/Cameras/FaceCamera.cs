using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;
    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    // we want to make sure unit has moved before camera does, so we should use late update
    private void LateUpdate()
    {
        // our current position plus our rotation (0,0,1) which will get our forward component
        // camera can tilt so world up will be mainCameraTransform.rotation * Vector3.up, this will have UI face camera at all times
        transform.LookAt(transform.position + mainCameraTransform.rotation * Vector3.forward, mainCameraTransform.rotation * Vector3.up);
    }
}
