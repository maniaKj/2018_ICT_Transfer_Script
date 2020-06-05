using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 : Module_Lable, Puzzle_Hologram, Obj_Module_Operation, player_shot
public class Puzzle_Part_Hologram : MonoBehaviour {
    public int hologram_Part_ID;
    public GameObject[] hologram_Joint;
    public GameObject[] Connected_Hologram;
    public int[] part_CanBe_assembled;
    public int[] part_Assemble_answer; // 잘 안 쓸 듯
    public bool is_assembled = false;
    public GameObject module_Linked;
    public Vector3 original_Scale;
    public float scale_Muliply = 0.5f;

    public struct Assembling_Data
    {
        public Vector3 assemble_Position;
        public Quaternion assemble_Rotation;
        public int assemble_Part_ID;
        public GameObject linked_Module_Obj;
        public GameObject Data_Sender_Obj;

        public Assembling_Data(Vector3 assemble_Position, Quaternion assemble_Rotation, int assemble_Part_ID, GameObject linked_Module_Obj, GameObject Data_Sender_Obj)
        {
            this.assemble_Position = assemble_Position;
            this.assemble_Rotation = assemble_Rotation;
            this.assemble_Part_ID = assemble_Part_ID;
            this.linked_Module_Obj = linked_Module_Obj;
            this.Data_Sender_Obj = Data_Sender_Obj;
        }

        public void get_Data(Vector3 position, Quaternion rotation, int num, GameObject module, GameObject obj_this)
        {
            assemble_Position = position;
            assemble_Rotation = rotation;
            assemble_Part_ID = num;
            linked_Module_Obj = module;
            Data_Sender_Obj = obj_this;
        }
    }

    public Assembling_Data s_assemble;
    public Puzzle_Hologram script_PH;

    //private
    GameObject Base_Puzzle_Obj;
    Vector3 target_Position;
    Quaternion target_rotation;
    GameObject[] Related_assemble_Section;
    public bool obj_Bridged = false;

    void Reset()
    {
        hologram_Joint = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) if (transform.GetChild(i).tag == "Pivot") hologram_Joint[i] = transform.GetChild(i).gameObject;

        Base_Puzzle_Obj = transform.parent.parent.gameObject;
        if (Base_Puzzle_Obj.GetComponent<Puzzle_Hologram>() == null) Debug.Log("From Puzzle_Part_Hologram 베이스 오브젝트 설정 잘 안됌. 자식 부모 관계 확인 ㄱ " + this.gameObject);
        else script_PH = Base_Puzzle_Obj.GetComponent<Puzzle_Hologram>();
        Related_assemble_Section = Base_Puzzle_Obj.GetComponent<Puzzle_Hologram>().puzzle_Section_pivot;

        original_Scale = transform.localScale;
    }

    void Awake()
    {
        
        //if (GetComponent<BoxCollider>() != null) GetComponent<BoxCollider>().isTrigger = true; //박스 콜리더 여러개인 특이케이스 있다. 주의할 것
        //if (GetComponent<SphereCollider>() != null) GetComponent<SphereCollider>().isTrigger = true;
        //if (GetComponent<CapsuleCollider>() != null) GetComponent<CapsuleCollider>().isTrigger = true;
        hologram_Joint = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) if(transform.GetChild(i).tag == "Pivot") hologram_Joint[i] = transform.GetChild(i).gameObject;

        Base_Puzzle_Obj = transform.parent.parent.gameObject;
        if (Base_Puzzle_Obj.GetComponent<Puzzle_Hologram>() == null) Debug.Log("From Puzzle_Part_Hologram 베이스 오브젝트 설정 잘 안됌. 자식 부모 관계 확인 ㄱ " + this.gameObject);
        else script_PH = Base_Puzzle_Obj.GetComponent<Puzzle_Hologram>();
        Related_assemble_Section = Base_Puzzle_Obj.GetComponent<Puzzle_Hologram>().puzzle_Section_pivot;
        Connected_Hologram = new GameObject[transform.childCount];

        if (GetComponent<BoxCollider>() == null && GetComponent<SphereCollider>() == null && GetComponent<CapsuleCollider>() == null) Debug.Log("From Puzzle_Part_Hologram 오브젝트에 콜리더 적용 안됐음! " + this.gameObject);

        original_Scale = transform.localScale;
        
    }

    public int Get_Disassemble_Signal()
    {
        if (obj_Bridged) return 0; // 다리역할을 하는 오브젝트는 분해 불가능
        else {
            Disassemble();
            return hologram_Part_ID;
        }
    }

    public void Get_Module_Signal(int moduleID)
    {
        Send_assemble_Siganal(moduleID);
    }

    void Send_assemble_Siganal(int moduleID)
    {
        GameObject module_linked_target = GetComponent<Obj_Module_Operation>().child_Module[moduleID / 100 % 10];
        //module_Linked = module_linked_target;
        s_assemble.get_Data(hologram_Joint[moduleID / 100 % 10].transform.position, hologram_Joint[moduleID / 100 % 10].transform.rotation, moduleID % 100, module_linked_target, this.gameObject);
        obj_Bridged = Base_Puzzle_Obj.GetComponent<Puzzle_Hologram>().Get_Assemble_Signal(s_assemble);
    }

    void Disassemble()
    {
        if (module_Linked != null)
        {
            //module_Linked.GetComponent<TransferableDataObject>().Get_disassemble_Signal(0);
            module_Linked.SendMessageUpwards("Object_Unlinking", module_Linked.GetComponent<TransferableDataObject>().ID);
        }
        this.gameObject.transform.parent.gameObject.SetActive(false);
    }//분해할 때는 링크 모듈에 분해 메시지 보내고 오브젝트 액티브 꺼주면 ok

    public void Object_Linking(GameObject target, int Connect_Num)
    {
        Connected_Hologram[Connect_Num] = target;
    }//얘네 어떻게 더 보기 쉽게 만들 수 없나??

    public void Object_Unlinking(int Connect_Num)
    {
        Connected_Hologram[Connect_Num / 100 % 10] = null;

        bool temp = false;
        foreach(GameObject hologram in Connected_Hologram) if (hologram != null) temp = true;
        if (!temp) obj_Bridged = false;
    }
}
