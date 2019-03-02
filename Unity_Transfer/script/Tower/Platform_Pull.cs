using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//해당 타워의 Tower_Test와 연동
//플랫폼의 InteractiveObj_MultiTag 연동
//플랫폼은 움직일 목적지를 지정할 오브젝트를 자식으로 가진다. 
//플랫폼이 위치 오브젝트 외의 다른 오브젝트를 자식으로 가질 수 있으므로 스크립트에서 마지막 자식 오브젝트는 제외하고 인식하지 않음

public class Platform_Pull : MonoBehaviour {
    public GameObject[] Target_Position_Obj;
    public float Speed_Platform = 15.0f;
    public int Current_Position = 0;
    public int Next_Position = 1;
    public float lerpSpeed = 10.0f;

    //private
    public Transform[] Target_position_Transform;
    public GameObject Target_Obj;
    public Transform platform_Transform;
    public bool criticalSection_Check = false;
    public float tower_detectRange;
    public Rigidbody rb;
    public int towerNum;
    public int[] route;
    public int[] pullDirection;
    public float freeDist = 1.0f;

    // Use this for initialization
    void Awake () {
        tower_detectRange = GetComponent<Tower_test>().Target_DetectRange + 1.0f;
        Target_position_Transform = new Transform[10];
    }
	
	// Update is called once per frame
	void Update () {
        if (criticalSection_Check) Pull();
	}

    void Pull()
    {
        float distance = Vector3.Distance(platform_Transform.position, Target_position_Transform[Next_Position].position);
        //나중에 시간 추가해서 일정 시간이 지나면 그냥 놔버리도록 수정(버그 대안)
        
        //각도 변하는 것도 추가
        if (distance > freeDist)
        {
            Vector3 Direction = (Target_position_Transform[Next_Position].position - platform_Transform.position).normalized;
            rb.MovePosition(platform_Transform.position + Direction * Speed_Platform * Time.deltaTime);
        }
        else if (distance > tower_detectRange) scriptOff();
        else
        {
            rb.velocity = Vector3.zero;
            Target_Obj.transform.GetChild(0).GetComponent<InteractiveObj_MultiTag>().platForm_shot = false;
            Current_Position = Next_Position;
            Target_Obj.transform.GetChild(0).GetComponent<InteractiveObj_MultiTag>().platform_criticalSection = false;
            scriptOff();
        }
    }

    void Destination_Decide()
    {
        //타워 두개가 겹치는 경우 예외 처리
        //이 부분 싹다 바꿔야 될 수도 있다
        int tower = 0, from = 0, to = 0, i;
        bool check = false;
        
        for (i = 0; i< pullDirection.Length; i++)
        {
            tower = pullDirection[i] / 100;
            from = (pullDirection[i] % 100) / 10;
            to = pullDirection[i] % 10;
            if (tower == towerNum && from == Current_Position) {
                check = true;
                break;
            }
            if (i == pullDirection.Length - 1 && check == false)
            {
                Debug.Log("해당되는 pull_route 없음");
                scriptOff();
            }
        }
        for(i = 0; i < route.Length; i++)
            if (from ==  route[i] / 10 && to == route[i] % 10) break;

        Debug.Log("tower : " + tower + "source : " + from + "desti : " + to);
        Next_Position = to;
    }

    public void GetSignal(GameObject receiver, int TowerID)
    {
        if (!criticalSection_Check)
        {
            Target_Obj = receiver;
            if (Target_Obj.transform.GetChild(0).GetComponent<InteractiveObj_MultiTag>().platform_criticalSection == false)
            {
                Target_Obj.transform.GetChild(0).GetComponent<InteractiveObj_MultiTag>().platform_criticalSection = true;
                platform_Transform = Target_Obj.transform.GetChild(0).transform;
                Debug.Log(Target_Obj.transform.childCount - 1 + "개 오브젝트가 위치 포지션으로 인식");
                for (int i = 1; i < Target_Obj.transform.childCount; i++)
                {
                    Target_position_Transform[i - 1] = Target_Obj.transform.GetChild(i);
                }
                rb = Target_Obj.transform.GetChild(0).GetComponent<Rigidbody>();
                towerNum = TowerID;
                route = Target_Obj.transform.GetChild(0).GetComponent<InteractiveObj_MultiTag>().Platform_route;
                pullDirection = Target_Obj.transform.GetChild(0).GetComponent<InteractiveObj_MultiTag>().Platform_pull_direction;
                Current_Position = Target_Obj.transform.GetChild(0).GetComponent<InteractiveObj_MultiTag>().currentPosition;
                Destination_Decide();
                criticalSection_Check = true;
            }
            else
            {
                scriptOff();
                Debug.Log("플랫폼이 이미 이동중입니다");
            }
        }
        else scriptOff();
    }

    void scriptOff()
    {
        criticalSection_Check = false;
        this.enabled = false;
    }
}
