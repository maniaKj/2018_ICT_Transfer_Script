using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dynamic_Cube : MonoBehaviour {
    public int[] cube_OP_Array;
    public float cubeUp_dist = 10.0f;
    public float cubeDown_dist = 5.0f;
    public float cube_Speed = 7.0f;
    public GameObject[] child_Obj;
    public int[] cube_Activating;  // -1 : donw, 0: normal, 1: up
    Transform[] cube_Transform;
    float[] cube_origianl_positioin_y;

    void Awake()
    {
        cube_Activating = new int[transform.childCount];
        child_Obj = new GameObject[transform.childCount];
        cube_Transform = new Transform[transform.childCount];
        cube_origianl_positioin_y = new float[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            child_Obj[i] = transform.GetChild(i).gameObject;
            cube_Transform[i] = child_Obj[i].transform;
            cube_origianl_positioin_y[i] = child_Obj[i].transform.position.y;
        }
    }

    void Update () {
		for(int i = 0; i < transform.childCount; i++)
        {
            if (cube_Activating[i] == 1) Cube_Up(i);
            else if (cube_Activating[i] == -1) Cube_Down(i);
        }
	}

    public void Get_Signal()
    {

    }

    public void Obj_Func_Signal()
    {
        
    }

    void Cube_Up(int cubeNum)
    {
        if (cube_Transform[cubeNum].position.y - cube_origianl_positioin_y[cubeNum] < cubeUp_dist) cube_Transform[cubeNum].Translate(Vector3.up * cube_Speed * Time.deltaTime);
        else cube_Activating[cubeNum] = 0;
    }

    void Cube_Down(int cubeNum)
    {
        if (cube_origianl_positioin_y[cubeNum] < cube_Transform[cubeNum].position.y) cube_Transform[cubeNum].Translate(Vector3.down * cube_Speed * Time.deltaTime);
        else cube_Activating[cubeNum] = 0;
    }

    void Script_Off()
    {
        this.enabled = false;
    }
}
