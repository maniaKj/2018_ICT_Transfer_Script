using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 Tower_test, Puzzle_Node, Puzzle_Hologram, Line_Connection
public class function_CNN : MonoBehaviour {
    public int Classifier_ID; //동일한 id가진 오브젝트가 같은 씬안에 있으면 안된다. 이거 나중에 디버깅 코드 짜둘 것
    GameObject cam_Head;
    public GameObject Connected_Monitor;

    float cam_Check_Distance;
    public float cam_Check_Angle = 50.0f;
    public int cam_Check_node_capacity = 30;

    //private
    MagneticMachine script_T;
    public Vector3 contact_point_Received;
    Vector3 cam_head_position;
    Quaternion Quaternion_cam_will_Look;
    public GameObject[] node_in_Distance;
    public Puzzle_Hologram script_PH;
    public Puzzle_Hologram_2 script_PH2;

    void Awake()
    {
        script_T = GetComponent<MagneticMachine>();
        cam_Check_Distance = script_T.Target_DetectRange;
        cam_Head = transform.GetChild(0).gameObject;
        node_in_Distance = new GameObject[cam_Check_node_capacity];
        StartCoroutine(Cyclic_Result_Check());
        if (Connected_Monitor == null) Debug.Log("From function_CNN 연결된 모니터 없어요 : " + this.gameObject);
    }

    public void GetSignal(Vector3 contact_Point ,GameObject target_Obj)
    {
        //Debug.Log("From function_CNN Get Signal");
        contact_point_Received = contact_Point;
        if (target_Obj.GetComponent<Puzzle_Part_Hologram>() != null)
        {
            script_PH = target_Obj.GetComponent<Puzzle_Part_Hologram>().script_PH;
            script_PH2 = null;
        }
        else if (target_Obj.GetComponent<Puzzle_Hologram_2>() != null)
        {
            script_PH2 = target_Obj.GetComponent<Puzzle_Hologram_2>();
            script_PH = null;
        }
        cam_head_position = cam_Head.transform.position;
        Quaternion_cam_will_Look = Quaternion.LookRotation((contact_Point - cam_head_position).normalized);
        Cam_Look();

        //Debug.Log("From function_CNN Get Cam head position : " + cam_head_position);
        //Debug.Log("From function_CNN Get Cam head global position : " + transform.TransformPoint(cam_head_position));
    } // 플레이어의 전기탄이 홀로그램에 부딪혀서 그 정보가 이곳으로

    void Cam_Look()
    {
        cam_Head.transform.rotation = Quaternion_cam_will_Look;
    } //인식기 각도 변경

    void Get_target_hologram_Info()
    {
        RaycastHit hit;
        Ray ray = new Ray(cam_Head.transform.position, cam_Head.transform.forward);
        if (Physics.Raycast(ray, out hit, cam_Check_Distance))
            if (hit.transform.GetComponent<Puzzle_Part_Hologram>() != null)
            {
                script_PH = hit.transform.GetComponent<Puzzle_Part_Hologram>().script_PH;
                script_PH2 = null;
            }
            else if (hit.transform.GetComponent<Puzzle_Hologram_2>() != null)
            {
                script_PH2 = hit.transform.GetComponent<Puzzle_Hologram_2>();
                script_PH = null;
            }
            else
            {
                script_PH = null;
                script_PH2 = null;
            }
        else
        {
            script_PH = null;
            script_PH2 = null;
        }

            //Debug.Log("From function_CNN get signal Check : " + cam_Head + "   " + cam_Check_Angle + "   " + cam_Check_Distance);
            int i = 0;
        Collider[] overlapped_Obj = Physics.OverlapSphere(transform.position, cam_Check_Distance);  // 나중에 여기에 레이어 마스크 먹이자
        foreach(Collider node in overlapped_Obj)
        {
            if(node.GetComponent<Puzzle_Node>() != null)
            {
                if (node.GetComponent<Puzzle_Node>().Get_Signal(cam_Head, cam_Check_Angle, cam_Check_Distance))
                {
                    node_in_Distance[i++] = node.gameObject;
                    //Debug.Log("From function_CNN nodeCheck True node");
                }
               //else Debug.Log("From function_CNN nodeCheck false node");
            }
        }
    } //일정 거리 &각도 안에 노드에게 인식 정보 전달 및 노드 정보 받음

    IEnumerator Cyclic_Result_Check()
    {
        while (true)
        {
            //Debug.Log("From function_CNN Corouting Running" + this.gameObject );
            yield return new WaitForSeconds(1.0f);
            if (script_T.Circuit_On) Get_target_hologram_Info(); //주기적으로 노드에게 인식 정보 전달
            else
            {
                script_PH = null;
                script_PH2 = null;
            }

            int i = 0, j;
            if (script_PH != null)
            {
                for (j = 0; j < 4; j++)
                    if (script_PH.Node_Check_result[j])
                    {
                        if (!script_PH.Node_Check_Cleared[j])
                        {
                            Send_order_to_Hologram(i++);
                            script_PH.Node_Check_Cleared[j] = true;
                        }
                        GetComponent<Line_Connection>().ScriptOnOff(true);
                    }
            }
            else if (script_PH2 != null)
            {
                for (j = 0; j < 4; j++)
                    if (script_PH2.Node_Check_result[j])
                    {
                        if (!script_PH2.Node_Check_Cleared[j])
                        {
                            Send_order_to_Hologram(i++);
                            script_PH2.Node_Check_Cleared[j] = true;
                        }
                        GetComponent<Line_Connection>().ScriptOnOff(true);
                    }
            }

            /*if (script_PH != null) foreach (bool result in script_PH.Node_Check_result) {
                if (result) Send_order_to_Hologram(i);
                i++;
            }
            else if(script_PH2 != null) foreach (bool result in script_PH2.Node_Check_result)
            {
                if (result) Send_order_to_Hologram(i);
                i++;
            }*/
            else GetComponent<Line_Connection>().ScriptOnOff(false);
            //Debug.Log("From function_CNN coroutine Check");
        }
    } // 주기적으로 노드 인식 정보 전달 및 체크

    void Send_order_to_Hologram(int num)
    {
        Debug.Log("From function_CNN result Sending");
        Connected_Monitor.GetComponent<Monitor>().Get_Signal(num);
        
    } //모니터에게 노드 인식 정보 전달

    public void OnTransferToPlayer()
    {
        GetComponent<Line_Connection>().ScriptOnOff(false);
    }
    public void Get_Placed_Signal()
    {
        //StopAllCoroutines();
        StartCoroutine(Cyclic_Result_Check());
    }

    /*void OnDrawGizmos()
    {
        Vector3 tmp = contact_point_Received - cam_head_position;
        float tmp_dist = cam_Check_Distance * Mathf.Tan(cam_Check_Angle * 3.14f / 180.0f);
        
        Gizmos.DrawLine(cam_head_position, contact_point_Received);
        Gizmos.DrawWireSphere(cam_head_position, cam_Check_Distance);
        Gizmos.DrawWireSphere(contact_point_Received, tmp_dist);
    }*/
}
