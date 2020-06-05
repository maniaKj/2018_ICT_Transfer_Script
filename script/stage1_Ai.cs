using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stage1_Ai : MonoBehaviour {
    public GameObject buttonObj;
    public GameObject gunObj;
    public GameObject learnBoardObj;
    public GameObject displayObj;
    public GameObject elevatorBlock;
    public GameObject textObj;
    public int learningPoint = 0;
    bool learnToggle = true;
    Color colorStart = Color.white;
    Color colorEnd = Color.blue;

    // Use this for initialization
    void Start () {
        learnBoardObj.GetComponent<Renderer>().material.color = Color.white;
        displayObj.GetComponent<Renderer>().material.color = Color.black;

    }
	
	// Update is called once per frame

    public void Button_signal()
    {
        Debug.Log("signal on");
        if (learnToggle)
        {
            learningPoint++;
            textObj.GetComponent<TextMesh>().text = learningPoint.ToString();
            learnBoardObj.GetComponent<Renderer>().material.color = Color.Lerp(colorStart, colorEnd, learningPoint / 500);
            if (learningPoint == 500)
            {
                learnToggle = false;
                gunObj.SetActive(true);
                elevatorBlock.SetActive(false);
                displayObj.GetComponent<Renderer>().material.color = Color.white;//ai wake up
            }
        }
    }
}
