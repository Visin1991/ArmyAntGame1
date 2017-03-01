using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeiThridPersonCamera : MonoBehaviour {

    public Vector2 pitchMinMax = new Vector2(0, 85);
    public float rotationSmoothTime = 0.5f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    public Transform target;
    public Vector2 rangeToTarget = new Vector2(2, 20);
    float dstToTarget = 10;
    public float cameraMoveSensitivity = 10;
    float yaw;  //Rotation around Y Axis
    float pitch = 75;//Rotation around X Axis
    float zoomInOut;
    public bool Xbox;

	// Update is called once per frame
	void LateUpdate () {

        if (Xbox)
        {
            yaw += Input.GetAxis("RXAxis") * cameraMoveSensitivity;
            pitch -= Input.GetAxis("RYAxis") * cameraMoveSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            dstToTarget += Input.GetAxis("LTrigger");
            dstToTarget = Mathf.Clamp(dstToTarget, rangeToTarget.x, rangeToTarget.y);
        }
        else {
            yaw += Input.GetAxis("Mouse X") * cameraMoveSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * cameraMoveSensitivity;
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
            dstToTarget += Input.GetAxis("Mouse ScrollWheel");
            dstToTarget = Mathf.Clamp(dstToTarget, rangeToTarget.x, rangeToTarget.y);
        }

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;
        transform.position = target.position - transform.forward * dstToTarget;
    }

    
}
