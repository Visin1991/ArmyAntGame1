using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public partial class WeiIKSnap : MonoBehaviour {

    public bool useIK;
   
    public bool leftHandIK;
    RaycastHit LHit;
    public Transform leftHandTF;
    public Transform leftShoderTF;
    public Vector3 LeftHandHolder;
    public Quaternion leftHandRot;  
    public float leftshoderPosionAdjust = 0.2f;
    public float leftArmLength;

    public bool rightHandIK;
    RaycastHit RHit;
    public Transform rightHandTF;
    public Transform rightShoderTF;
    public Vector3 RightHandHolder;
    public Quaternion rightHandRot;
    public float rightShoderPositionAdjust = 0.2f;
    public float rightArmLength;

    private Animator anim;

    public float handReachableHeight = 2.5f; //maxHeight the player can reach, from the foot postion
    public float rangeToGetCOI = 3.0f; //get the ClimbalbeIK object in range of
    float sqrRangeToGetCOI;

    float nexTimeToCheckCollider=0.0f;
    float nextTimeToCheckTouchableIKObj = 0.0f;
    public bool tryToFindClimbHolder = false;
    List<IKObject> climbableIKObjs = new List<IKObject>();
    List<IKObject> touchableIKObjs = new List<IKObject>();
	// Use this for initialization
	void Start () {
        Debug.Log((Quaternion.Euler(1,1,1)* new Vector3(20,5,1)));
       anim =  GetComponent<Animator>();
       sqrRangeToGetCOI = rangeToGetCOI * rangeToGetCOI;
        leftArmLength = (leftHandTF.position - leftShoderTF.position).magnitude;
        rightArmLength = (rightHandTF.position - rightShoderTF.position).magnitude;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            GetNextClimbHolder();
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (tryToFindClimbHolder){
            GetAllCOIColliderInRange();
            if (climbableIKObjs.Count > 0){
                CheckClimbableHolder();
                GetTouchableHolder();
            }    
        }
        
    }

    void OnAnimatorIK()
    {
        if (leftHandIK)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandHolder);
        }
        if (rightHandIK)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            anim.SetIKPosition(AvatarIKGoal.RightHand, RightHandHolder);
        }
    }

    //Update this function every 2 second, to improve the performance.
    void GetAllCOIColliderInRange()
    {
        if (Time.time < nexTimeToCheckCollider) { return; }
        nexTimeToCheckCollider = Time.time + 1.0f;

        Collider[] allColliders = Physics.OverlapSphere(transform.position, rangeToGetCOI);
        foreach (Collider c in allColliders)
        {
            IKObject coi = c.gameObject.GetComponent<IKObject>();
            if(coi)
            {
                if (!climbableIKObjs.Contains(coi)) {
                    climbableIKObjs.Add(coi);
                }      
            }
        }
    }

    //This should be check every 0.1 seconds, to improve the peformance.
    /// <summary>
    ///     Check if the ClimbableIKObject is close enough to prepare to Climbing. If not we than will remove
    /// it from climbableIKObjs.
    /// </summary>
    void CheckClimbableHolder()
    {
        if(Time.time < nextTimeToCheckTouchableIKObj) { return; }
        nextTimeToCheckTouchableIKObj = Time.time + 0.1f;       
        for (int i = climbableIKObjs.Count - 1;i >=0;i--)
        {//iterate backwards by index, removing matching items
            float sqrDstFromNearestEdge = climbableIKObjs[i].bounds.SqrDistance(transform.position);
            //Debug.Log(sqrDstFromNearestEdge);
            if (sqrRangeToGetCOI < sqrDstFromNearestEdge){ //For some reason, the Sqrt     
                climbableIKObjs.RemoveAt(i);
            }
        }
    }

    //This should be update everyFrame. When the player try to Climb.
    /// <summary>
    /// even the climbable object is close enough to our player.
    /// but player still may not be able to reach the top of the climable Object.
    /// So we need to detecte if the player's hand can reach the top of the Climbable Object;
    /// We use shoulder as sart position. handlength as the max distance to check if we can reach
    /// the top ob climbable object.
    /// </summary>
    void GetTouchableHolder()
    {
        touchableIKObjs.Clear();
        foreach (IKObject c in climbableIKObjs){
            if ((c.transform.position.y + c.bounds.size.y/2) > handReachableHeight+transform.position.y) {continue;}
            touchableIKObjs.Add(c);
        }

        foreach (IKObject c in touchableIKObjs)
        {
            Vector3 leftShoderPosition = leftShoderTF.position - transform.right * leftshoderPosionAdjust;
            Vector3 leftShoderClosetEdgePoint = c.GetClosestPointFromTopEdge(leftShoderPosition);
            leftHandIK = ((leftShoderClosetEdgePoint - leftShoderPosition).magnitude <= leftArmLength);
            if (leftHandIK)
            {
                LeftHandHolder = leftShoderClosetEdgePoint;
                
                //FromToRotation returns a Quaternion that would rotate the first vector so that it matches the second vector.
            }
            Debug.DrawLine(leftShoderPosition, leftShoderClosetEdgePoint, Color.black);

            Vector3 rightShoderPosition = rightShoderTF.position + transform.right * rightShoderPositionAdjust;
            Vector3 rightShoderClosetEdgePoint = c.GetClosestPointFromTopEdge(rightShoderPosition);
            rightHandIK = ((rightShoderClosetEdgePoint - rightShoderPosition).magnitude <= rightArmLength);
            if (rightHandIK)
            {
                RightHandHolder = rightShoderClosetEdgePoint;
            }
            Debug.DrawLine(rightShoderPosition, rightShoderClosetEdgePoint,Color.black);
        }
    }

    void GetNextClimbHolder()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeToGetCOI);
    }
}
