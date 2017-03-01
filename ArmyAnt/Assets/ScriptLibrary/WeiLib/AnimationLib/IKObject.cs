using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKObject : MonoBehaviour, Ibounds {

    Collider c;
    [HideInInspector]
    public Bounds bounds;
    [HideInInspector]
    public float dstToPlayer;
    [HideInInspector]
    public Vector3 closetPointToPlayer;
    float topY;

    public Material ikObjectMat;
    Material mat;

    [HideInInspector]
    static Color color;

    //represent the 4 top vertices
    public Vector3[] vertices = new Vector3[4];

    Transform targetAnimatorTransform;
    Vector3 dst;
    public delegate void FollowAnimatorTransform();
    public FollowAnimatorTransform followAnimatorTransform;

    void Start() {
        mat = Instantiate(ikObjectMat);
        GetComponent<MeshRenderer>().material = mat;
        CreateBounds();  
    }

    private void Update()
    {
        bounds.center = transform.position;
        if (followAnimatorTransform!= null)
        {
            followAnimatorTransform();
        }
    }

    public void FollowTarget()
    {
        Vector3 dst2D = new Vector3(dst.x, transform.position.y, dst.z);
        float angle = Vector3.Angle(targetAnimatorTransform.forward, dst2D);
        if (angle > 80)
        {
            Vector3 cross = Vector3.Cross(targetAnimatorTransform.forward, dst2D);
            Vector3 newDst;
            if (cross.y < 0)
            {
                newDst = Quaternion.AngleAxis(-60, targetAnimatorTransform.up) * targetAnimatorTransform.forward;
            }
            else {
                newDst = Quaternion.AngleAxis(60, targetAnimatorTransform.up) * targetAnimatorTransform.forward;
            }
             dst = new Vector3(newDst.x,dst.y,newDst.z);
        }
        transform.position = targetAnimatorTransform.position + dst;
    }

    public void SetAnimatorTargetTF(Transform t)
    {
        targetAnimatorTransform = t;
        dst = transform.position - targetAnimatorTransform.position;
    }

    public void CreateBounds()
    {
        c = GetComponent<BoxCollider>();
        bounds = c.bounds;
        vertices[0] = bounds.center + new Vector3(bounds.size.x, bounds.size.y, bounds.size.z) / 2;
        vertices[1] = bounds.center + new Vector3(-bounds.size.x, bounds.size.y, bounds.size.z) / 2;
        vertices[2] = bounds.center + new Vector3(-bounds.size.x, bounds.size.y, -bounds.size.z) / 2;
        vertices[3] = bounds.center + new Vector3(bounds.size.x, bounds.size.y, -bounds.size.z) / 2;
        topY = transform.position.y + bounds.size.y / 2;
    }

    public Bounds Bounds { get { return bounds; } }
      
    public Vector3[] Vertices { get { return vertices; } }

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
