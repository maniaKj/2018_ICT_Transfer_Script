using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aim_UI : MonoBehaviour {
    [Header("Object and File Applying")]
    public GameObject UIobj_PARENT;

    [Space(40)]
    [Header("Debuging Check Variable")]
    public bool ray_Cached;
    public int gun_Data_Buffer;
    public GameObject Showing_Obj;
    public GameObject[] UIObj;
    public RaycastHit ray_Hit_Obj;

    //Hide
    bool Machine_Circuit_Check = false;

    void Awake()
    {
        UIobj_PARENT = GameObject.Find("Aim+Explane");
        if(UIobj_PARENT != null)
        {
            UIObj = new GameObject[UIobj_PARENT.transform.childCount];
            for(int i = 0;i < UIobj_PARENT.transform.childCount; i++)
            {
                UIObj[i] = UIobj_PARENT.transform.GetChild(i).gameObject;
            }
        }
        for (int i = 0; i < UIObj.Length; i++) UIObj[i].SetActive(false);
        Showing_Obj = UIObj[0];
    }
    public void Get_Signal(bool Ray_Checked, RaycastHit hit_Obj, int gun_Buffer, bool Machine_Circuit)
    {
        ray_Hit_Obj = hit_Obj;
        ray_Cached = Ray_Checked;
        gun_Data_Buffer = gun_Buffer;
        Machine_Circuit_Check = Machine_Circuit;
        if (Ray_Checked) Show_UI(Target_Distinguishing(hit_Obj.transform.gameObject));
        else Show_UI(0);
    }

    private int Target_Distinguishing(GameObject target)
    {
        int ui_Num = 0; // 0 : none, 1 : Data Get, 2 : Data Transfer, 3 : Get Machine, 4 : Machine Shoot, 5 : Machine Shoot & Data Get, 6 : Machine Set on Circuit, 7 : Data Hologram Identify, 8 : Drawing Panel, 9 : Circuit_Drawing, 10 : Timer
        Module_Lable ML = target.GetComponent<Module_Lable>();
        Puzzle_Part_Hologram PPH = target.GetComponent<Puzzle_Part_Hologram>();
        Tower_test TT = target.GetComponent<Tower_test>();
        Data_Hologram DT = target.GetComponent<Data_Hologram>();
        Drawing Draw = target.GetComponent<Drawing>();
        Puzzle_Drawing p_Draw = target.GetComponent<Puzzle_Drawing>();
        Timer Tm = target.GetComponent<Timer>();

        if (ML != null)
        {
            if (gun_Data_Buffer == 0 && ML.ModuleID % 100 != 0 && ML.Check_Module_Restrict(gun_Data_Buffer) == 1)
            {
                ui_Num = 1;
            }
            else if (gun_Data_Buffer != 0 && ML.ModuleID % 100 == 0 && ML.Check_Module_Restrict(gun_Data_Buffer) == 1)
            {
                ui_Num = 2;
            }
            else if (ML.Check_Module_Restrict(gun_Data_Buffer) == 2)
            {
                ui_Num = 2;
            }
        }
        else if (Tm != null)
        {
            ui_Num = 10;
        }
        else if (PPH != null)
        {
            if (gun_Data_Buffer == 0 && PPH.hologram_Part_ID != 0 && PPH.module_Linked.GetComponent<Module_Lable>().Check_Module_Restrict(gun_Data_Buffer) != 0)
            {
                ui_Num = 1;
            }//홀로그램 분해
        }//퍼즐 홀로그램 조준
        else if (TT != null)
        {
            if(TT.TowerFunc == Tower_test.TowerStyle.Button_Sig1 || TT.TowerFunc == Tower_test.TowerStyle.Button_Sig2 || TT.TowerFunc == Tower_test.TowerStyle.Button_Sig3)
            {
                ui_Num = 11;
            }

            if (gun_Data_Buffer == 0)
            {
                //Debug.Log("From player_shot Get Cnn_machine");
                if (TT.TowerFunc == Tower_test.TowerStyle.CNN)
                {
                    if (TT.Circuit_On) ui_Num = 5; else ui_Num = 3;
                }
                else if (TT.TowerFunc == Tower_test.TowerStyle.Electric)
                {
                    if (TT.Circuit_On) ui_Num = 5; else ui_Num = 3;
                }
            }
            else
            {
                if (TT.TowerFunc == Tower_test.TowerStyle.CNN)
                {
                    if (TT.Circuit_On) ui_Num = 4; else ui_Num = 0;
                }
                else if (TT.TowerFunc == Tower_test.TowerStyle.Electric)
                {
                    if (TT.Circuit_On) ui_Num = 4; else ui_Num = 0;
                }
            }
        }//인식기 조준
        else if (DT != null)
        {
            if (ray_Hit_Obj.distance < 10)
            {
                ui_Num = 7;
            }
        }
        else if (Draw != null)
        {
            ui_Num = 8;
        }
        else if (p_Draw != null)
        {
            ui_Num = 9;
        }
        else
        {
            if (Machine_Circuit_Check) ui_Num = 6; else ui_Num = 0;

        }
        return ui_Num;
    }

    private void Show_UI(int num)
    {
        Showing_Obj.SetActive(false);
        Showing_Obj = UIObj[num];
        Showing_Obj.SetActive(true);
    }
}
