using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 bullet ,tower_test, player_shot,Obj_Module_Operation, Circuit_Node
//오브젝트가 두 타워의 범위 내에 겹쳐져 있을 경우>?
public class InteractiveObj_MultiTag : MonoBehaviour {
    public enum AllType { NormalCube, Monitor, Tower, CNN_image_classifier, Player, PlatForm, Door ,puzzle_Hologram, Button, Tower_Table};
    public AllType ObjType;
    public bool autoSetting = true;
    public string Obj_Name;

    //Condition Check Parameter
    public int circuit_Check = 0;
    public bool module_Check = false;
    public bool learning_Check = false;

    //아래 값들 int ID로 수정할것
    public bool Is_canbePulled = false;
    public bool Canbe_Grab = false;
    public bool Platform_Pull = false;

    //platform variable
    public int[] Platform_route;
    public int[] Platform_pull_direction;
    public bool platform_criticalSection = false;
    public int currentPosition = 0;
    public bool platForm_shot = false;

    //private
    ResourceRepository repository_script;
    float Tower_detect_range_MAXIMUM = 75.0f;
    //bool Is_Tower_Near = false;

    void Awake()
    {
        if (autoSetting)
            switch (ObjType)
            {
                case AllType.NormalCube:
                    Obj_Name = "Normal Cube";
                    Is_canbePulled = true;
                    Canbe_Grab = true;
                    break;
                case AllType.Monitor:
                    Obj_Name = "Monitor";
                    break;
                case AllType.Tower:
                    Obj_Name = "Tower";
                    Canbe_Grab = true;
                    break;
                case AllType.CNN_image_classifier:
                    Obj_Name = "CNN_Image_Classifier";
                    break;
                case AllType.Player:
                    Obj_Name = "Player";
                    Is_canbePulled = true;
                    break;
                case AllType.PlatForm:
                    Obj_Name = "Platform";
                    Platform_Pull = true;
                    break;
                case AllType.Door:
                    Obj_Name = "Door";
                    break;
                case AllType.puzzle_Hologram:
                    Obj_Name = "Hologram Section";
                    break;
                case AllType.Button:
                    Obj_Name = "Button";
                    break;
                default:
                    Obj_Name = "Null";
                    break;
            }
        repository_script = GameObject.Find("Repository_Obj").GetComponent<ResourceRepository>();
    }

    public void Get_Signal() //기능 수행, Circuit_Node에서 전력 수신 여부, Obj_Module_Operation에서 모듈 기능 사용 가능 여부 확인 + 총 맞은면 발생
    {
        //Debug.Log("From 'InteractiveObj_MultiTag' Bullet collision event");
        Obj_Module_Operation Module_script = GetComponent<Obj_Module_Operation>();
        Circuit_Node Circuit_script = GetComponent<Circuit_Node>();

        //SendMessage("Obj_Func_Signal", Module_script.module_Function_availability);
        if (Module_script == null && Circuit_script == null) SendMessage("Obj_Func_Signal",0);
        else if (Module_script == null && Circuit_script.signal_ID != 0) SendMessage("Obj_Func_Signal", Circuit_script.signal_ID);
        else if (Module_script.module_Consistency && Circuit_script == null) SendMessage("Obj_Func_Signal", 0);
        else if (Module_script.module_Consistency && Circuit_script.signal_ID != 0) SendMessage("Obj_Func_Signal", Circuit_script.signal_ID);
        else Debug.Log("From 'InteractiveObj_MultiTag' function can't be done");
    }

    public void Get_Signal2(Vector3 bullet_Hit_point)  //여기서는 tower 연동/기능 수행 + 총 맞으면 발생
    {
        //Case : Pull
        //Debug.Log("bullet Collision");
        Collider[] obj_Overlapped = Physics.OverlapSphere(transform.position, Tower_detect_range_MAXIMUM);//오브젝트 레이어 적극 활용할 것
        
        if (ObjType == AllType.puzzle_Hologram)
        {
            //Debug.Log("From InteractiveObj_MultiTag puzzle Holo Get Signal");
            if (repository_script.TOWER_CONTROLLED_BY_PLAYER != null)
                if (Get_Distance(repository_script.TOWER_CONTROLLED_BY_PLAYER) < Tower_detect_range_MAXIMUM)
                repository_script.TOWER_CONTROLLED_BY_PLAYER.GetComponent<MagneticMachine>().Get_Signal(this.gameObject, bullet_Hit_point);
                else Debug.Log("From InteractiveObj_MultiTag 오브젝트랑 타워의 거리가 너무 멀다");
            else Debug.Log("From InteractiveObj_MultiTag 컨트롤 타워 설정 안됬어요");
        } // 홀로그램 퍼즐 인식
        else if(ObjType == AllType.Button)
        {
            GetComponent<Button_Hologram_Deploy>().Get_Signal();
        }
        else if (ObjType != AllType.PlatForm) {
            if (repository_script.TOWER_CONTROLLED_BY_PLAYER != null)
                if (Get_Distance(repository_script.TOWER_CONTROLLED_BY_PLAYER) < Tower_detect_range_MAXIMUM)
                    repository_script.TOWER_CONTROLLED_BY_PLAYER.GetComponent<MagneticMachine>().Get_Signal(this.gameObject, bullet_Hit_point);
                else Debug.Log("From InteractiveObj_MultiTag 오브젝트랑 타워의 거리가 너무 멀다");
            else Debug.Log("From InteractiveObj_MultiTag 컨트롤 타워 설정 안됬어요");
        } // 잡아당기는 거 넣기

        //Case : Platform Pull
        if (ObjType == AllType.PlatForm)
        {
            platForm_shot = true;
            //GetComponent<Render>
            //플랫폼이 미리 타워에게 신호를 줘서 아래 코드좀 간결하게 하자
        }
        if (ObjType == AllType.Tower)
            foreach (Collider hit in obj_Overlapped)
            {
                if (hit.GetComponent<InteractiveObj_MultiTag>() != null && hit.GetComponent<InteractiveObj_MultiTag>().ObjType == AllType.PlatForm)
                    if (hit.GetComponent<InteractiveObj_MultiTag>().platForm_shot)
                    {
                        GetComponent<MagneticMachine>().Get_Signal(hit.gameObject, Vector3.zero);
                        break;
                    }
            }
            
        
    }

    public void Get_Signal_tower_pick()
    {
        if (ObjType == AllType.Tower || ObjType == AllType.CNN_image_classifier) GetComponent<MagneticMachine>().Get_Control_Signal(0);

    }//player_shot 에서 Q키 입력 확인 된 경우 실행

    float Get_Distance(GameObject target)
    {
        float Distance;
        Distance = Vector3.Distance(target.transform.position, this.gameObject.transform.position);
        return Distance;
    }
}
