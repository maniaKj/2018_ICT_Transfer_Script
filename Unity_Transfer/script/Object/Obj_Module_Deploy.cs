using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 : Obj_Module_Operation, bullet
public class Obj_Module_Deploy : MonoBehaviour {
    public bool onOff = false;
    public GameObject[] child_Module;
    public GameObject[] child_Deploy_Objects; //플레이어가 직접 지정해야함.

    public int obj_Type = 0; // 0 : normal, 1 : Monitor
    public int i = 0;

    public void Awake()
    {
        InteractiveObj_MultiTag script_I = GetComponent<InteractiveObj_MultiTag>();
        if (script_I != null && script_I.ObjType == InteractiveObj_MultiTag.AllType.Monitor) obj_Type = 1;
    }

    public void Get_Signal()
    {
        if (obj_Type == 0) Deploy_Module();//일반 경우
        else if (obj_Type == 1) Deploy_specified_Obj(i++);//모니터 경우

        if (i == child_Deploy_Objects.Length) i = 0;
    }

    void Deploy_Module()
    {
        if (obj_Type == 0)
        {
            if (!onOff) foreach (GameObject child in child_Module) child.SetActive(true);
            else foreach (GameObject child in child_Module) child.SetActive(false);
            onOff = !onOff;
        }
    }

    public void Deploy_specified_Obj(int num)
    {
        foreach(GameObject child in child_Deploy_Objects) child.SetActive(false);
        child_Deploy_Objects[num].SetActive(true);
    }
}
