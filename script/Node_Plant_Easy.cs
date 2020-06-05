using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Node_Plant_Easy : MonoBehaviour {
    public GameObject[] Node_Filter;
    public GameObject Target_Filter;
    public Transform Target_Position;
    public GameObject target_Hologram;
    public bool State_Change_node_plant = false;
    public float normal_Dis = 3.0f;
    public int current_Filter_Num = 0;
    

    //Control
    public int put_Filter_Num = 10;
    public bool Filter_change = false;
    public bool put_Node = false;
    public bool print_All_Node = false;

    public List<GameObject> All_Node_cluster = new List<GameObject>();
    Quaternion local_roation;

    void Awake()
    {
        local_roation = Target_Filter.transform.rotation;
    }

    void FixedUpdate()
    {
        if (put_Filter_Num != 10 && Filter_change)
        {
            Node_FIlter_Change();
            Filter_change = false;
        }
        if (put_Node || Input.GetKeyDown(KeyCode.K)) Node_Plant();
        if (print_All_Node) Printf_Node_Value();
    }

    void Node_FIlter_Change()
    {
        Quaternion Temp = Target_Filter.transform.rotation;
        Destroy(Target_Filter);
        GameObject temp = Instantiate(Node_Filter[put_Filter_Num], Target_Position.position, Temp, Target_Position);
        Target_Filter = temp;
        current_Filter_Num = put_Filter_Num;
    }

    void Node_Plant()
    {
        Vector3[] target_node_position = new Vector3[Target_Filter.transform.childCount];
        Vector3[] target_node_Normal = new Vector3[Target_Filter.transform.childCount];
        bool Every_node_ray_catch = true;
        RaycastHit hit;
        for (int i = 0; i < Target_Filter.transform.childCount; i++)
        {
            Ray ray = new Ray(Target_Filter.transform.GetChild(i).transform.position, transform.forward);
            if(Physics.Raycast(ray, out hit)) {
                target_node_position[i] = hit.point + hit.normal * normal_Dis;
                target_node_Normal[i] = hit.normal;
            }
            else
            {
                Every_node_ray_catch = false;
                break;
            }
        }
        
        if (Every_node_ray_catch)
        {
            GameObject clone_Node = Instantiate(Node_Filter[current_Filter_Num], Vector3.zero, Quaternion.identity);
            for (int i = 0; i < Target_Filter.transform.childCount; i++)
            {
                clone_Node.transform.GetChild(i).position = target_node_position[i];
                clone_Node.transform.GetChild(i).rotation = Quaternion.LookRotation(target_node_Normal[i]);
            }
        }

    }

    void Printf_Node_Value()
    {
        print_All_Node = false;
        All_Node_cluster = new List<GameObject>();
        Find_All_Node(target_Hologram);
        FileStream fsa = File.Create("Test_Node_Value.txt");
        fsa.Close();

        StreamWriter sw = new StreamWriter("Test_Node_Value.txt");
        string write_Line;
        int i = 1;
        foreach(GameObject obj in All_Node_cluster)
        {
            write_Line = " " + Mathf.Round(obj.transform.position.x * 100) / 100 + " " + Mathf.Round(obj.transform.position.y * 100) / 100 + " " + Mathf.Round(obj.transform.position.z * 100) / 100 + " " + obj.GetComponent<Puzzle_Node>().Node_ID + "   : " + i++ + "번째 노드 좌표 값 + 타입 값";
            sw.WriteLine(write_Line);
        }
        sw.Close();
    }

    void Find_All_Node(GameObject obj)
    {
        for(int i = 0; i < obj.transform.childCount; i++)
        {
            if (obj.transform.GetChild(i).GetComponent<Puzzle_Node>() != null) All_Node_cluster.Add(obj.transform.GetChild(i).gameObject);
            else Find_All_Node(obj.transform.GetChild(i).gameObject);
        }
    }
}
