using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 Puzzle_Node
//근석이 CNN 작업 완료하면 여기 스크립트와 연계
public class Puzzle_Hologram_2 : MonoBehaviour {

    //필수 변수
    public int puzzle_ID;
    public bool[] Node_Check_result = new bool[4];
    public bool[] Node_Check_Cleared = new bool[4];

    //private
    public GameObject[] puzzle_Node_Group1 = new GameObject[80];
    public GameObject[] puzzle_Node_Group2 = new GameObject[80];
    GameObject[] puzzle_Node_Group3 = new GameObject[80];
    GameObject[] puzzle_Node_Group4 = new GameObject[80];

    GameObject[] node_Group;
    int a = 0, b = 0, c = 0, d = 0;
    float node_Check_Delay = 2.0f;

    void Awake()
    {
        puzzle_Node_Group1 = new GameObject[80];
        puzzle_Node_Group2 = new GameObject[80];
        puzzle_Node_Group3 = new GameObject[80];
        puzzle_Node_Group4 = new GameObject[80];
        //자식 오브젝트 중 노드 오브젝트 전부 찾아서 분류
        Node_classifier(this.gameObject);
        Node_array_arrange();

        Node_Check_result = new bool[4];
        for (int i = 0; i < Node_Check_result.Length; i++) Node_Check_result[i] = false;

        StartCoroutine(Node_Check_Routine());
    }

    bool Check_Node_onOff(int group_Num)
    {
        bool temp_Check = true;

        switch (group_Num)
        {
            case 0:
                node_Group = puzzle_Node_Group1;
                break;
            case 1:
                node_Group = puzzle_Node_Group2;
                break;
            case 2:
                node_Group = puzzle_Node_Group3;
                break;
            case 3:
                node_Group = puzzle_Node_Group4;
                break;
            default:
                break;
        }

        foreach (GameObject node in node_Group)
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
            for (int i = 0; i < 4; i++) Node_Check_result[i] = Check_Node_onOff(i);
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
        while (i < a)
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
}
