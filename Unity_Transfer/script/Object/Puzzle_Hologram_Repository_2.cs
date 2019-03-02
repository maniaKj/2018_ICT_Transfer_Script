using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//연계 : 
public class Puzzle_Hologram_Repository_2 : MonoBehaviour {

    //public GameObject Hologram_Puzzle_Controller;
    [Header("Apply Holo Obj")]
    public GameObject Data_Hologram;
    [Space (30)]
    [Header("Auto Applying Just leave")]
    public Transform pivot_Obj;
    public GameObject moduleObj;
    //Puzzle_Hologram script_Puzzle_Hologram;

    void Reset() { Awake(); }

    void Awake()
    {
        //if (C_hologram_Part == null || C_hologram_Part.tag == "Pivot") Debug.Log("From Puzzle_Hologram_repository 홀로그램 파트 적용 안됐음! pivot 오브젝트 말고 홀로그램 본체여야 함." + this.gameObject);
        //Hologram_Puzzle_Controller = C_hologram_Part.transform.parent.parent.gameObject;
        //script_Puzzle_Hologram = Hologram_Puzzle_Controller.GetComponent<Puzzle_Hologram>();
        //if (C_hologram_Part.GetComponent<Puzzle_Part_Hologram>() != null) C_hologram_Part.GetComponent<Puzzle_Part_Hologram>().module_Linked = moduleObj;
        //else Debug.Log("From Puzzle_Hologram_repository 오브젝트 설정 제대로 안됨. 홀로그램 본체(피봇아님)을 집어 넣을 것 : " + this.gameObject);
        for (int i = 0; i < transform.childCount; i++) if (transform.GetChild(i).tag == "Pivot") pivot_Obj = transform.GetChild(i); //피봇 오브젝트 참조

        StartCoroutine(Wait_And_Awake());
    }

    public void Get_Module_Signal(int get_moduleID)
    {
        //Debug.Log("From Puzzle_Hologram_Repository_2 ModuleID : " + get_moduleID);
        GameObject target;
        if (get_moduleID == 0)
        {
            moduleObj.SetActive(true);
            moduleObj.GetComponent<Module_Lable>().GetSignal(0);
            Data_Hologram = null;
            GameObject.FindGameObjectWithTag("player").GetComponent<player_shot>().previous_script_ML = moduleObj.GetComponent<Module_Lable>();
            //Debug.Log("ModuleSig 0");
        }//Hologram Consume
        else if (get_moduleID % 100 < 90 && get_moduleID % 100 >= 30)
        {
            //Debug.Log("ModuleSig 30");
            /*moduleObj.SetActive(false);
            target = moduleObj.GetComponent<Module_Lable>().Connected_Func_machine;
            moduleObj.GetComponent<Module_Lable>().ModuleID = 0;
            moduleObj.GetComponent<Module_Lable>().Change_Module_Appearance(0);
            target.transform.position = pivot_Obj.transform.position;
            target.transform.rotation = pivot_Obj.transform.rotation;
            Data_Hologram = target;
            target.GetComponent<Data_Hologram>().Get_Appear_Signal();*/
        }//Get Hologram

        /*GameObject[] puzzle_Section = script_Puzzle_Hologram.puzzle_Section_pivot;
        int target_Num = 0;
        foreach (GameObject temp in puzzle_Section)
        {
            if (temp.transform.GetChild(0).GetComponent<Puzzle_Part_Hologram>().hologram_Part_ID == get_moduleID % 100)
            {
                script_Puzzle_Hologram.Hologram_positioning(target_Num, pivot_Obj.position, pivot_Obj.rotation, moduleObj);
                break;
            }
            target_Num++;
        } //모듈값 홀로그램 위치시키기 */
    }
    public void Get_Exception_Signal(GameObject Target_Obj)
    {
        //Debug.Log("ModuleSig E");
        moduleObj.SetActive(true);
        moduleObj.GetComponent<Module_Lable>().GetSignal(Target_Obj.GetComponent<Data_Hologram>().moduleID);
        Data_Hologram = null;
    }//홀로그램 흡수할 때, 플레이어가 이미 데이터를 가지고 있어서 더 흡수하질 못하는 경우

    IEnumerator Wait_And_Awake()
    {
        yield return new WaitForSeconds(0.1f);
        moduleObj = GetComponent<Obj_Module_Operation>().child_Module[0];
        //Debug.Log("From Puzzle_Hologram_Repository_2 MOP's child Module : " + GetComponent<Obj_Module_Operation>().child_Module[0]);
        //Get_Module_Signal(C_hologram_Part.GetComponent<Puzzle_Part_Hologram>().hologram_Part_ID);
    }
}
