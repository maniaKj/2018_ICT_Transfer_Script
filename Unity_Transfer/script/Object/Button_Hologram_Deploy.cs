using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 Obj_Module_Deploy
public class Button_Hologram_Deploy : MonoBehaviour {
    //나중에 필요하면 이름 바꾸기
    public enum Button_Func_Type { Hologram_Deploy};
    public Button_Func_Type Button_Func;
    public GameObject Monitor_Obj;

    void Awake()
    {
        Monitor_Obj = transform.parent.gameObject;
    }

	public void Get_Signal()
    {
        if(Button_Func == Button_Func_Type.Hologram_Deploy) Monitor_Obj.GetComponent<Obj_Module_Deploy>().Get_Signal();
    }
}
