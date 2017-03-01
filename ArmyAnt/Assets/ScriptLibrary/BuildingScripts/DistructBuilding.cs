using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Rigidbody))]
public class DistructBuilding : MonoBehaviour {
    public BuildingAtributes BA;
    public GameObject prefab;
    public GameObject Box;
    // private float ExposionPower = 10.0f;
    //private float ExplosionRadius = 5.0f;
    Rigidbody Rbox;
    void Start ()
    {
        BA = gameObject.GetComponent<BuildingAtributes>();
        Box = this.gameObject;
        //AddRigidBody();
    }

	void Update ()
    {
        if (BA.BuildingHealth > 0)
        {    
            //StartCoroutine(Despawner(10));
        }
        else
        {
           // AddExpolison();
            this.transform.parent = null;
            StartCoroutine(Despawner(1f));
        }
    }
    void AddRigidBody()
    {
        if(!Box.GetComponent<Rigidbody>())
        {
            Box.AddComponent<Rigidbody>();
            Rbox = gameObject.GetComponent<Rigidbody>();
            Rbox.useGravity = true;
           // GameObject smoke = (GameObject)Instantiate(prefab, Box.transform.position, Box.transform.rotation);
            //Destroy(smoke, 2.0f);
        }
    }
    /*void RemoveRigidBody()
    {
        if(Box.GetComponent<Rigidbody>())
        {
            Destroy(Box.GetComponent<Rigidbody>());
        }
    }*/
    
    /*void AddExpolison()
    {
        /Vector3 explosionPos = transform.position;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
            Debug.Log("there is no rigidbody on this gameobject");
        //rb.AddExplosionForce(ExposionPower,explosionPos, ExplosionRadius,0.2f);

    }*/
    IEnumerator Despawner(float time)
    {
        
        yield return new WaitForSeconds(time);
        //make the dissolve shader
        Destroy(gameObject);
    }
}
