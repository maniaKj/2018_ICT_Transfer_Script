using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 function_CNN
//노드는 콜리더 IS trigger 상태로 필요함
public class Puzzle_Node : MonoBehaviour
{
    public int Node_ID;
    public bool node_OnOFF = false;
    public GameObject Hologram_control_Obj;

    //Coroutine
    public float corouting_Delay_Time = 0.5f;

    //private
    public GameObject cam_Head;
    public float angle_Limit;
    public float distance_Limit;
    public float obj_dist;
    public float obj_angle;
    public Material[] node_Mat = new Material[2];
    IEnumerator cycle_coroutine;
    Renderer rend;

    void Awake()
    {
        node_Mat[0] = Resources.Load("material/Emission_Material/Yellow_Normal") as Material;
        node_Mat[1] = Resources.Load("material/Emission_Material/Yellow_Emission") as Material;
        rend = GetComponent<Renderer>();
        corouting_Delay_Time = 3.5f;
        cycle_coroutine = Cyclic_Check();
    }

    public bool Get_Signal(GameObject cam_Head_Obj, float cam_Angle_limit, float cam_DIstance_limit)
    {
        Get_Tower_Variable(cam_Head_Obj, cam_Angle_limit, cam_DIstance_limit);
        node_OnOFF = Check_Angle_and_Block();
        
        if (node_OnOFF)
        {
            Change_Appearance(true);
            StopCoroutine(cycle_coroutine);
            //Debug.Log("From Puzzle_Node Coroutine Stopped");
            cycle_coroutine = Cyclic_Check();
            StartCoroutine(cycle_coroutine);
            return true;
        }
        return false;

        
    }

    public void Change_Appearance(bool onOff)
    {
        //Behaviour halo = (Behaviour)GetComponent("Halo");
        //halo.enabled = onOff;
        if (onOff) rend.material = node_Mat[1];
        else rend.material = node_Mat[0];
    }

    bool Check_Angle_and_Block()
    {
        if (cam_Head != null && angle_Limit > 0)
        {
            //Debug.Log("실행함");
            Vector3 tmp_Dist = transform.position - cam_Head.transform.position;
            float distance = Vector3.Distance(transform.position, cam_Head.transform.position);
            float angle = Vector3.Angle(tmp_Dist, cam_Head.transform.forward);
            RaycastHit hit;

            obj_dist = distance;
            obj_angle = angle;

            if (Physics.Linecast(transform.position, cam_Head.transform.position, out hit) && hit.transform.gameObject == cam_Head.transform.parent.gameObject)// 중간에 장애물 있는지 확인 여기좀 위험하다
                if (angle < angle_Limit && distance < distance_Limit) return true;
        }
        //else Debug.Log("실행안함");

        return false;
    } //거리체크도 필요합니다. 나중에 수정

    void Get_Tower_Variable(GameObject cam_head_Obj, float angle_value, float dist_value)
    {
        cam_Head = cam_head_Obj;
        angle_Limit = angle_value;
        distance_Limit = dist_value;
    }

    IEnumerator Cyclic_Check()
    {
        //Debug.Log("From Puzzle_Node Coroutine Started");
        yield return new WaitForSeconds(1.2f);
        //Debug.Log("From Puzzle_Node Coroutine Ended and Node Signal false");
        Change_Appearance(false);
        Get_Tower_Variable(null, 0, 0);
        node_OnOFF = false;
    }//주기적으로 노드가 인식되고 있는지 체크

    void OnDrawGizmos()
    {
        if (cam_Head != null)
            Gizmos.DrawRay(transform.position, -(transform.position - cam_Head.transform.position));
    }
}
