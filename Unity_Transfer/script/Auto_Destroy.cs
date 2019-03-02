using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto_Destroy : MonoBehaviour {

    // Use this for initialization
    public float Destroy_Time = 2.0f;
    private void OnEnable()
    {
        Destroy(this.gameObject, Destroy_Time);
    }
}
