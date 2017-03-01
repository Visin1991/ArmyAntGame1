using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Test1_2
{
    public partial class PlayerController1_2
    {

        [HideInInspector]
        public bool onAir;

        float privewY;
        float currentY;
        public bool swipingKeyDown;
        public bool punchKeyDown;
        public bool die = false;
        public Transform hipTransform;

        public int animationBlockMask;

        public float jumpForceDelayTime = 0.4f;
        public float throwBuildingDelayTime = 1.05f;

        Player1_2 p;

        public TrailRenderer trailRenderLeft;
        public TrailRenderer trailRenderRight;

        void Start2() { 
            trailRenderLeft.enabled = false;
            trailRenderRight.enabled = false;
            p = GetComponent<Player1_2>();
        }

        partial void UpdateAnimationSmooth()
        {
            //Set Animation information
            SetbasicMoveAnimation();
            SetSpecialAnimation();
            UpdateAnimationSmooth_IK();
        }

        #region SetAnimationInfo

        //  We do not set any flag when we assign a value to animator
        //Instead we use animation state machine Enter and Exit to call back, and 
        //Set all flag values.
        void SetbasicMoveAnimation()
        {
            float animationSpeedPercent = ((isRunning) ? 1 : .5f) * moveInput.magnitude;
            if (!blockMovementInput)
                animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
        }

        void SetSpecialAnimation()
        {
            if (jumpKeyDown && !blockJumpAnimation)
            {
                animator.SetBool("Jump", jumpKeyDown);
            }

            if (swipingKeyDown && !blockAttackAnimation)
            {
                //Debug.Log(blockAttackAnimation);
                animator.SetBool("Attack", swipingKeyDown);
                animator.SetInteger("AttackTypes", 1);
            }

            if (punchKeyDown && !blockAttackAnimation)
            {
                animator.SetBool("Attack", punchKeyDown);
                animator.SetInteger("AttackTypes", 0);
            }
            if (die)
            {
                animator.SetBool("die", die);
            }


        }

        #endregion

        /// <summary>
        ///     When we enter a animation state or exit a animation state we callback those 
        /// function to set or reset some specific values.
        /// </summary>
        #region AnimationCallBackFunction

        public bool throwBuilding = false;

        public enum NonParamsCallBackType
        {
            CallBackFunc1,
            CallBackFunc2,
            PunchGorge,
            JumpAnimationEnter,
            SetExitHipPosAsPos,
            SwipingEnter,
            PunchEnter,
            Hurricane_kick_Enter,
            DoDamageToTrue,
            DoDamageToFalse,
            RootMotionTrue,
            RootMotionFalse,
            TrailRenderOnLeft,
            TrainRenderOnRight,
            TrailRenderOffLeft,
            TrailRenderOffRight,
            GetPickUpBuilding
        }

        static bool initialCallbackMethod = false;
        static List<MethodInfo> methodInfos = new List<MethodInfo>();
        public static List<MethodInfo> CreateCallBackInstance()
        {
            if (!initialCallbackMethod)
            {
                List<NonParamsCallBackType> callBackFunctypes = GetListFromEnum<NonParamsCallBackType>();
                foreach (NonParamsCallBackType type in callBackFunctypes)
                {
                    methodInfos.Add(typeof(PlayerController).GetMethod(type.ToString(), BindingFlags.Public | BindingFlags.Instance));
                }
                initialCallbackMethod = true;
                return methodInfos;
            }
            else
            {
                return methodInfos;
            }
        }
        //CallBack functions
        public void CallBackFunc1()
        {
            Debug.Log("Call Func 1");
        }
        public void CallBackFunc2()
        {
            Debug.Log("Call Func 2");
        }
        public void PunchGorge()
        {
            Debug.Log("PunchGorge get called");
        }
        public void JumpAnimationEnter()
        {
            Invoke("ApplayJumpForce", jumpForceDelayTime);
        }
        public void SwipingEnter()
        {
            Invoke("ThrowBuilding", throwBuildingDelayTime);
        }
        public void PunchEnter()
        {
            p.checkDamage = true;
            WeiAudioManager.instance.PlaySound2D("playerSound");
        }
        public void Hurricane_kick_Enter()
        {
            WeiAudioManager.instance.PlaySound2D("MotionEffect", 1);
            p.checkDamage = true;

        }
        public void SetExitHipPosAsPos()
        {
            p.checkDamage = false;
            transform.position = new Vector3(hipTransform.position.x, transform.position.y, hipTransform.position.z);
        }
        public void DoDamageToTrue()
        {
            p.checkDamage = true;
        }
        public void DoDamageToFalse()
        {
            p.checkDamage = false;
        }
        public void RootMotionTrue()
        {
            animator.applyRootMotion = true;
        }
        public void RootMotionFalse()
        {
            animator.applyRootMotion = false;
        }
        public void TrailRenderOnLeft()
        {
            trailRenderLeft.enabled = true;
        }
        public void TrainRenderOnRight()
        {
            trailRenderRight.enabled = true;
        }
        public void TrailRenderOffLeft()
        {
            trailRenderLeft.enabled = false;
        }
        public void TrailRenderOffRight()
        {
            trailRenderRight.enabled = false;
        }
        //Internal callBack sub Functions
        void ApplayJumpForce()
        {
            WeiAudioManager.instance.PlaySound2D("MotionEffect", 0);
            //rg.AddForce(Vector3.up * rgJumpForce + transform.forward * rgJumpForce / 2);
        }
        void ThrowBuilding()
        {
            WeiAudioManager.instance.PlaySound2D("Impact");
            throwBuilding = true;
        }

        //Specific call Back function
        public enum CallBackSpecialType
        {
            MovePosition,
            RotateYAxis,
            AddForce,
        }

        public void RotateYAxis(params object[] list)
        {
            if (list[0] is float) { transform.Rotate(Vector3.up * (float)list[0]); }
            else { Debug.LogError("Pass inappropriate params"); return; }    
        }
        public void MovePosition(params object[] list)
        {
            if (list[0] is Vector3)
            {//transform.position += new Vector3(((Vector3)list[0]).x, ((Vector3)list[0]).y, ((Vector3)list[0]).z);
                transform.position += (Vector3)list[0];
            }else{
                Debug.LogError("MovePosition have to pass a Vector3  as params");
            }
        }
        public void AddForce(params object[] list)
        {
            if (list[0] is float && list[1] is Vector3){
                rg.AddForce((float)list[0] * ((Vector3)list[1]).normalized);
            }else{
                Debug.LogError("AddForce have to pass a float and a vector3 as params");
            }
        }

        //IK Call back
        public void PickBuildingIK(params object[] list)
        {

        }
        #endregion

        #region AnimationBlockMask

        bool blockAttackAnimation = false;
        bool blockJumpAnimation = false;
        public void ResetAnimationBlockMask(int mask)
        {
            //Debug.Log(mask);
            blockMovementInput = (mask & (int)WeiASMB1_2.MaskTypes.blockMovement) == 0 ? false : true;
            
            blockAttackAnimation = (mask & (int)WeiASMB1_2.MaskTypes.blockAttack) == 0 ? false : true;
            
            blockJumpAnimation = (mask & (int)WeiASMB1_2.MaskTypes.blockJump) == 0 ? false : true;
        }
        #endregion

        #region StaticHelperFuncs
        static List<T> GetListFromEnum<T>()
        {
            List<T> enumList = new List<T>();
            System.Array enums = System.Enum.GetValues(typeof(T));
            foreach (T e in enums)
            {
                enumList.Add(e);
            }
            return enumList;
        }
        #endregion
    }
}
