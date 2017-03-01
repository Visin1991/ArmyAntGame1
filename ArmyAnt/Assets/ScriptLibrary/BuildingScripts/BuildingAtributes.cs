using UnityEngine;
using System.Collections;

public class BuildingAtributes : MonoBehaviour {
    public float BuildingHealth = 100;
    public void MinusHealth(float amount)
    {
        BuildingHealth -= amount;
    }

}
