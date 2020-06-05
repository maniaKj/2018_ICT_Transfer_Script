using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 Puzzle_Part_Hologram, Puzzle_Node
public class Puzzle_Hologram : MonoBehaviour {
    //필수 변수
    public GameObject[] puzzle_Section_pivot;
    
    public bool[] Node_Check_result = new bool[4];
    public bool[] Node_Check_Cleared = new bool[4];

    public List<GameObject> Module_Group = new List<GameObject>();
    public bool Assemble_Complete = false;

    //private
    GameObject[] puzzle_Node_Group1 = new GameObject[80];
    GameObject[] puzzle_Node_Group2 = new GameObject[80];
    GameObject[] puzzle_Node_Group3 = new GameObject[80];
    GameObject[] puzzle_Node_Group4 = new GameObject[80];

    GameObject[] node_Group;
    int a = 0, b = 0, c = 0, d = 0;
    float node_Check_Delay = 2.0f;

    void Reset()
    {
        Awake();
    }

    void Awake()
    {
        if (puzzle_Section_pivot.Length == 0) Debug.Log("From Puzzle_Hologram 퍼즐 피봇 적용 안됐음 pivot 오브젝트여야 함" + this.gameObject);
        else foreach(GameObject tmp in puzzle_Section_pivot) if(tmp.tag != "Pivot") Debug.Log("From Puzzle_Hologram 퍼즐 피봇 적용 안됐음 pivot 오브젝트여야 함" + this.gameObject);
        int zero_Check = 0;
        foreach (GameObject temp in puzzle_Section_pivot) if (temp.transform.GetChild(0).GetComponent<Puzzle_Part_Hologram>().hologram_Part_ID == 0) zero_Check++;
        if (zero_Check != 1) Debug.Log("From Puzzle_Hologram 각 퍼즐 파트 ID 재확인 할것 중심파트 ID는 무조건 0 다른 파트는 0 외의 다른 숫자 " + this.gameObject);
        puzzle_Node_Group1 = new GameObject[80];
        puzzle_Node_Group2 = new GameObject[80];
        puzzle_Node_Group3 = new GameObject[80];
        puzzle_Node_Group4 = new GameObject[80];
        //자식 오브젝트 중 노드 오브젝트 전부 찾아서 분류
        Node_classifier(this.gameObject);
        Node_array_arrange();
        Find_All_Child_Module(this.gameObject);

        Node_Check_result = new bool[4];
        for (int i = 0; i < Node_Check_result.Length; i++) Node_Check_result[i] = false;

        StartCoroutine(Node_Check_Routine());
    }

    public bool Get_Assemble_Signal (Puzzle_Part_Hologram.Assembling_Data receive_Data) {
         Hologram_Assembling(receive_Data);
        return true;
	} //홀로그램 조립 신호 받을 때

    void Hologram_Assembling(Puzzle_Part_Hologram.Assembling_Data receive_Data)
    {
        int target_Num = 0;
        foreach (GameObject section in puzzle_Section_pivot)
        {
            if (section.transform.GetChild(0).GetComponent<Puzzle_Part_Hologram>().hologram_Part_ID == receive_Data.assemble_Part_ID) break;
            target_Num++;
        } //ID값으로 오브젝트 배열에서 일치하는 오브젝트 찾아내기

        receive_Data.Data_Sender_Obj.GetComponent<Puzzle_Part_Hologram>().Object_Linking(puzzle_Section_pivot[target_Num].transform.GetChild(0).gameObject, receive_Data.linked_Module_Obj.GetComponent<TransferableDataObject>().ID / 100 % 10);
        //가져다 붙일 홀로그램 + 연결되는 모듈 ID를 보낸다.

        Debug.Log("From Puzzle_Hologram Assembling");
        //Debug.Log("From Puzzle_Hologram linked obj : " + receive_Data.linked_Module_Obj.GetComponent<Module_Lable>().ModuleID);
        Hologram_positioning(target_Num, receive_Data.assemble_Position, receive_Data.assemble_Rotation, receive_Data.linked_Module_Obj); ;

        //Debug.Log("From Puzzle_Hologram assembled obj ID : " + receive_Data.assemble_Part_ID + ", name : " + puzzle_Section[receive_Data.assemble_Part_ID]);
    } //홀로그램 조립

