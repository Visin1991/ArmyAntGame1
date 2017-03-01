using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeiRootIK{

    /// <summary>
    /// the definition of a joint
    /// </summary>
    [System.Serializable]
    public class Joints
    {
        public Transform transform;
        [Range(0f, 1f)]
        public float weight;
        [HideInInspector]
        public Vector3 solvePos;

        //options for QuaternionLimits
        public enum QuaternionLimits { twistX_SwingYZ, twistY_SwingXZ, twistZ_swingXY }
        [HideInInspector]
        public QuaternionLimits limitMode; //currently in beta

        public Joints(Transform _joint, float _weight)
        {
            _weight = Mathf.Clamp(_weight, 0f, 1f);
            weight = _weight;
            transform = _joint;
        }
    }

    /// <summary>
    /// the definition of an IK chain
    /// </summary>
    [System.Serializable]
    public class Chain
    {
        public Transform target;

        private Vector3 IKHandle;
        private Quaternion IKRotation;

        [Range(0f, 1f)]
        public float weight = 1f;
        [Range(0f, 1f)]
        public float weightRotation = 0f;

        public int iterations;
        public List<WeiRootIK.Joints> joints = new List<WeiRootIK.Joints>();

        #region Helping Methods
        /// <summary>
        /// Set the IK target Position which the chain will solve towards
        /// </summary>
        /// <param name="_target"></param>
        public void SetIKPosition(Vector3 _target)
        {
            IKHandle = target ? target.position : _target;
        }

        /// <summary>
        /// Find the current IK solver Position
        /// </summary>
        /// <returns>the current IK position</returns>
        public Vector3 GetIKPosition()
        {
            IKHandle = target ? target.position : IKHandle;
            return IKHandle;
        }

        /// <summary>
        /// Set the IK target rotation which will effect the rotation of the end effector
        /// </summary>
        public void SetIKRotation(Quaternion _rotation)
        {
            Quaternion _local = GetEndEffector() == null ? IKRotation : GetEndEffector().rotation;
            IKRotation = target ? Quaternion.Lerp(_local, target.rotation, weightRotation) : Quaternion.Lerp(_local, _rotation, weightRotation);
        }

        /// <summary>
        /// Get the current active IK rotation
        /// </summary>
        /// <returns>the rotation of the active rotation influencer</returns>
        public Quaternion GetIKRotation()
        {
            IKRotation = target ? target.rotation : IKRotation;
            Quaternion _local = Quaternion.Lerp(GetEndEffector().rotation, IKRotation, weightRotation);
            return _local;
        }

        /// <summary>
        /// Get the transform data of the end effector
        /// </summary>
        /// <returns>the endEffector transformation</returns>
        public Transform GetEndEffector()
        {
            return joints[joints.Count - 1].transform ? joints[joints.Count - 1].transform : null;
        }

        /// <summary>
        /// Set the IK position weight
        /// </summary>
        /// <param name="_weight"></param>
        public void SetIKPositionWeight(float _weight)
        {
            weight = _weight;
        }

        /// <summary>
        /// Set the IK Rotation weight
        /// </summary>
        /// <param name="_weight"></param>
        public void SetIKRotationWeight(float _weight)
        {
            weightRotation = _weight;
        }

        /// <summary>
        /// Solve the IK chain using heuristic iterative search methods (CCD and FABRIK);
        /// </summary>
        /// <param name="_solver"></param>
        public void SolveChain()
        {
           
        }

        /// <summary>
        /// Solve the IK chain analytically (joint count must equal to 3)
        /// </summary>
        /// <param name="_direction"></param>
        /// <param name="_axis"></param>
        public void SolveChainAnalytically(Vector3 _direction, Vector3 _axis)
        {
            if (this.joints.Count != 3) return;
        }
        #endregion
    }

    /// <summary>
    /// The defenition of a Kinematic bone.
    /// Rotate the bone to look at the target;
    /// </summary>
    [System.Serializable]
    public class KinematicBone
    {
        [Range(0f, 1f)]
        public float weight;
        public Transform bone;
        public Transform target;
        public Vector3 axis;
    }
}
