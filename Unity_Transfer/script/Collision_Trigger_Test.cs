using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision_Trigger_Test : MonoBehaviour {
    
	void OnCollisionEnter () {
        Debug.Log("Collision Detect");
	}

    void OnTriggerEnter()
    {
        Debug.Log("Trigger Detect");
    }
}
