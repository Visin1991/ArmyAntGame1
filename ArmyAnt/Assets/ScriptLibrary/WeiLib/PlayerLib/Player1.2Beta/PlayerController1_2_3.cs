using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Test1_2
{
    //This partial class is mainly to deal with IK system;
    public partial class PlayerController1_2
    {
        delegate void UpdateDel();
        UpdateDel updateDel;
        IKObject currentIKTarget;
        //Hand IK parts
        //==========================================
        public Transform leftHandIKTargetTF;
        public Transform rightHandIKTargetTK;

        [HideInInspector]
        public float lefthandPositionWeight = 1;
        [HideInInspector]
        public float rightHandPositionWeight = 1;
        [HideInInspector]
        public float leftHandRotationWeight = 1;
        [HideInInspector]
        public float rightHandRotationWeigt = 1;

        public bool leftHandIK = false;
        public bool rightHandIK = false;
        //=============================================
        //Hip IK parts
        //=============================================
        float hipMaxHeight = 0.0f;
        float distToIKObject = 0.0f;
        //=============================================
        //Shouder IK parts
        //Transform rightShouderTransform;
        //Transform leftShoderTRansform;
        NavMeshAgent naveMeshAngent;

        void Start3()
        {
            hipMaxHeight = hipTransform.transform.localPosition.y * transform.localScale.y;
            //rightShouderTransform = animator.GetBoneTransform(HumanBodyBones.RightShoulder);
            //leftShoderTRansform = animator.GetBoneTransform(HumanBodyBones.LeftShoulder);
            naveMeshAngent = GetComponent<NavMeshAgent>();
            naveMeshAngent.speed = walkSpeed;
        }

        void Update3()
        {
            if (updateDel != null)
            {
                updateDel();
            }
        }

        void FixedUpdate3()
        {
            //fieldOfView.DebugDrawFielOfView();
            GetAllIKObjectInRange();
            if (inCheckRangeIKObjs.Count > 0)
            {
                UpdateinRangeIkObjs();
            }
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (leftHandIK)
            {
                leftHandIKTargetTF.position = currentIKTarget.Bounds.ClosestPoint(LeftHandBoneTF.position);  
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lefthandPositionWeight);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandIKTargetTF.position);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, leftHandRotationWeight);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandIKTargetTF.rotation);
            }
            if (rightHandIK)
            {
                rightHandIKTargetTK.position = currentIKTarget.Bounds.ClosestPoint(RightHandBoneTF.position);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, rightHandPositionWeight);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandIKTargetTK.position);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, rightHandRotationWeigt);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandIKTargetTK.rotation);
            }

             
        }

        void UpdateAnimationSmooth_IK() //call this method from PlayerController1_2_2.
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                SetClosetIKObject();
                StartPickIKTarget();    
            }
        }

        void StartPickIKTarget()
        {
            if(currentIKTarget == null) { return; }
            naveMeshAngent.enabled = true;
            naveMeshAngent.SetDestination(currentIKTarget.closetPointToPlayer);

            updateDel -= OnMoveToTarget;
            updateDel += OnMoveToTarget;
        }

        void OnMoveToTarget()
        {
            animator.SetFloat("speedPercent", 0.5f);
            if (naveMeshAngent.remainingDistance <= 0.5f)
            {
                animator.SetBool("Special", true);
                naveMeshAngent.enabled = false;
                updateDel -= OnMoveToTarget;
            }
        }

        public void GetPickUpBuilding()
        {
            
            leftHandIKTargetTF.SetParent(currentIKTarget.transform);
            rightHandIKTargetTK.SetParent(currentIKTarget.transform);
            currentIKTarget.SetAnimatorTargetTF(transform);

            currentIKTarget.followAnimatorTransform -= currentIKTarget.FollowTarget;
            currentIKTarget.followAnimatorTransform += currentIKTarget.FollowTarget;

            leftHandIK = true;
            rightHandIK = true;
        }

        void SetClosetIKObject()
        {
            if (inCheckRangeIKObjs.Count <= 0) { return; }
            float minDist = float.MaxValue;
            IKObject closetIkObj = inCheckRangeIKObjs[0];
            foreach (IKObject i in inCheckRangeIKObjs)
            {
                i.closetPointToPlayer = i.bounds.ClosestPoint(transform.position);
                float dst = (transform.position - i.closetPointToPlayer).magnitude;
                if(dst < minDist) { minDist = dst; closetIkObj = i; i.dstToPlayer = dst; }
            }
            closetIkObj.TintColor(Color.cyan);
            currentIKTarget = closetIkObj;
        }

        public Transform LeftHandBoneTF
        {
            get { return (animator.GetBoneTransform(HumanBodyBones.LeftHand)); }
        }
        public Transform RightHandBoneTF
        {
            get { return (animator.GetBoneTransform(HumanBodyBones.RightHand)); }
        }

        public void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 50, 20), "Stop"))
            {
                animator.speed = 0.0f;
            }
            if (GUI.Button(new Rect(10, 35, 50, 20), "Play"))
            {
                animator.speed = 1.0f;
            }
        }

        #region IKObjects
        float nexTimeToCheckIKCollider = 0.0f;
        List<IKObject> inCheckRangeIKObjs = new List<IKObject>();
        public AiUtility.FieldOfView fieldOfView;

        [Range(0.1f,1.0f)]
        public float updateTime = 0.5f;
        //Update this function every 2 second, to improve the performance.
        void GetAllIKObjectInRange()
        {  
            if (Time.time < nexTimeToCheckIKCollider) { return; }
            nexTimeToCheckIKCollider = Time.time + updateTime;
            IKObject.SetTintColor(Color.red);
            fieldOfView.GetAllColliderInsideFieldOfView<IKObject>(ref inCheckRangeIKObjs, TintIKObjectColor,true);//this call back function will only be called when a new object add in to the List.
        }
        void TintIKObjectColor()
        {
            inCheckRangeIKObjs[inCheckRangeIKObjs.Count-1].TintColor();
        }
        /// <summary>
        /// This should be check every 0.1 seconds, to improve the peformance.
        ///     Check if the IKObject in inCheckRangeIKObjs update out off range. If it is, we than will remove
        /// it from inCheckRangeIKObjs.
        /// </summary>
        float nextTimeToUpdateIKObjs = 0.0f;
        void UpdateinRangeIkObjs()
        {
            
            if (Time.time < nextTimeToUpdateIKObjs) { return; } 
            nextTimeToUpdateIKObjs = Time.time + 0.1f;
            //Debug.Log(inCheckRangeIKObjs.Count);
            for (int i = inCheckRangeIKObjs.Count - 1; i >= 0; i--)
            {//iterate backwards by index, removing matching items
                if (!fieldOfView.IfObjectInFieldOfView(inCheckRangeIKObjs[i],true))
                {
                    inCheckRangeIKObjs[i].TintColor(Color.white);
                    inCheckRangeIKObjs.RemoveAt(i);
                }
            }
        }
        #endregion
        public Vector3 originaPos;
        public Vector3 forwardEndPos;
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, fieldOfView.viewDistance);

            Gizmos.color = Color.grey;
            Gizmos.DrawSphere(originaPos, 0.1f);

            Gizmos.color = Color.black;
            Gizmos.DrawSphere(forwardEndPos, 0.1f);

            Gizmos.DrawLine(originaPos, forwardEndPos);
        }
    }
}