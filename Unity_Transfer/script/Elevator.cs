using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour {
    bool updown = true;
    float tmp = 0.0f;
    float velocity;
    Vector3 basePosition;
    Vector3 targetPosition;
    public float currentY;
    public float upDist = 10.0f;
    public float uptime = 1.5f;
    public float errorRange = 1.0f;

	// Use this for initialization
	void Start () {
        targetPosition = new Vector3(transform.position.x, transform.position.y + upDist, transform.position.z);
        basePosition = transform.position;
	}

    // Update is called once per frame
    void Update()
    {
        Elevating();
    }

    void Elevating()
    {
        switch (updown)
        {
            case true:
                if (targetPosition.y - currentY > errorRange) currentY = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref velocity, uptime);
                else ScriptOff();
                break;
            case false:
                if (currentY - basePosition.y > errorRange) currentY = Mathf.SmoothDamp(transform.position.y, basePosition.y, ref velocity, uptime);
                else ScriptOff();
                break;
        }
        transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
    }

    public void upDownToggle()
    {
        updown = !updown;
    }

    void ScriptOff()
    {
        this.enabled = false;
    }
}
