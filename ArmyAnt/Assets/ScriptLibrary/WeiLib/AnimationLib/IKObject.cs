using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKObject : MonoBehaviour, Ibounds {

    Collider c;
    [HideInInspector]
    Bounds bounds;
    [HideInInspector]
    public float dstToPlayer;
    [HideInInspector]
    public Vector3 closetPointToPlayer;
    [HideInInspector]
    public float topY;

    public float botY;

    public Material ikObjectMat;
    Material mat;

    [HideInInspector]
    static Color color;

    //represent the 4 top vertices
    public Vector3[] vertices = new Vector3[4];

    Transform targetAnimatorTransform;
    Vector3 dst;
    public delegate void UpdateDel();
    public UpdateDel updateDel;

    Vector3 privewPos = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

    void Start() {
        mat = Instantiate(ikObjectMat);
        GetComponent<MeshRenderer>().material = mat;
        c = GetComponent<BoxCollider>();
        bounds = c.bounds;
        CreateVertices();  

    }

    private void Update()
    {
        bounds.center = transform.position;

        if (updateDel!= null)
        {
           updateDel();
        }
    }

    //We will add this function to updateDel, after our player pick up this Object
    public void FollowTarget()
    {
        //RestrictIKAngle();
        if (Input.GetKey(KeyCode.V)) { RotateDstAroundX(); }
        transform.rotation = targetAnimatorTransform.rotation;
        transform.position = targetAnimatorTransform.position + dst;
    }

    void RestrictIKAngle()
    {
        Vector3 dst2D = new Vector3(dst.x, transform.position.y, dst.z);        //The parallel Vector to player position.
        float angle = Vector3.Angle(targetAnimatorTransform.forward, dst2D);    
        if (angle > 60)
        {
            Vector3 cross = Vector3.Cross(targetAnimatorTransform.forward, dst2D); //check the sign of the angle(negative or positive)
            Vector3 newDst;
            if (cross.y < 0)
            {
                newDst = Quaternion.AngleAxis(-60, targetAnimatorTransform.up) * targetAnimatorTransform.forward; //get the new position, which rotate the player's forward direction
            }
            else
            {
                newDst = Quaternion.AngleAxis(60, targetAnimatorTransform.up) * targetAnimatorTransform.forward;
            }
            dst = new Vector3(newDst.x, dst.y, newDst.z);
        }
    }

    //This function will be called when the player pick the IKObject
    public void SetAnimatorTargetTF(Transform t)
    {
        targetAnimatorTransform = t;
        Vector3 newPos = targetAnimatorTransform.position + targetAnimatorTransform.up * 2.8f;
        transform.position = newPos;
        transform.rotation = targetAnimatorTransform.rotation;
        dst = newPos - targetAnimatorTransform.position;
    }

    public void CreateVertices()
    {
        if (!IfObjectMoved()) return; //if we didn't change the position wo donot need to update our vertices Pos;
        vertices[0] = bounds.center + new Vector3(bounds.size.x, bounds.size.y, bounds.size.z) / 2;
        vertices[1] = bounds.center + new Vector3(-bounds.size.x, bounds.size.y, bounds.size.z) / 2;
        vertices[2] = bounds.center + new Vector3(-bounds.size.x, bounds.size.y, -bounds.size.z) / 2;
        vertices[3] = bounds.center + new Vector3(bounds.size.x, bounds.size.y, -bounds.size.z) / 2;
        topY = transform.position.y + bounds.size.y / 2;
        botY = transform.position.y - bounds.size.y / 2;
    }

    public Bounds Bounds { get {return bounds; } }
      
    public Vector3[] Vertices { get { CreateVertices(); return vertices; } }

    public Transform Transform { get { return transform; } }

    public int GetClosestVertex(Vector3 pos)
    {
        int index = 0;
        float minDst = float.MaxValue;

        for(int i = 0;i <vertices.Length;i++)
        {
            float dst = (vertices[i] - pos).magnitude;
            if (dst< minDst) {
                index = i;
                minDst = dst;
            }
        }
        return index;
    }

    bool IfObjectMoved()
    {
        Vector3 newPos = transform.position;
        if (newPos == privewPos) return false;
        else { privewPos = newPos;return true; }
    }

    //Used for Debug.
    public void DrawReachableToEdge(Vector3 pos)
    {
        int i = GetClosestVertex(pos);
        Debug.DrawLine(vertices[i],vertices[(i + 1) % 4], Color.green);
        if (i == 0)
            Debug.DrawLine(vertices[i], vertices[3], Color.green);
        else
            Debug.DrawLine(vertices[i], vertices[i - 1], Color.green);

        Debug.DrawLine(pos, vertices[i], Color.green);
        
        Vector3 projectionPoint = bounds.ClosestPoint(pos);
        Debug.DrawLine(pos,new Vector3(projectionPoint.x, topY,projectionPoint.z), Color.red);
    }

    public Vector3 GetClosestPointFromTopEdge(Vector3 pos)
    {
        Vector3 projectionPoint = bounds.ClosestPoint(pos);
        return new Vector3(projectionPoint.x, topY, projectionPoint.z);
    }

    public void TintColor()
    {
        mat.color = color;
    }

    public void TintColor(Color c)
    {
        mat.color = c;
    }

    public static void SetTintColor(Color c)
    {
        color = c;
    }

    #region IKAnimationFunc

    [ContextMenu("Rotate Dst")]
    void RotateDstAroundX()
    {
        dst = WeiVector3.RotateVectorAround(dst, targetAnimatorTransform.right, 2);
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(vertices[0], 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(vertices[1], 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(vertices[2], 0.1f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(vertices[3], 0.1f);

    }
}
