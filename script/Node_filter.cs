using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_filter : MonoBehaviour {
    public int filter_ID;
    public float filter_Detect_Range = 5.0f;

    //private
    float Detect_delay_time = 0.5f;
    Collider[] node_Detected_Current;
    Collider[] node_Detected_Previous;
    Transform tf;
    Node_classify node_classify_script;
    bool Detect_delay_toggle = true;

	void Awake () {
        tf = transform;
	}
	
	void Update () {
		
	}

    IEnumerator Detect_Node_In_Range()
    {
        Detect_delay_toggle = false;
        node_Detected_Current = Physics.OverlapSphere(tf.position, filter_Detect_Range);
        Node_DetectCheck_Chage();
        yield return new WaitForSeconds(Detect_delay_time);
        Detect_delay_toggle = true;
    }

    void Node_DetectCheck_Chage()
    {
        foreach (Collider node in node_Detected_Previous)
        {
            if (node.GetComponent<Node_classify>() != null)
            {
                node.GetComponent<Node_classify>().Detect_Check = false;
            }
        }
        foreach (Collider node in node_Detected_Current)
        {
            if (node.GetComponent<Node_classify>() != null)
            {
                node.GetComponent<Node_classify>().Detect_Check = true;
            }
        }
        foreach (Collider node in node_Detected_Previous)
        {
            if (node.GetComponent<Node_classify>() != null && !node.GetComponent<Node_classify>().Detect_Check)
            {
            }
        }
    }
}
