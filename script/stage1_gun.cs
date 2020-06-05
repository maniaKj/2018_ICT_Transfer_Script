using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage1_gun : MonoBehaviour {
    public GameObject playerObj;
    public GameObject stageObj;
    public GameObject playerGunObj;
    // Use this for initialization
    void Start () {
        playerObj = GameObject.FindGameObjectWithTag("player");
        stageObj = GameObject.FindGameObjectWithTag("stage");
    }
	
	// Update is called once per frame
	void OnCollisionEnter (Collision collision) {
		if(collision.gameObject.tag == "player")
        {
            playerObj.GetComponent<player_shot>().enabled = true;
            playerGunObj.SetActive(true);
            Destroy(gameObject);
        }
	}
}
