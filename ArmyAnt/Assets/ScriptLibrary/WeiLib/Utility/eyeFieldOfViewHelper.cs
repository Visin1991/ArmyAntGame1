using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eyeFieldOfViewHelper : MonoBehaviour {

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 3);
    }

}
