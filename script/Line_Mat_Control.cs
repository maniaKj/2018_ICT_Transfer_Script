using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line_Mat_Control : MonoBehaviour {

    public GameObject[] Curved_Line_out;
    public GameObject[] Curved_Line_in;
    [Range(0.0f, 1.0f)] public float inline_Mat_Value;
    [Range(0.0f, 1.0f)] public float outline_Mat_Value;

    private void Awake()
    {
        Curved_Line_in = GameObject.FindGameObjectsWithTag("Line_in");
        Curved_Line_out = GameObject.FindGameObjectsWithTag("Line_Out");
        StartCoroutine(Coroutine_Update());
    }

    IEnumerator Coroutine_Update()
    {
        while (true)
        {
            for (int i = 0; i < Curved_Line_out.Length; i++) Curved_Line_out[i].GetComponent<Renderer>().material.SetFloat("_Cut", outline_Mat_Value);
            for (int i = 0; i < Curved_Line_in.Length; i++) Curved_Line_in[i].GetComponent<Renderer>().material.SetFloat("_Cut", inline_Mat_Value);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
