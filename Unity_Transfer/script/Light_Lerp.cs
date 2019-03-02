using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Lerp : MonoBehaviour {
    public Color Color_start;
    public Color Color_end;
    public float color_speed = 0.2f;
    float tmp = 0.0f;
    bool updown = true;

	void Update () {
        LightIntense();
	}

    void LightIntense()
    {
        switch (updown)
        {
            case true:
                if (tmp < 1) tmp += Time.deltaTime * color_speed;
                else ScriptOff();
                break;
            case false:
                if (tmp >= 0) tmp -= Time.deltaTime * color_speed;
                else ScriptOff();
                break;
        }
        GetComponent<Light>().color = Color.Lerp(Color_start, Color_end, tmp);
    }

    public void LightToggle()
    {
        updown = !updown;
    }

    void ScriptOff()
    {
        this.enabled = false;
    }
}
