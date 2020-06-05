using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagneticMachine : MonoBehaviour {
    public enum TowerStyle {Electric, CNN, Timer, Button_Sig1, Button_Sig2, Button_Sig3};
    [Header("Variable For Adjust")]
    public TowerStyle TowerFunc;
    public float Function_DelayTime = 2.0f;
    public float Target_DetectRange = 50.0f;
    public bool Influenced_By_Circuit = true;

    [Space(15)]
    [Header("Variable For Check")]
    [SerializeField]bool PowerOn = false;
    [SerializeField]bool delayOn = false;

    //private && Hide
    [HideInInspector]public GameObject circuit_Obj;
    [HideInInspector]public GameObject tower_Controll_Mark_Obj;
    GameObject Target_Obj;
    GameObject repository_Obj;
    Collider[] hitColliders;

    [HideInInspector] public bool Circuit_On = false;
    bool delayOn_detect = false;

    Vector3 bullet_contact_Point;
    Material[] material_Array;
    Material Original_Material;
    Renderer marker_Renderer;

    //maybe Useless
    [HideInInspector] public bool TOWER_CONTROLL = false;
    [HideInInspector] public int TowerID;

    void Awake()
    {
        repository_Obj = GameObject.Find("Repository_Obj");
        material_Array = repository_Obj.GetComponent<ResourceRepository>().Materials;
        if(tower_Controll_Mark_Obj == null) for (int i = 0; i < transform.childCount; i++) if (transform.GetChild(i).tag == "Marker") tower_Controll_Mark_Obj = transform.GetChild(i).gameObject;
        if (tower_Controll_Mark_Obj != null)
        {
            marker_Renderer = tower_Controll_Mark_Obj.GetComponent<Renderer>();
            Original_Material = marker_Renderer.material;
            marker_Renderer.material = material_Array[4];
        }
        else
        {
            marker_Renderer = GetComponent<Renderer>();
            Original_Material = marker_Renderer.material;
        }
        StartCoroutine(Cyclic_Result_Check());
        StartCoroutine(Delay_Bug_Handler());

        //디버그
        if (tower_Controll_Mark_Obj == null) Debug.Log("From Tower_test 마크 오브젝트 없어요 // " + this.gameObject);
        if (GetComponent<InteractiveObj_MultiTag>().ObjType != InteractiveObj_MultiTag.AllType.Tower) Debug.Log("From Tower_test 오브젝트가 타워로 설정되지 않았어요 " + this.gameObject);
    }

    //거리탐색 아예 제외해버려도 되겠다
    void FixedUpdate () {
        if (PowerOn && !delayOn)
        {
            switch (TowerFunc)
            {
                case TowerStyle.Electric:
                    Tower_Electric();////
                    break;
                case TowerStyle.CNN :
                    Tower_CNN_manage();////
                    break;
                case TowerStyle.Timer:
                    Function_Timer();////
                    break;
                case TowerStyle.Button_Sig1:
                    ButtonSignal(1);
                    break;
                case TowerStyle.Button_Sig2:
                    ButtonSignal(2);
                    break;
                case TowerStyle.Button_Sig3:
                    ButtonSignal(3);
                    break;
                default:
                    break;
            }
            StartCoroutine(Delay());
            PowerOn = false;
        }
        else if (PowerOn && delayOn)
        {
            //Debug.Log("delatTime => Power off");
            PowerOn = false;
        }
    }

    public void Get_Signal(GameObject send_obj, Vector3 hitPoint)
    {
        Debug.Log("From Tower_test Tower GetSignal Check : " + this.name);
        Target_Obj = send_obj;
        PowerOn = true;
        bullet_contact_Point = hitPoint;
        if (!delayOn && Target_DetectRange >= Vector3.Distance(transform.position, send_obj.transform.position))
        {
            
            //Debug.Log("From Tower_test Tower (" + TowerID + ") GetSignal : " + send_obj + " " + Target_Obj);
        }
    }
    public void Get_Control_Signal(int signalNum)
    {
        ResourceRepository script_Repository = repository_Obj.GetComponent<ResourceRepository>();
        if(signalNum == 0)
        {
            TOWER_CONTROLL = !TOWER_CONTROLL;
            script_Repository.Get_Tower_Control_Change_Signal(this.gameObject);
        }
        else TOWER_CONTROLL = false;

        if (TOWER_CONTROLL) marker_Renderer.material = material_Array[4];
        else marker_Renderer.material = material_Array[0];
    }
    
    //All Fucntion Parts///
    void Tower_CNN_manage()
    {
        //Debug.Log("From Tower_test Tower CNN Function implement");
        GetComponent<function_CNN>().GetSignal(bullet_contact_Point, Target_Obj);
    }

    void Tower_Electric()
    {
        if (Target_Obj.GetComponent<InteractiveObj_MultiTag>() != null && Target_Obj.GetComponent<InteractiveObj_MultiTag>().Is_canbePulled && Circuit_On)
        {
            /*Vector3 direction = transform.position - Target_Obj.transform.position;
            float distance = Vector3.Distance(transform.position, Target_Obj.transform.position);
            Target_Obj.GetComponent<Rigidbody>().AddForce(direction * distance * PullPower);
            Target_Obj.GetComponent<Rigidbody>().AddForce(Target_Obj.transform.up * PullPower_vertical);*/
            GetComponent<Pull>().enabled = true;
            GetComponent<Pull>().GetSignal(Target_Obj);
        }
        else if (Target_Obj.GetComponent<InteractiveObj_MultiTag>() != null && Target_Obj.GetComponent<InteractiveObj_MultiTag>().Platform_Pull)
        {
            //신호 오브젝트의 부모 전송할것
            GetComponent<Platform_Pull>().enabled = true;
            GetComponent<Platform_Pull>().GetSignal(Target_Obj.transform.parent.gameObject, TowerID);
            Debug.Log("Tower (" + TowerID + ") Platform Pull");
        }
        /*else if(Target_Obj.GetComponent<InteractiveObj_MultiTag>() != null && Target_Obj.GetComponent<InteractiveObj_MultiTag>().Receive_electric)
        {
            //Target_Obj.SendMessage("Obj_Func_Signal");
        }*/
    }

    void Function_Timer()
    {
        GetComponent<Timer>().Get_Signal();
    }

    void ButtonSignal(int input)
    {
        SendMessageUpwards("Get_Button_Signal", input);
    }
    //All Fucntion Parts///

    public void Classifier_Placed(GameObject connected_Obj)
    {
        Circuit script_C = connected_Obj.GetComponent<Circuit>();
        //InteractiveObj_MultiTag script_I = connected_Obj.GetComponent<InteractiveObj_MultiTag>();
        if (script_C != null) circuit_Obj = connected_Obj;
        else circuit_Obj = null;
        Circuit_onoff_check();
        if (GetComponent<function_CNN>() != null) GetComponent<function_CNN>().Get_Placed_Signal();
        if (GetComponent<Pull>() != null) GetComponent<Pull>().Get_Rotate_Signal();
        StartCoroutine(Cyclic_Result_Check());
        StartCoroutine(Delay_Bug_Handler());

    } //인식기 배치할 때 변수 적용
    void Circuit_onoff_check()
    {
        if (circuit_Obj != null)
        {
            if (circuit_Obj.GetComponent<Circuit>().signal_Type != 0) Circuit_On = true;
            else Circuit_On = false;
        }
        else Circuit_On = false;
    } //회로 신호를 받는지 확인

    void ChangeAppearance(bool onoff)
    {
        if (onoff)
        {
            if (Circuit_On && Influenced_By_Circuit) marker_Renderer.material = material_Array[4];
            else if (!Circuit_On && Influenced_By_Circuit) marker_Renderer.material = Original_Material;
            else if (!Influenced_By_Circuit) marker_Renderer.material = material_Array[4];
        }
        else
        {
            marker_Renderer.material = Original_Material;
        }
    }
    IEnumerator Cyclic_Result_Check()
    {
        while (true)
        {
            //Debug.Log("From function_CNN Corouting Running" + this.gameObject + Circuit_On);
            Circuit_onoff_check();
            ChangeAppearance(!delayOn);
            yield return new WaitForSeconds(1.0f);
            //Debug.Log("From Tower_test Cyclic Check");
        }
    } //주기적으로 회로 신호 체크
    
    IEnumerator Delay()
    {
        delayOn = true;
        ChangeAppearance(false);
        yield return new WaitForSeconds(Function_DelayTime);
        delayOn = false;
        ChangeAppearance(true);
    }
    IEnumerator Delay_Bug_Handler()
    {
        while (true)
        {
            if (delayOn)
            {
                yield return new WaitForSeconds(Function_DelayTime + 0.5f);
                //marker_Renderer.material = material_Array[4];
                ChangeAppearance(true);
                delayOn = false;
            }
            else yield return new WaitForSeconds(Function_DelayTime - 1.0f);
        }
    }
}
