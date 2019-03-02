using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Take_Off_Module : MonoBehaviour {

    GameObject player_Obj;

    void Awake()
    {
        player_Obj = GameObject.FindGameObjectWithTag("player");
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.tag == "player")
        {
            col.GetComponent<player_shot>().Module_buffer_return();
        }
    }
}
