using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DLB : MonoBehaviour {
    public int learningPoint = 0;
    bool learnToggle = false;

    // Use this for initialization
    void OnCollisionEnter(Collision bullet)
    {
        if (bullet.gameObject.tag == "bullet")
        {
            learnToggle = true;
        }
    }
    void Update()
    {
        if (learnToggle)
        {
            learningPoint++;
            if (learningPoint == 10000) learnToggle = false;
        }
    }

    // Update is called once per frame
 
}
