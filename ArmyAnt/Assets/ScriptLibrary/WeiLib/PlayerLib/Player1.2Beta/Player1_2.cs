using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test1_2{
    [RequireComponent(typeof(PlayerController1_2))]
    public class Player1_2 : LivingEntity
    {

        PlayerController1_2 pController;

        public GameObject pickUpHandler;
        public float pickDistance;
        public LayerMask pickUpLayer;
        public float ThrowForce = 2;
        public int playerIndex;

        // Use this for initialization
        public KeyCode pickUpKey = KeyCode.E;
        public KeyCode runningKey = KeyCode.LeftShift;
        public KeyCode jumpKey = KeyCode.Space;
        public KeyCode SwipingKey = KeyCode.F;
        public KeyCode PunchKey = KeyCode.R;
        public string AxisUpDown = "Vertical";
        public string AxisLeftRight = "Horizontal";

        //Input Information
        bool pickKeyDown = false;
        bool isRunning = false;
        bool isJumpKeyDown = false;
        bool isSwipingKeyDown = false;
        bool isPunchKeyDown = false;

        Vector2 moveInput = Vector2.zero;

        public override void Start()
        {
            base.Start();
            pController = GetComponent<PlayerController1_2>();
            pController.InitCamera();
            pController.InitAnimation();
            pController.InitRigidBody();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateInputInfo();
            UpdatePlayerControllerInfo();
            UpdateAdditionalAction();
        }

        private void FixedUpdate()
        {
            pController.UpdateRigidBodyController();
        }

        #region UpdateGroupes
        /// <summary>
        ///     Update all nesseccery Input information inside Player class. and save those information
        /// so PlayerController skillAngent or whatever all can chek Input In side Player class. Then we can
        /// deal with more Input combal. Also can find out the Input conflict very eazily
        /// </summary>
        void UpdateInputInfo()
        {
            pickKeyDown = Input.GetKeyDown(pickUpKey);
            isRunning = Input.GetKey(runningKey);
            isJumpKeyDown = Input.GetKeyDown(jumpKey);
            isSwipingKeyDown = Input.GetKeyDown(SwipingKey);
            isPunchKeyDown = Input.GetKeyDown(PunchKey);
            moveInput.x = Input.GetAxisRaw(AxisLeftRight);
            moveInput.y = Input.GetAxisRaw(AxisUpDown);
        }

        void UpdatePlayerControllerInfo()
        {
            pController.isRunning = isRunning;
            pController.moveInput = moveInput;
            pController.jumpKeyDown = isJumpKeyDown;
            pController.swipingKeyDown = isSwipingKeyDown;
            pController.punchKeyDown = isPunchKeyDown;
            pController.UpdateAnimation();
            pController.GenericMotion();
            //pController.UpdateRigidBodyController(); //we should put Physics part in FixedUpdate
        }

        void UpdateAdditionalAction()
        {
            if (pickKeyDown)
            {
                PickTheBuilding();
            }
            if (pController.throwBuilding)
            {
                if (pickUpHandler.transform.childCount > 0)
                {
                    ThrowBuilding();
                }
                pController.throwBuilding = false;
            }
        }

        public override void Die()
        {
            base.Die();
            pController.die = true;
        }
        #endregion

        #region AdditionActionFunction

        void PickTheBuilding()
        {
            Collider[] cs = Physics.OverlapSphere(transform.position, pickDistance, pickUpLayer);

            if (cs.Length > 0)
            {
                if (cs[0].GetComponent<Rigidbody>()) { Destroy(cs[0].GetComponent<Rigidbody>()); }

                if (!cs[0].transform.GetComponent<BuildingHealth>()) { return; }

                cs[0].transform.parent = pickUpHandler.transform;
                cs[0].transform.localPosition = new Vector3(0.005f, 0.23f, -0.84f);


                cs[0].transform.GetComponent<BuildingHealth>().holderPlayerIndex = playerIndex;

                if (playerIndex == 0)
                {
                    cs[0].transform.GetComponent<BuildingHealth>().otherplayer = 1;
                }
                else if (playerIndex == 1)
                    cs[0].transform.GetComponent<BuildingHealth>().otherplayer = 0;
            }

        }

        void ThrowBuilding()
        {
            GameObject pickUpHolder = pickUpHandler.transform.GetChild(0).gameObject;
            if (pickUpHolder != null)
            {
                pickUpHolder.transform.parent = null;
                Rigidbody rPickup;
                if (!pickUpHolder.GetComponent<Rigidbody>())
                {
                    rPickup = pickUpHolder.transform.gameObject.AddComponent<Rigidbody>();
                    rPickup.useGravity = true;
                }
                else
                {
                    rPickup = pickUpHolder.transform.GetComponent<Rigidbody>();
                }

                pickUpHolder.transform.GetComponent<BuildingHealth>().BeThrowed();
                rPickup.AddForce(transform.forward * ThrowForce);
            }
        }

        #endregion
        public bool checkDamage = false;
        private void OnCollisionEnter(Collision collision)
        {
            if (checkDamage)
            {
                if (collision.transform.GetComponent<LivingEntity>())
                {
                    collision.transform.GetComponent<LivingEntity>().TakeDamage(10);
                }
            }
        }

        private void OnGUI()
        {
            // GUILayout.Label(new GUIContent("Player Health : " + health.ToString()));
        }
    }
}
