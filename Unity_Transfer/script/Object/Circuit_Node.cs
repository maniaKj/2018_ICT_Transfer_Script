using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 Circuit, Obj_Module_Operation
public class Circuit_Node : MonoBehaviour {
    [Header("Insert Data ID for function")] public int target_ID = 0;
    bool root_Node = false; //시작 노드 power 항상 on
    [HideInInspector] public List<GameObject> child_Circuit = new List<GameObject>();
    [Header("Insert Target Node Obj")] public GameObject[] connected_Node;

    [Space(10)]
    [Tooltip("아랫부분 잘 안쓸 듯")]
    [Header("Below Variable is for check")]
    public int signal_ID;
    public bool signal_Correct = false;
    bool Node_Hacked = false;
    
    //private
    Obj_Module_Operation module_Script;
    bool power_Onoff = false;

    public void Awake()
    {
        module_Script = GetComponent<Obj_Module_Operation>();
        if (root_Node) power_Onoff = true;
        Find_All_Circuit_Obj(this.gameObject);
    }//자식 오브젝트 중 회로 오브젝트 찾기

	public void Get_Circuit_Signal(int signalType) 
    {
        Debug.Log("From Circuit_Node Get Circuit Signal : " + this.gameObject + " signal Num : " + signalType);

        signal_ID = signalType; 
        if (signal_ID == target_ID) signal_Correct = true; else signal_Correct = false;
        Send_Circuit_Signal(signal_ID);

        /*if (signalType != 8 && !Node_Hacked)
        {
            signal_ID = signalType;
            if (signal_ID == 0 && !root_Node) power_Onoff = false; //처음 시작 노드는 signal_ID 가 0이 되면 안됨
            else if (signal_ID != 0) power_Onoff = true;
            if(signal_ID == target_ID && power_Onoff) Send_Circuit_Signal(signal_ID);
            else Send_Circuit_Signal(0);


            //if (power_Onoff && signal_Correct) Send_Circuit_Signal(signal_ID);
            //else Send_Circuit_Signal(0);
        }
        else if(signalType == 8)
        {
            signal_ID = signalType;
            Node_Hacked = true;
        }
        else if (Node_Hacked)
        {
            if (signalType == 7) Node_Hacked = false;
            else signal_ID = signalType;
            if (!Node_Hacked)
            {
                SendMessage("Get_Circuit_Signal_ANOTHER", signal_ID);
                Send_Circuit_Signal(signal_ID);

            }
        }*/
    }//모듈 신호 && 전력 신호 확인할 것

    void Send_Circuit_Signal(int signalType)
    {
        foreach(GameObject circuit in child_Circuit) circuit.SendMessage("Get_Circuit_Signal", signalType);
        foreach (GameObject next_node in connected_Node) next_node.SendMessage("Get_Circuit_Signal", signalType);
        if(signal_Correct) SendMessage("Get_Circuit_Signal_ANOTHER", signalType); // 여기 나중에 수정
        else SendMessage("Get_Circuit_Signal_ANOTHER", 0.0f);
    }

    //여기도 나중에 일반화
    void Get_Module_Signal(int module_ID)
    {
        if (module_Script.module_Consistency) Get_Circuit_Signal(module_ID);
        else Get_Circuit_Signal(0);
    }

    public void Get_Highpass_Signal(int signal_Num)
    {
        Send_Circuit_Signal(8);
    }//함부로 쓰지 말것. 해킹 오브젝트 전용

    void Find_All_Circuit_Obj(GameObject parent)
    {
        for (int i = 0; i < parent.transform.childCount; i++) if (parent.transform.GetChild(i).GetComponent<Circuit>() != null)
            {
                child_Circuit.Add(parent.transform.GetChild(i).gameObject);
                if (parent.transform.childCount != 0) Find_All_Circuit_Obj(parent.transform.GetChild(i).gameObject);
            }
    }

    private void OnDrawGizmos()
    {
        //for(int i = 0; i < connected_Node.Length;i++) if(connected_Node.Length != 0) Gizmos.DrawLine(this.transform.position, connected_Node[i].transform.position);

    }
}