    public void Hologram_positioning(int target_Num, Vector3 target_position, Quaternion target_rotation, GameObject linked_Module)
    {
        Puzzle_Part_Hologram script_PPH = puzzle_Section_pivot[target_Num].transform.GetChild(0).GetComponent<Puzzle_Part_Hologram>();
        puzzle_Section_pivot[target_Num].SetActive(true);
        puzzle_Section_pivot[target_Num].transform.position = target_position;
        puzzle_Section_pivot[target_Num].transform.rotation = target_rotation;
        puzzle_Section_pivot[target_Num].transform.GetChild(0).GetComponent<Puzzle_Part_Hologram>().module_Linked = linked_Module;
        if (linked_Module.GetComponent<TransferableDataObject>().ID >= 20000) puzzle_Section_pivot[target_Num].transform.GetChild(0).localScale = script_PPH.original_Scale * script_PPH.scale_Muliply;
        else if(linked_Module.GetComponent<TransferableDataObject>().ID < 20000) puzzle_Section_pivot[target_Num].transform.GetChild(0).localScale = script_PPH.original_Scale;
        linked_Module.GetComponent<TransferableDataObject>().OnActivate(false);
    } // 홀로그램 위치시키기

    bool Check_Node_onOff(int group_Num)
    {
        bool temp_Check = true;

        switch (group_Num)
        {
            case 1:
                node_Group = puzzle_Node_Group1;
                break;
            case 2:
                node_Group = puzzle_Node_Group2;
                break;
            case 3 :
                node_Group = puzzle_Node_Group3;
                break;
            case 4 :
                node_Group = puzzle_Node_Group4;
                break;
            default:
                break;
        }
        
        foreach(GameObject node in node_Group)
            if (!node.GetComponent<Puzzle_Node>().node_OnOFF)
                temp_Check = false;

        if (temp_Check) return true;

        return false;
    } //노드 전부 커버됐는지 확인용

    IEnumerator Node_Check_Routine()
    {
        while (true)
        {
            yield return new WaitForSeconds(node_Check_Delay);
            for (int i = 2; i <= 4; i++) Node_Check_result[i-1] = Check_Node_onOff(i);
            bool module_assemble_Check = true;
            foreach (GameObject module in Module_Group) if (module.GetComponent<TransferableDataObject>().ID % 100 == 0) module_assemble_Check = false;
            if (module_assemble_Check) Assemble_Complete = true;
            else Assemble_Complete = false;
        }
    } //노드 커버 여부 주기적으로 확인

    void Node_classifier(GameObject target)
    {
        Transform temp = target.transform;
        for (int i = 0; i < temp.childCount; i++)
        {
            //if(temp.GetChild(0).gameObject != null) Node_classifier(temp.GetChild(i).gameObject);
            if (temp.GetChild(i).gameObject.GetComponent<Puzzle_Node>() != null)
            {
                temp.GetChild(i).gameObject.GetComponent<Puzzle_Node>().Hologram_control_Obj = this.gameObject;
                //Debug.Log("From Puzzle_Hologram find node Obj : " + temp.GetChild(i).gameObject);
                switch (temp.GetChild(i).gameObject.GetComponent<Puzzle_Node>().Node_ID)
                {
                    case 0:
                        puzzle_Node_Group1[a] = temp.GetChild(i).gameObject;
                        a++;
                        break;
                    case 1:
                        puzzle_Node_Group2[b] = temp.GetChild(i).gameObject;
                        b++;
                        break;
                    case 2:
                        puzzle_Node_Group3[c] = temp.GetChild(i).gameObject;
                        c++;
                        break;
                    case 3:
                        puzzle_Node_Group4[d] = temp.GetChild(i).gameObject;
                        d++;
                        break;
                    default:
                        break;
                }
            }
            else Node_classifier(temp.GetChild(i).gameObject);
        }
    }
    void Node_array_arrange() //귀찮아서 코드 더럽게 짬
    {
        node_Group = puzzle_Node_Group1;
        puzzle_Node_Group1 = new GameObject[a];
        int i = 0;
        while(i<a)
        {
            puzzle_Node_Group1[i] = node_Group[i];
            i++;
        }
       node_Group = puzzle_Node_Group2;
        puzzle_Node_Group2 = new GameObject[b];
        i = 0;
        while (node_Group[i] != null)
        {
            puzzle_Node_Group2[i] = node_Group[i];
            i++;
        }
        node_Group = puzzle_Node_Group3;
        puzzle_Node_Group3 = new GameObject[c];
        i = 0;
        while (node_Group[i] != null)
        {
            puzzle_Node_Group3[i] = node_Group[i];
            i++;
        }
        node_Group = puzzle_Node_Group4;
        puzzle_Node_Group4 = new GameObject[d];
        i = 0;
        while (node_Group[i] != null)
        {
            puzzle_Node_Group4[i] = node_Group[i];
            i++;
        }
    } 
    void Find_All_Child_Module(GameObject target)
    {
        Transform temp = target.transform;
        for (int i = 0; i < temp.childCount; i++)
        {
            if (temp.GetChild(i).GetComponent<TransferableDataObject>() != null) Module_Group.Add(temp.GetChild(i).gameObject);
            else Find_All_Child_Module(temp.GetChild(i).gameObject);
        }
    }//자식 오브젝트 중 모듈 다 찾기 => 링크 모듈확인해서 조립 완성됐는지 확인

}
