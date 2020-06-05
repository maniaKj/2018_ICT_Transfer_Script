using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 player_RMfunction, InteractiveObj_MultiTag, Module_Lable, Repository_sprite_material, bullet
public class player_shot : MonoBehaviour
{
    [Header("Variable For Adjusting")]
    public float fire_delay = 0.2f;
    public int module_Buffer = 0;
    public float Hologram_Check_Distance = 10.0f;

    [Space(15)]
    [Header("Object and File Applying")]
    public GameObject[] fire_bullet = new GameObject[4];
    public GameObject fire_Position;
    public GameObject placing_Tower;
    public GameObject[] Hologram_Marker = new GameObject[2];
    public GameObject gun_Shoot_Particle;
    [SerializeField] AudioClip gun_sound_1;
    [SerializeField] AudioClip gun_sound_2;

    //for Debuging
    [Space(15)]
    [Header("Debuging Check Variable")]
    public bool Test_DT_Identify = false;
    public LayerMask ray_Layermask;

    //Hide
    [HideInInspector] public bool grab_check = false;
    [HideInInspector] public bool hand_check = false;
    [HideInInspector] public bool Cnn_Obj_Check = false;
    [HideInInspector] public bool Machine_Circuit_Check = false;
    [HideInInspector] public float radius_aim = 3.0f;
    [HideInInspector] public MagneticMachine Connectec_Data_Obj;
    [HideInInspector] public GameObject aim_Mark_obj;
    [HideInInspector] public RaycastHit hit;

    bool Is_delay = false;
    [HideInInspector]public bool ray_Catched_Obj = false;
    float distance_aim = 50.0f;

    Vector3 aim_Position; // 디버그용
    Vector3 aimed_circuit_position;
    Quaternion target_Quat;

    GameObject module_Obj_inGun;
    GameObject aim_Checker;
    GameObject temp_Bullet;
    GameObject machine_Obj_locating_target;
    
    player_RMfunction pr;
    [HideInInspector]public TransferableDataObject previous_script_ML;
    Module_in_Gun Gun_Module_script;
    Aim_UI script_Aim_ui;
    AudioSource compo_Audio;

