using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_classify : MonoBehaviour {
    public int node_ID;
    public bool node_on = false;
    public float emission_Intensity = 3.0f;
    public bool Detect_Check = false;

    //private
    Color emission_color;
    Color emission_color_zero = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
    Material mt;

	void Awake () {
        mt = GetComponent<Renderer>().material;
        emission_color = new Color(emission_Intensity, emission_Intensity, emission_Intensity, 1.0f);
    }

    public void GetSignal()
    {
        Emission_OnOff(node_on);
    }

    void Emission_OnOff(bool onoff)
    {
        if (onoff) mt.SetColor("_EmissionColor", emission_color_zero);
        else mt.SetColor("_EmissionColor", emission_color);
        node_on = !node_on;
    }
}
