using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 function_CNN, Obj_Module_Deploy, Circuit_Node
public class Monitor : MonoBehaviour {
    public GameObject[] Connected_Classifier;
    public GameObject[] Child_Obj_Array;
    //public GameObject Tower_Table;
    public GameObject[] Output_Module; //결과 모듈 숫자는 홀로그램 노드 종류 숫자와 동일해야 함.
    public GameObject antenna_Obj;
    public bool Deploy_Specific_obj_array = false;

    //Deep Learning
    public int Learning_Order = 0;
    public float Learning_Value = 0;
    public float Learning_Speed = 60.0f;
    public TextMesh Learning_Text;
    //public float Learning_

    //public GameObject function_Module;  //모듈 조합 하기

    //private
    Obj_Module_Deploy script_Deploy;

    void Awake()
    {
        script_Deploy = GetComponent<Obj_Module_Deploy>();
        foreach(GameObject obj in Connected_Classifier) obj.GetComponent<function_CNN>().Connected_Monitor = this.gameObject;
        if(Child_Obj_Array.Length == 0) Debug.Log("From Monitor 스크립트 다시 확인할 것 " + this.gameObject);
        if (script_Deploy != null) script_Deploy.child_Deploy_Objects = Child_Obj_Array;
        if (script_Deploy != null && Deploy_Specific_obj_array) script_Deploy.Deploy_specified_Obj(0);

        for (int i = 0; i < transform.childCount; i++) if (transform.GetChild(i).tag == "Pivot") antenna_Obj = transform.GetChild(i).gameObject;
        if (antenna_Obj == null) Debug.Log("From Monitor 안테나 오브젝트 설정 안됨 자식으로 배치해 둘것 tag : pivot으로 해놓고.  : " + this.gameObject);
    }

    public void Get_Signal(int num)
    {
        Debug.Log("From Monitor Signal Received " + this.gameObject);
        if (num == Learning_Order && Learning_Value == 0) StartCoroutine(Deep_Learning(0));
        //if (script_Deploy != null) script_Deploy.Deploy_specified_Obj(2);
    }//신호 받으면 결과 모듈 출력할 수 있도록

    public void Get_Module_Signal(int module_ID)
    {
        if (GetComponent<Obj_Module_Operation>().module_Consistency)
        {
            StartCoroutine(Deep_Learning(0));
            //GetComponent<Obj_Module_Deploy>().Deploy_specified_Obj(2);
        }
        
    }

    IEnumerator Deep_Learning(int num)
    {
        Debug.Log("From Monitor Deep Learning Start");
        while (Learning_Value < 100)
        {
            Learning_Value += Mathf.Round(Learning_Speed * 0.01f);
            Learning_Text.text = Learning_Value + "%";
            yield return new WaitForSeconds(0.1f);
        }
        Learning_Text.text = "100%";
        Learning_Value = 0;
        Learning_Order++;

        Output_Module[num].SetActive(true);
    }
}