    void Awake()
    {
        //ray_Layermask = 1 << 2; ray_Layermask = 1 << 12; ray_Layermask = ~ray_Layermask;
        compo_Audio = GetComponent<AudioSource>();
        pr = GetComponent<player_RMfunction>();
        module_Obj_inGun = transform.GetChild(0).GetChild(0).GetChild(1).gameObject;
        Gun_Module_script = module_Obj_inGun.GetComponent<Module_in_Gun>();
        script_Aim_ui = GetComponent<Aim_UI>();
        Hologram_Marker[0] = GameObject.Find("Cam_Marker"); Hologram_Marker[1] = GameObject.Find("Pull_Machine_Marker");
        StartCoroutine(Aim_investigate());
        if (Hologram_Marker[0] == null || Hologram_Marker[1] == null) Debug.Log("From Player_Shot 홀로그램 마커 설정 안되있음 2개 설정할것!!");

        //aim_Checker = Instantiate(aim_Mark_obj, Vector3.zero, Quaternion.identity); //총 조준점 확인용
    }//변수 초기화

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Is_delay && !grab_check) StartCoroutine(Shot(0, 0));
        if (Input.GetMouseButtonDown(1) && !Is_delay && !grab_check) StartCoroutine(Shot(1, 0));
        //if (Input.GetKeyDown(KeyCode.Q) && !Is_delay && !grab_check) StartCoroutine(Shot(9, 9)); //나중에 지울수도
        if (Input.GetKeyDown(KeyCode.P) && !Is_delay && !grab_check) Module_buffer_return();
        //if (Input.GetKeyDown(KeyCode.V)) GetComponent<InteractiveObj_MultiTag>().Get_Signal2(Vector3.zero); //나중에 지울수도
    }//마우스 버튼 인식

    void FixedUpdate()
    {
        Ray_Check();
    }//매 고정프레임 만다 조준점 체크

    IEnumerator Shot(int mouse_Button, int Gun_state)
    {
        Is_delay = true;
        if (!hand_check) 
        {
            if (Gun_state == 0)
            {
                if (mouse_Button == 0)
                {
                    if(ray_Catched_Obj && module_Buffer < 90) module_InOut(hit.transform.gameObject);
                    else if(ray_Catched_Obj && module_Buffer >= 90) CNN_cam_place(hit.transform.gameObject);

                    gun_Shoot_Particle.GetComponent<ParticleSystem>().Play();
                    compo_Audio.PlayOneShot(gun_sound_1);
                }//왼쪽 마우스 클릭
                else if (mouse_Button == 1)
                {
                    temp_Bullet = Instantiate(fire_bullet[2], fire_Position.transform.position, target_Quat);
                    gun_Shoot_Particle.GetComponent<ParticleSystem>().Play();
                    compo_Audio.PlayOneShot(gun_sound_2);
                }// 오른쪽 마우스 클릭
                //Gun Sound Here
                //if(ray_Catched_Obj) Debug.Log("From player_shot Ray Collision Obj : " + hit.transform.gameObject);
            } // 총 번호 1
            else if (Gun_state == 9 && ray_Catched_Obj) Pick_tower_to_Control(hit.transform.gameObject);//Q키 누른 경우

            if (temp_Bullet != null) temp_Bullet.GetComponent<bullet>().Get_Signal(hit.point, hit.normal);
        }
        else if (Gun_state == 0 && mouse_Button == 1) GetComponent<InteractiveObj_MultiTag>().Get_Signal2(Vector3.zero);
        yield return new WaitForSeconds(fire_delay);
        Is_delay = false;
    }//총 쐈을 때

    IEnumerator Aim_investigate()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (ray_Catched_Obj && module_Buffer >= 90) Machine_Circuit_Check = Aim_point_Overlap(hit.point);
            else Machine_Circuit_Check = Aim_point_Overlap(Vector3.zero);
            //Debug.Log("From player_shot aim_check coroutine running");
        }

    }//일정 주기로 조준점 근처 회로 인식

    void Ray_Check()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out hit, 1000, ray_Layermask))
        {
            target_Quat = Quaternion.LookRotation((hit.point - fire_Position.transform.position).normalized);
            //Aim_point_Overlap(hit.point);
            ray_Catched_Obj = true;
            if (hit.transform.GetComponent<HologramObject>() != null && Vector3.Distance(hit.transform.position, this.transform.position) < Hologram_Check_Distance) Cnn_Obj_Check = true;
            else Cnn_Obj_Check = false;

            //aim_Checker.transform.position = hit.point;//총 조준점 확인용
            //if (Cnn_Obj_Check) Debug.Log("From player_shot CNN_Obj Check True : " + hit.transform.gameObject); else Debug.Log("From player_shot CNN_Obj Check False");
            //Debug.Log("From player_shot Ray HIt point : " + hit.point);
            //Debug.Log("From player_shot Ray HIt with : " + hit.transform.gameObject);
        }//Ray에 닿은 물체가 있을 때
        else
        {
            target_Quat = Quaternion.LookRotation((Camera.main.transform.position + Camera.main.transform.forward.normalized * distance_aim - fire_Position.transform.position).normalized); ;//Ray에 닿은 물체 없을 때
            ray_Catched_Obj = false;
            Cnn_Obj_Check = false;
        }
        script_Aim_ui.Get_Signal(ray_Catched_Obj, hit, module_Buffer, Machine_Circuit_Check);
    }//조준점의 오브젝트 인식 & 정보 파악

    bool Aim_point_Overlap(Vector3 aim_position)
    {
        if (aim_position != Vector3.zero)
        {
            Collider[] overlapped_Obj = Physics.OverlapSphere(aim_position, radius_aim);
            aim_Position = aim_position;//디버그용
            machine_Obj_locating_target = null;
            foreach (Collider obj in overlapped_Obj)
            {
                if ((obj.GetComponent<Circuit>() != null && obj.GetComponent<Circuit>().signal_Type != 0) || (obj.gameObject.GetComponent<InteractiveObj_MultiTag>() != null && obj.gameObject.GetComponent<InteractiveObj_MultiTag>().ObjType == InteractiveObj_MultiTag.AllType.Tower_Table))
                {
                    machine_Obj_locating_target = obj.gameObject;
                    if (obj.GetComponent<Circuit>() != null) aimed_circuit_position = obj.ClosestPoint(aim_position);
                    //else aimed_circuit_position = obj.transform.position;
                    aim_Mark_obj.transform.position = aimed_circuit_position;
                    aim_Mark_obj.transform.rotation = obj.transform.rotation;
                    //Debug.Log("From player_shot Find Circuit Obj : " + machine_Obj_locating_target.gameObject + aimed_circuit_position);
                    return true;
                }//조준점 근처 Radius 거리 안에 회로 찾기
            }
            if(machine_Obj_locating_target == null)
            {
                //Debug.Log("From player_shot Check No Circuit_Obj");
                aim_Mark_obj.transform.position = Vector3.zero;
                return false;
            }
        }
        else
        {
            //Debug.Log("From player_shot Check_ aim_Position -> Zero");
            machine_Obj_locating_target = null;
            aimed_circuit_position = Vector3.zero;
            return false;
        }
        return false;
    }//조준점 근처 회로 인식

    public void module_InOut(GameObject target)
    {
        TransferableDataObject ML = target.GetComponent<TransferableDataObject>();
        Puzzle_Part_Hologram PPH = target.GetComponent<Puzzle_Part_Hologram>();
        MagneticMachine magneticMachine = target.GetComponent<MagneticMachine>();
        HologramObject DT = target.GetComponent<HologramObject>();
        Machine_Effect ME = target.GetComponent<Machine_Effect>();


        if (ML != null)//모듈 조준
        {
            if (module_Buffer == 0 && ML.ID % 100 != 0 && ML.IsTransferPossible)
            {
                module_Buffer = ML.ID % 100;
                ML.OnDataTransfer(0);
                Gun_Module_script.GetSignal(module_Buffer);

                Connectec_Data_Obj = ML.ConnectedMachine; // 머신 전용
                ML.DetachConnectedMachine(); // 머신 전용
                if (module_Buffer == 90) aim_Mark_obj = Hologram_Marker[0]; else if (module_Buffer == 91) aim_Mark_obj = Hologram_Marker[1];
                previous_script_ML = ML;
            }
            else if (module_Buffer != 0 && ML.ID % 100 == 0 && ML.IsTransferPossible)
            {
                ML.OnMachineDataTransfer(Connectec_Data_Obj);
                ML.OnDataTransfer(module_Buffer);
                module_Buffer = 0;
                Connectec_Data_Obj = null;
                Gun_Module_script.GetSignal(module_Buffer);
                previous_script_ML = null;
            }
        }//모듈 조준
        else if (PPH != null)
        {
            if (module_Buffer == 0 && PPH.hologram_Part_ID != 0 && PPH.module_Linked.GetComponent<TransferableDataObject>().IsTransferPossible)
            {
                //Debug.Log("From 'player_shot' DisAssemble");
                module_Buffer = PPH.Get_Disassemble_Signal();
                previous_script_ML = PPH.module_Linked.GetComponent<TransferableDataObject>();

                Gun_Module_script.GetSignal(module_Buffer);
            }//홀로그램 분해
        }//퍼즐 홀로그램 조준
        else if (magneticMachine != null && ME != null)
        {
            if (module_Buffer == 0)
            {
                //Debug.Log("From player_shot Get Cnn_machine");
                if (magneticMachine.TowerFunc == MagneticMachine.TowerStyle.CNN)
                {
                    module_Buffer = 90;
                    aim_Mark_obj = Hologram_Marker[0];
                }
                else if (magneticMachine.TowerFunc == MagneticMachine.TowerStyle.Electric)
                {
                    module_Buffer = 91;
                    aim_Mark_obj = Hologram_Marker[1];
                }
                Connectec_Data_Obj = magneticMachine;
                target.SendMessage("OnTransferToPlayer");
                target.GetComponent<Machine_Effect>().Get_Signal(false);

                Gun_Module_script.GetSignal(module_Buffer);
                previous_script_ML = null;
            }
        }//인식기 조준
        else if (DT != null && Test_DT_Identify)
        {
            if (module_Buffer == 0)
            {
                //Destroy(target);
                module_Buffer = DT.ID;

                Connectec_Data_Obj = magneticMachine;
                target.SendMessage("OnTransferToPlayer");

                Gun_Module_script.GetSignal(module_Buffer);
            }
        }
    }//데이터 전송 & 흡수

    public void Get_Hologram_Data(GameObject target)
    {
        HologramObject DT = target.GetComponent<HologramObject>();
        if (DT != null)
        {
            target.SendMessage("OnTransferToPlayer");
            if (module_Buffer == 0)
            {
                //Destroy(target);
                module_Buffer = DT.ID;
                Connectec_Data_Obj = target.GetComponent<MagneticMachine>();

                Gun_Module_script.GetSignal(module_Buffer);
            }
            else
            {
                target.SendMessageUpwards("Get_Exception_Signal", target);
            }
        }
    }//홀로그램 인식시 데이터 흡수

    void CNN_cam_place(GameObject target)
    {
        TransferableDataObject Data = target.GetComponent<TransferableDataObject>();
        if (machine_Obj_locating_target != null)
        {
            Circuit script_C = machine_Obj_locating_target.GetComponent<Circuit>();
            InteractiveObj_MultiTag script_I = machine_Obj_locating_target.GetComponent<InteractiveObj_MultiTag>();
            if ((script_C != null && script_C.signal_Type != 0) || (script_I != null && script_I.ObjType == InteractiveObj_MultiTag.AllType.Tower_Table))
            {
                //Instantiate(placing_Tower, aimed_circuit_position, machine_Obj_locating_target.transform.rotation);
                Connectec_Data_Obj.transform.position = aimed_circuit_position;
                Connectec_Data_Obj.transform.rotation = machine_Obj_locating_target.transform.rotation;
                target.SetActive(true);
                //Connectec_Data_Obj.GetComponent<function_CNN>().Classifier_Placed(machine_Obj_locating_target);
                Connectec_Data_Obj.gameObject.SetActive(true);
                Connectec_Data_Obj.GetComponent<MagneticMachine>().Classifier_Placed(machine_Obj_locating_target);
                Connectec_Data_Obj.GetComponent<Machine_Effect>().Get_Signal(true);
                Connectec_Data_Obj = null;
                aim_Mark_obj.transform.position = Vector3.zero;
                module_Buffer = 0;
                Gun_Module_script.GetSignal(module_Buffer);
            }
        }//회로 위에 배치
        else if (Data != null)
        {
            if (Data.ID % 100 == 0 && Data.IsTransferPossible)
            {
                Data.OnDataTransfer(module_Buffer);
                Data.OnMachineDataTransfer(Connectec_Data_Obj);
                module_Buffer = 0;
                Gun_Module_script.GetSignal(module_Buffer);

                previous_script_ML = null;
                Connectec_Data_Obj = null;
            }
        }//모듈에 배치
    }//회로위에 장치 배치 신호

    public void Module_buffer_return()
    {
        if (module_Buffer == 0) Debug.Log("From player_shot 반환할 값이 없습니다");
        else if (module_Buffer <= 30)
        {
            previous_script_ML.OnDataTransfer(module_Buffer);
            module_Buffer = 0;
            Gun_Module_script.GetSignal(module_Buffer);
            previous_script_ML = null;
            Debug.Log("From player_shot 모듈 반환");
        }
        else if (module_Buffer <= 50)
        {
            if (previous_script_ML != null)
            {
                previous_script_ML.OnMachineDataTransfer(Connectec_Data_Obj.GetComponent<MagneticMachine>());
                previous_script_ML.OnDataTransfer(module_Buffer);
            }
            else Connectec_Data_Obj.gameObject.SetActive(true);

            Connectec_Data_Obj = null;

            module_Buffer = 0;
            Gun_Module_script.GetSignal(module_Buffer);
            previous_script_ML = null;
            Debug.Log("From player_shot 데이터 홀로그램 반환");
        }
        else if (module_Buffer < 90)
        {
            previous_script_ML.OnDataTransfer(module_Buffer);

            module_Buffer = 0;
            Gun_Module_script.GetSignal(module_Buffer);
            previous_script_ML = null;
            Debug.Log("From player_shot 조립 홀로그램 반환");
        }
        else if (module_Buffer <= 99)
        {
            if (previous_script_ML != null)
            {
                previous_script_ML.OnDataTransfer(module_Buffer);
                previous_script_ML.OnMachineDataTransfer(Connectec_Data_Obj.GetComponent<MagneticMachine>());
            }
            else
            {
                Connectec_Data_Obj.gameObject.SetActive(true);
                Connectec_Data_Obj.GetComponent<MagneticMachine>().Classifier_Placed(Connectec_Data_Obj.GetComponent<MagneticMachine>().circuit_Obj);
                Connectec_Data_Obj.GetComponent<Machine_Effect>().Get_Signal(true);
                aim_Mark_obj.transform.position = Vector3.zero;
            }

            Connectec_Data_Obj = null;

            module_Buffer = 0;
            Gun_Module_script.GetSignal(module_Buffer);
            previous_script_ML = null;
            Debug.Log("From player_shot 타워 반환");
        }
    }//데이터 복귀시키기

    void Pick_tower_to_Control(GameObject target)
    {
        //Debug.Log("From player_shot Tower Pick tried");
        InteractiveObj_MultiTag script_IM = target.GetComponent<InteractiveObj_MultiTag>();
        if (script_IM != null) script_IM.Get_Signal_tower_pick();
    }//안씀

    /*void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(aim_Position, radius_aim);
    }*/
}
