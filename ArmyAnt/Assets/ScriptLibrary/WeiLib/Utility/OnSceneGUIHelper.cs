using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSceneGUIHelper : MonoBehaviour {

    LivingEntity liveEntity;
	// Use this for initialization
	void Start () {
        liveEntity = GetComponent<LivingEntity>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        GUILayout.Label(new GUIContent("Player Health : " + liveEntity.health.ToString()));
    }
}
