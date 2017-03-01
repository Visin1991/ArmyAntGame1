using UnityEngine;
using System.Collections;

public class BuildingHealth : MonoBehaviour
{
    public bool hit = false;
    public float Damage = 100;
    public BuildingAtributes DB;
    
    public int holderPlayerIndex;

    public bool canDoDamage = false;
    public int otherplayer = -1;

    void Start()
    {
        DB = gameObject.GetComponent<BuildingAtributes>();
        //Debug.Log(DB.name);
        if (DB == null)
        {
            Debug.LogError(gameObject.name);
            Debug.LogError(": does not have a DistructBuilding Script");
        }
    }

    public void BeThrowed()
    {
        canDoDamage = true;
        Invoke("ResetInfo", 0.5f);
    }

    private void OnCollisionEnter(Collision other)
    {
      
       
    }

    void ResetInfo()
    {
        otherplayer = -1;
        canDoDamage = false;
    }
    
}

