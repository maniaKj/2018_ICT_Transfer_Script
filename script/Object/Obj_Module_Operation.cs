using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 : Module_Lable, Circuit_Node, Puzzle_Part_Hologram
public class Obj_Module_Operation : MonoBehaviour {
    [HideInInspector]public GameObject[] child_Module;
    [Header("데이터 ID 입력(여러개 입력 가능)")] public int[] module_Operation_Array;
    public bool module_Consistency = false;
    
    //private
    TransferableDataObject[] child_Module_script;
    bool Is_Monitor = false;
    int child_module_Num = 0;

    void Reset() //자식 오브젝트 중 모듈 오브젝트 찾기
    {
        Awake();
    }

    void Awake() {
        if (GetComponent<Monitor>() != null)
        {
            Is_Monitor = true;
        }//해당 오브젝트가 모니터일 경우 결과값 도출 모듈을 제외해서 모듈 탐색

        child_Module = new GameObject[10];
        Find_Module_Obj(this.gameObject);
        GameObject[] temp_Modules = child_Module;
        child_Module = new GameObject[child_module_Num];
        child_Module_script = new TransferableDataObject[child_module_Num];
        for (int i = 0; i < child_module_Num; i++)
        {
            child_Module[i] = temp_Modules[i];
            child_Module_script[i] = child_Module[i].GetComponent<TransferableDataObject>();
            
        }
        if (GetComponent<Puzzle_Part_Hologram>() != null)  Module_ID_Change();

        if (GetComponent<Obj_Module_Deploy>() != null) GetComponent<Obj_Module_Deploy>().child_Module = child_Module;
    }

    void Module_Check_Consistency()
    {
        bool module_Check = false;

        int[] Correct_Code_Part = new int[5];
        bool[] tmp = new bool[5];
        bool[] OverCheck = new bool[5];
        foreach (int opID in module_Operation_Array)
        {
            bool totalCheck = true;
            for (int i = 0; i < 5; i++) {
                Correct_Code_Part[i] = (int)((opID / Mathf.Pow(100, i)) % 100);
                if (Correct_Code_Part[i] == 0) tmp[i] = true;
                else
                {
                    for(int j = 0; j < child_Module_script.Length; j++)
                    {
                        if(child_Module_script[j].ID == Correct_Code_Part[i] && OverCheck[j] == false)
                        {
                            tmp[i] = true;
                            OverCheck[j] = true;
                            break;
                        }
                    }
                }
            }
            foreach (bool checked_tmp in tmp) if (!checked_tmp) totalCheck = false;
            if(totalCheck) module_Check = true;
            
        }  // 적합한 모듈이 있는지 확인
        if (module_Check)
        {
           // Debug.Log("From 'Obj_Module_Operation' Learning Start");
            module_Consistency = true;
        }
        //else Debug.Log("From 'Obj_Module_Operation' Can't Run module : ");
    } //적절한 모듈이 위치했는지 확인 그 다음 변수 변경

    //모듈이 바뀔 때마다 자식으로부터 신호 받음, 나중에 자식 배열 값 추가할 것
    public void OnChildChange(int moduleID)
    {
        module_Consistency = false;
        Module_Check_Consistency();
        SendMessage("Get_Module_Signal", moduleID);// 여기 나중에 수정
        //Debug.Log("From Obj_Module_Operation Get Child Signal child Module ID : " + child_Module_script[0].ModuleID);
    }

    void Module_ID_Change()
    {
        int i = 0;
        foreach (GameObject module in child_Module)
        {
            if (GetComponent<Puzzle_Part_Hologram>() != null)
            {
                int sectionID = GetComponent<Puzzle_Part_Hologram>().hologram_Part_ID;
            }
            i++;
        }
    }//모듈 특성 많아지면 여길 활용할 것

    void Find_Module_Obj(GameObject target) //자식 오브젝트 찾기 + 인식처리되는 모듈에 한해서
    {
        Transform temp = target.transform;
        //Debug.Log("From Obj_Module_Operation module debug Obj " + this.gameObject + " child num : " + temp.childCount);
        for(int i=0;i<temp.childCount; i++)
        {
            if (temp.GetChild(i).GetComponent<TransferableDataObject>() != null)
            {
                child_Module[child_module_Num] = temp.GetChild(i).gameObject;
                child_module_Num++;
            }
            else Find_Module_Obj(temp.GetChild(i).gameObject);
        }
    }
}
