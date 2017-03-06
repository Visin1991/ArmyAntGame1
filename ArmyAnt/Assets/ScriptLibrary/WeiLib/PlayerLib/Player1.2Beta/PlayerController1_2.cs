using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test1_2
{
    public partial class PlayerController1_2 : MonoBehaviour
    {
        public static PlayerController1_2 instance;
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(instance);   
            }
            instance = this;
        }
        //=======================================
        //----------Generic----------------------
        //=======================================
        #region Generic

        public float walkSpeed = 5;
        public float runSpeed = 10;
        public bool isRunning = false;
        public float ccJumpHeight = 1.0f;
        public float rgJumpForce = 200.0f;
        public bool jumpKeyDown = false;

        Vector3 velocityDir;                    //By default velocity Direction is transform.forward.
        float postProcessedMoveSpeed = 0;       //speed is only a Scalar
        Vector3 velocity3D;                     //Velocity contains speed and Direction.
        Vector3 velocity3DAdditional = Vector3.zero; //this velocity is used for pasitive motion. like process getting hit movement...whatever

        public float turnSmoothTime = 0.2f;     //the bigger the slower
        float turnSmoothVelocity;

        public float speedSmoothTime = 0.1f;
        float speedSmoothVelocity;
        float currentSpeed;

        public float defaultYRotation = 0;

        [HideInInspector]
        public Vector2 moveInput;
        float moveAnimationSpeed;

        bool blockMovementInput = false;

        //      no longer impliment input information process inside PlayerController.
        //For better organization, We check all Input information in Player Script.
        //void InputUpdate(){}

        //Facing Direction
        void FacingInputDir()
        {
            if (moveInput != Vector2.zero)
                transform.eulerAngles = Vector3.up * Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            else
                transform.eulerAngles = Vector3.up * defaultYRotation;
        }
        void FacingInputDir_Stay()
        {
            if (moveInput != Vector2.zero)
            {
                transform.eulerAngles = Vector3.up * Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            }
        }
        void FacingInputDir_Stay_AlignCameraDir()
        {
            if (moveInput != Vector2.zero)
            {
                transform.eulerAngles = Vector3.up * (Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y);
            }
        }
        void FacingInputDir_Stay_AlignCamaraDir_Smooth()
        {
            if (moveInput != Vector2.zero && !blockMovementInput)
            {
                float targetRotation = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
            }
        }
        //Velocity Direction
        void UpdateVelocityDir(Vector3 v)
        {
            velocityDir = v;
        }
        void UpdateVelocityDir_WithFacingDir()
        {
            velocityDir = transform.forward;
        }
        void UpdateVelocityDir_WithInputDir()
        {
            velocityDir = new Vector3(moveInput.x, 0, moveInput.y);
        }
        //Post process Speed and Velocity
        void PostProcessed_MoveSpeed()
        {
            postProcessedMoveSpeed = isRunning ? moveInput.magnitude * runSpeed : moveInput.magnitude * walkSpeed;
        }
        void PostProcessed_MoveSpeed_Velocity()
        {
            postProcessedMoveSpeed = isRunning ? moveInput.magnitude * runSpeed : moveInput.magnitude * walkSpeed;
            velocity3D = postProcessedMoveSpeed * velocityDir;
        }
        void PostProcessed_MoveSpeed_Velocity_Smooth()
        {
            float targetSpeed = isRunning ? moveInput.magnitude * walkSpeed : moveInput.magnitude * runSpeed;
            postProcessedMoveSpeed = Mathf.SmoothDamp(postProcessedMoveSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));
            velocity3D = postProcessedMoveSpeed * velocityDir;
        }

        //Motion
        //Call this function in Player Class every frame in Update
        public void GenericMotion()
        {
            FacingInputDir_Stay_AlignCamaraDir_Smooth();
            UpdateVelocityDir_WithFacingDir();
            PostProcessed_MoveSpeed_Velocity();
        }
        public void GenericMotion_FixedFacing()
        {
            UpdateVelocityDir_WithInputDir();
            PostProcessed_MoveSpeed_Velocity();
        }

        #endregion
        //=======================================
        //-----------Camera-----------------------
        //=======================================
        #region Camera
        public Transform cameraT;
        public void InitCamera()
        {
            //cameraT = Camera.main.transform;
        }
        #endregion
        //=======================================
        //-----------Animation-------------------
        //=======================================
        #region Animation
        Animator animator;
        [HideInInspector]
        public bool onJump;
        public void InitAnimation()
        {
            animator = GetComponentInChildren<Animator>();
        }

        void MoveAnimation()
        {
            moveAnimationSpeed = ((isRunning) ? 1 : .5f) * moveInput.magnitude;
            animator.SetFloat("speedPercent", moveAnimationSpeed);
        }

        //Call this function in Player Class every frame in Update
        public void UpdateAnimation()
        {
            UpdateAnimationSmooth();
        }

        partial void UpdateAnimationSmooth();

        #endregion
        //=======================================
        //------------RigidBodyController--------
        //=======================================
        #region RigidBodyController
        Rigidbody rg;

        public float jumpPreAnimationTime = 0.5f;
        public float JumpCD = 1;
        public void InitRigidBody()
        {
            rg = GetComponent<Rigidbody>();
        }

        //Call this function inside FixedUpdate
        public void UpdateRigidBodyController()
        {
            //Main input move
            if (!blockMovementInput) { rg.MovePosition(rg.position + velocity3D * Time.fixedDeltaTime); }
            //Additional movement.
            if (velocity3DAdditional != Vector3.zero) { rg.MovePosition(rg.position + velocity3DAdditional * Time.fixedDeltaTime); }
        }
        #endregion
        //=======================================
        //------------CharacterController--------
        //=======================================
        #region CharacterController

        public float ccGravity = -12;
        float velocityY;

        [Range(0, 1)]
        public float airControlPercent;
        CharacterController controller;

        public void InitCharacterController()
        {
            //Debug.Log("InitCharacterController");
            controller = GetComponent<CharacterController>();
        }

        public void UpdateCCGrabity()
        {
            velocityY += Time.deltaTime * ccGravity;
            velocity3D += Vector3.up * velocityY;
        }

        void FaceDirectionImmediately_AndStay()
        {
            FacingInputDir();

            PostProcessed_MoveSpeed_Velocity();
            transform.Translate(velocity3D * Time.deltaTime, Space.World);
        }

        void MoveWithAnimation()
        {
            FacingInputDir();
            PostProcessed_MoveSpeed_Velocity();
            MoveAnimation();
            transform.Translate(velocity3D * Time.deltaTime, Space.World);
        }

        void MoveWithAnimationSmooth()
        {
            FacingInputDir();
            PostProcessed_MoveSpeed_Velocity_Smooth();
            UpdateAnimationSmooth();
            transform.Translate(velocity3D * Time.deltaTime, Space.World);
        }

        public void SimpleMove()
        {
            FacingInputDir_Stay();
            UpdateVelocityDir_WithFacingDir();
            PostProcessed_MoveSpeed_Velocity();
            //UpdateCCGrabity();
            //CCMove();
            //CCJump();
        }

        void UpdateCharacterController_Full()
        {
            FacingInputDir();
            UpdateVelocityDir_WithFacingDir();
            PostProcessed_MoveSpeed_Velocity_Smooth();
            UpdateAnimationSmooth();
            UpdateCCGrabity();
            CCMove();
            CCJump();
        }

        void CCMove()
        {
            //Debug.Log(velocity3D );
            controller.Move(velocity3D * Time.deltaTime);
            if (controller.isGrounded)
            {
                velocityY = 0.0f;
            }
        }

        void JumpWithAirControl()
        {
            // input
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            Vector2 inputDir = input.normalized;
            bool running = Input.GetKey(KeyCode.LeftShift);

            Move(inputDir, running);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CCJump();
            }
            // animator
            float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
            animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
        }

        void Move(Vector2 inputDir, bool running)
        {
            if (inputDir != Vector2.zero)
            {
                float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + cameraT.eulerAngles.y;
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
            }

            float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

            velocityY += Time.deltaTime * ccGravity;
            Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

            controller.Move(velocity * Time.deltaTime);
            currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;

            if (controller.isGrounded)
            {
                velocityY = 0;
            }
        }

        void CCJump()
        {
            //So is we are grounded, we can add velocity up
            if (jumpKeyDown)
            {
                if (controller.isGrounded)
                {
                    float jumpVelocity = Mathf.Sqrt(-2 * ccGravity * ccJumpHeight);
                    velocityY = jumpVelocity;
                }
            }
        }
        #endregion
        //---------------------------------------
        float GetModifiedSmoothTime(float smoothTime)
        {
            if (!onJump)
            {
                return smoothTime;
            }

            if (airControlPercent == 0)
            {
                return float.MaxValue;
            }
            return smoothTime / airControlPercent;
        }

        void Start()
        {
            Start2();
            Start3();
        }

        private void FixedUpdate()
        {
            FixedUpdate3();
        }

        private void Update()
        {
            Update3();
        }
    }
}