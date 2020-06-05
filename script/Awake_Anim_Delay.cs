using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awake_Anim_Delay : MonoBehaviour {
    public string Parameter_Name = "state";
    public float anim_awake_time;
    Animator animation_controll;

    void Awake()
    {
        animation_controll = GetComponent<Animator>();
        StartCoroutine(Awake_Delay());
    }

    IEnumerator Awake_Delay()
    {
        yield return new WaitForSeconds(anim_awake_time);
        animation_controll.SetInteger(Parameter_Name, 1);
    }
}
