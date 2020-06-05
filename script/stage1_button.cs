using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage1_button : MonoBehaviour {
    public GameObject playerObj;
    public GameObject stageObj;
	// Use this for initialization
	void Start () {
        playerObj = GameObject.FindGameObjectWithTag("player");
        stageObj = GameObject.FindGameObjectWithTag("stage");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionStay(Collision collision)
    {
        Debug.Log("button stay");
        if (collision.gameObject.tag == "player")
        {
            
            stageObj.GetComponent<stage1_Ai>().Button_signal();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("button Enter");
        if (collision.gameObject.tag == "player")
        {

            stageObj.GetComponent<stage1_Ai>().Button_signal();
        }
    }
}
