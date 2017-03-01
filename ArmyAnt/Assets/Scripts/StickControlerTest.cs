using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class StickControlerTest : MonoBehaviour {

	public float moveForce = 5;
	public float boostMultiplier = 2;
	Rigidbody rg;
	// Use this for initialization
	void Start () {
		rg = this.GetComponent<Rigidbody>();
	}
	
	void FixedUpdate()
	{
		Vector2 moveVec = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"),CrossPlatformInputManager.GetAxis("Vertical")) * moveForce;
		Vector3 moveDir = new Vector3(moveVec.x,moveVec.y,0.0f);
		bool isBoosting = CrossPlatformInputManager.GetButton("Boost");
		//? is Condition operator
		//Debug.Log(isBoosting ? boostMultiplier : 1);
		rg.AddForce(moveDir * (isBoosting ? boostMultiplier : 1));

	}
}
