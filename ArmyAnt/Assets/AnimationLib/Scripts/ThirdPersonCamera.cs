using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

	public bool lockCursor;
	public float mouseSensitivity = 10;
	public Transform target;
	public float dstFromTarget = 2;
	public Vector2 pitchMinMax = new Vector2 (-40, 85);

	public float rotationSmoothTime = .12f;
	Vector3 rotationSmoothVelocity;
	Vector3 currentRotation;

	float yaw = 0;
	float pitch = 75;

	void Start() {

		if (lockCursor) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

    void Update()
    {
        TouchLib.ZoomInOut();
    }

    void LateUpdate () {
        //yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        //pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        //yaw += TouchLib.GetSwipeHorizontal() * mouseSensitivity;
        //pitch -= TouchLib.GetSwipeVertical() * mouseSensitivity;
        Vector2 yawPitch = TouchLib.GetSwipe2D() * mouseSensitivity;
        yaw += yawPitch.x;
        pitch -= yawPitch.y;

		pitch = Mathf.Clamp (pitch, pitchMinMax.x, pitchMinMax.y);

		currentRotation = Vector3.SmoothDamp (currentRotation, new Vector3 (pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
		transform.eulerAngles = currentRotation;

		transform.position = target.position - transform.forward * dstFromTarget;

	}

}
