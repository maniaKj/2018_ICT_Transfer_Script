using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine_Operation : MonoBehaviour {
    public GameObject ob;
    Obj_Module_Operation script_Mop;

    void Awake()
    {
        script_Mop = GetComponent<Obj_Module_Operation>();
    }

    public void Get_Module_Signal(int module_num)
    {
        if (script_Mop.module_Consistency) Turn_On_Machine();
    }

    void Turn_On_Machine()
    {

    }
}
