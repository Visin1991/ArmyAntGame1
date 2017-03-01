using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {

	PlayerController controller;
	GunController gunController;
	int gunIndex;

	// Use this for initialization
	public override void Start () {
		base.Start();
		controller = GetComponent<PlayerController>();
		controller.InitCamera();
		controller.InitRigidBody();
		gunController = GetComponent<GunController>();
		//controller.InitCharacterController();
	}
	
	// Update is called once per frame
	void Update () {

		controller.GenericMotion();
		//controller.GenericMotion_FixedFacing();

		if(CrossPlatformInputManager.GetButton("Boost"))
		{
			gunController.OnTriggerHold();
		}
		if(CrossPlatformInputManager.GetButtonUp("Boost"))
		{
			gunController.OntriggerRelease();
		}
		if(Input.GetKeyDown(KeyCode.R))
		{
			gunController.Reload();
		}

		if(CrossPlatformInputManager.GetButtonDown("SwitchWepon"))
		{
            Debug.Log("ChangeWepoon");
			gunController.EquipGunIndex(gunIndex%3);
			gunIndex++;
		}

		if(Input.GetKeyDown(KeyCode.K))
		{
			TakeDamage(10.0f);
		}
			
	}

	void FixedUpdate()
	{
		controller.UpdateRigidBodyController();
	}
		
}
