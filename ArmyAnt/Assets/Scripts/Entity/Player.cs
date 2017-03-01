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
    public KeyCode shootKey;
    public KeyCode jumpKey;
    public KeyCode switchKey;
    public KeyCode reloadKey;

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

        StandardInput();
    }

	void FixedUpdate()
	{
		controller.UpdateRigidBodyController();
	}

    void StandardInput()
    {
        if (Input.GetKey(shootKey))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetKeyUp(shootKey))
        {
            gunController.OntriggerRelease();
        }
        if (Input.GetKeyDown(switchKey))
        {
            Debug.Log("ChangeWepoon");
            gunController.EquipGunIndex(gunIndex % 3);
            gunIndex++;
        }
        if (Input.GetKeyDown(reloadKey))
        {
            gunController.Reload();
        }
    }

    void CrossPlatFormImput()
    {       
		if(CrossPlatformInputManager.GetButtonDown("Boost"))
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
    }
}
