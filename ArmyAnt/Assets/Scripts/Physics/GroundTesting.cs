using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTesting : MonoBehaviour {

	Ray ray;

	[HideInInspector]
	public bool isGround = false;

	public float rayLength = 0.3f;
	void FixedUpdate()
	{
		if( Physics.Raycast(transform.position,Vector3.down,rayLength))
		{
			isGround = true;
		}else
		{
			isGround = false;
		}
	}

}
