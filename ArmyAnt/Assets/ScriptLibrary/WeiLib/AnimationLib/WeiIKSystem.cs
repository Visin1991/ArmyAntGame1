using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeiIKSystem : MonoBehaviour {

    Animator anim;

    public Transform leftIKTarget;
    public Transform rightIKTarget;

    Vector3 lFpos;
    Vector3 rFpos;

    Quaternion lFrot;
    Quaternion rFrot;

    float lFWeight;
    float rFWeight;

    public Transform leftFoot;
    public Transform rightFoot;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        //leftFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        //rightFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit leftHit;
        RaycastHit rightHit;

        Vector3 lpos = leftFoot.TransformPoint(Vector3.zero);
        Vector3 rpos = rightFoot.TransformPoint(Vector3.zero);

        if (Physics.Raycast(lpos, -Vector3.up, out leftHit, 5))
        {
            lFpos = leftHit.point;
            Debug.DrawLine(leftFoot.position, lFpos, Color.red);
            lFrot = Quaternion.FromToRotation(transform.up, leftHit.normal) * transform.rotation;
        }

        if (Physics.Raycast(rpos, -Vector3.up, out rightHit, 5))
        {
            rFpos = rightHit.point;
            Debug.DrawLine(rightFoot.position, rFpos, Color.blue);
            rFrot = Quaternion.FromToRotation(transform.up, rightHit.normal) * transform.rotation;
        }

    }

    void OnAnimatorIK()
    {


        lFWeight = 1;//anim.GetFloat("leftFoot");
        rFWeight = 1;//anim.GetFloat("rightFoot");

        anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, lFWeight);
        anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, rFWeight);

        anim.SetIKPosition(AvatarIKGoal.LeftFoot, leftIKTarget.position);
        anim.SetIKPosition(AvatarIKGoal.RightFoot, rightIKTarget.position);

        anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, lFWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, rFWeight);

        anim.SetIKRotation(AvatarIKGoal.LeftFoot, lFrot);
        anim.SetIKRotation(AvatarIKGoal.RightFoot, rFrot);
    }
}
