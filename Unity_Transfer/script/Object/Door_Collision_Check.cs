using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_Collision_Check : MonoBehaviour {
    public bool ExitCheck = false;
    public bool EnterCheck = false;
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger Enter");
        //transform.parent.GetComponent<Awsome_Door>().Door_Opened = true;
        if(EnterCheck) transform.parent.GetComponent<Awsome_Door>().Door_Signal(true);
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Trigger Exit");
        //transform.parent.GetComponent<Awsome_Door>().Door_Opened = false;
        if(ExitCheck) transform.parent.GetComponent<Awsome_Door>().Door_Signal(false);
    }
}
