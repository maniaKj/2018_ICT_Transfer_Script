using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계  player_shot, Obj_Module_Operation
public class Module_Lable : MonoBehaviour {
    public int ModuleID;
    public int shader_Updown = 1;
    [Range(0.0f, 1.0f)] public float Shader_AlphaCut = 0.0f;
    public float Shader_alpha_Speed = 0.03f;
    //public int Module_Type;
    public bool module_Sprite_Static = false;
    public bool module_Color_Static = false;
    public bool Dont_activeoff_on_Awake = false;
    public bool Only_One_Operate = false;

    public bool Operative_Module = true;
    public int module_ID_restircted = 0;

    public AudioClip Effect_Sound;

    [HideInInspector]
    public Obj_Module_Operation MOP;
    public GameObject Connected_Func_machine;

    //private
    GameObject repository_Obj;
    Material[] material_Array;
    Sprite[] normal_sprite_Array;
    Sprite[] hologram_sprite_Array;
    Sprite[] Assemble_hologram_sprite_Array;
    Sprite[] Machine_sprite_Array;
    SpriteRenderer child_Sprite_rend;
    SpriteRenderer obj_Sprite_rend;
    BoxCollider obj_collider;
    AudioSource compo_Audio;


    void Reset()
    {
        repository_Obj = GameObject.Find("Repository_Obj");
        material_Array = repository_Obj.GetComponent<Repository_sprite_material>().I_HAVE_MATERIAL;
        normal_sprite_Array = repository_Obj.GetComponent<Repository_sprite_material>().I_HAVE_SPRITE;
        obj_Sprite_rend = GetComponent<SpriteRenderer>();
        obj_collider = GetComponent<BoxCollider>();
        child_Sprite_rend = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        Change_Module_Appearance(ModuleID);
    }

    void Awake()
    {
        repository_Obj = GameObject.Find("Repository_Obj");
        material_Array = repository_Obj.GetComponent<Repository_sprite_material>().I_HAVE_MATERIAL;
        normal_sprite_Array = repository_Obj.GetComponent<Repository_sprite_material>().I_HAVE_SPRITE;
        hologram_sprite_Array = repository_Obj.GetComponent<Repository_sprite_material>().Data_Hologram_Sprite;
        Assemble_hologram_sprite_Array = repository_Obj.GetComponent<Repository_sprite_material>().Hologram_Part_Sprite;
        Machine_sprite_Array = repository_Obj.GetComponent<Repository_sprite_material>().Machine_Sprite;
        obj_Sprite_rend = GetComponent<SpriteRenderer>();
        obj_collider = GetComponent<BoxCollider>();
        child_Sprite_rend = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        compo_Audio = this.gameObject.AddComponent<AudioSource>();

        Change_Module_Appearance(ModuleID);
        if (module_ID_restircted != 0 && module_ID_restircted < 99)
        {
            Change_Module_Appearance(module_ID_restircted);
            ModuleID = 0;
        }
        else Change_Module_Appearance(ModuleID);
        StartCoroutine(First_Signal());
        if (!Dont_activeoff_on_Awake) this.gameObject.SetActive(false);

        if (ModuleID >= 90 && ModuleID < 10000 && Connected_Func_machine == null) Debug.Log("From Module_Lable 머신 모듈인데 머신 연결 안되있음 수정해 : " + this.gameObject);
        else if (ModuleID >= 90 && ModuleID < 10000 && Connected_Func_machine != null)
        {
            if (Connected_Func_machine.GetComponent<Tower_test>().TowerFunc == Tower_test.TowerStyle.CNN && ModuleID != 90) Debug.Log("From Module_Lable 머신이랑 모듈ID 불일치 수정해 : " + this.gameObject);
            if (Connected_Func_machine.GetComponent<Tower_test>().TowerFunc == Tower_test.TowerStyle.Electric && ModuleID != 91) Debug.Log("From Module_Lable 머신이랑 모듈ID 불일치 수정해 : " + this.gameObject);
        }
    }

    public int Check_Module_Restrict(int gun_Buffer)
    {
        if (module_ID_restircted != 0)
        {
            if (module_ID_restircted == gun_Buffer) return 2;
            else if (gun_Buffer == 0 && ModuleID != 0 &&module_ID_restircted != 99) return 3;
            else return 0;
        }
        else
        {
            if ((ModuleID < 10000 && gun_Buffer <= 50) || (ModuleID >= 10000 && gun_Buffer >= 30) || (ModuleID < 10000 && gun_Buffer > 50)) return 1;
            else if (ModuleID < 10000 && gun_Buffer >= 90 && gun_Buffer <= 99) return 1;
            else if (gun_Buffer == 0) return 1;
            else return 0;
        }
    }

    public void GetSignal(int get_moduleID)
    {
        //Debug.Log("From Module_Lable Get_Signal");
        ModuleID = ModuleID - ModuleID % 100 + get_moduleID % 100;
        Change_Module_Appearance(ModuleID);
        if (MOP != null) MOP.Get_Child_Signal(ModuleID);
    } //신호받으면 외관, ID 바꾸고 부모 오브젝트로 신호
    public void Get_Machine_Signal(GameObject machine)
    {
        Connected_Func_machine = machine;
    }
    public void Get_disassemble_Signal(int get_moduleID)
    {
        ModuleID = ModuleID - ModuleID % 100 + get_moduleID;
        Change_Module_Appearance(ModuleID);
        this.gameObject.SetActive(true);
        Get_Enabling_Signal(true);
    }//홀로그램 퍼즐 분해할 때 Puzzle_Part_Hologram

    public void Get_Enabling_Signal(bool OnOff)
    {
        obj_collider.enabled = OnOff;
        obj_Sprite_rend.enabled = OnOff;
        child_Sprite_rend.enabled = OnOff;
    }

    public void Change_Module_Appearance(int moduleID)
    {
        if (!module_Sprite_Static)
        {
            if (moduleID % 100 < 30)
            {
                obj_Sprite_rend.sprite = normal_sprite_Array[moduleID % 100];
                child_Sprite_rend.sprite = normal_sprite_Array[moduleID % 100];
            }//General : Orange
            else if (moduleID % 100 < 50)
            {
                obj_Sprite_rend.sprite = hologram_sprite_Array[(moduleID % 100) % 30];
                child_Sprite_rend.sprite = hologram_sprite_Array[(moduleID % 100) % 30];
            }//Data Hologram : Blue
            else if (moduleID % 100 < 90)
            {
                obj_Sprite_rend.sprite = Assemble_hologram_sprite_Array[moduleID % 50];
                child_Sprite_rend.sprite = Assemble_hologram_sprite_Array[moduleID % 50];
            }//Assemble Hologram : Green
            else if (moduleID % 100 < 100)
            {
                obj_Sprite_rend.sprite = Machine_sprite_Array[(moduleID % 100) % 90];
                child_Sprite_rend.sprite = Machine_sprite_Array[(moduleID % 100) % 90];
            }//Special Machine : 
            StopAllCoroutines();
            StartCoroutine(Module_Shader_Effect());
        }//10000 이상은 홀로그램 파츠 id 함부로 수정 하지 말것
        /*else if (!module_Sprite_Static && moduleID >= 10000) {
            obj_Sprite_rend.sprite = Assemble_hologram_sprite_Array[moduleID % 50];
            child_Sprite_rend.sprite = Assemble_hologram_sprite_Array[moduleID % 50];
        }//홀로그램 파츠일 경우*/

        if (!module_Color_Static)
        {
            if (moduleID % 100 == 0)
            {
                obj_Sprite_rend.material = material_Array[10];
                child_Sprite_rend.material = material_Array[10];
            }
            else if (moduleID % 100 < 30)
            {
                obj_Sprite_rend.material = material_Array[7];
                child_Sprite_rend.material = material_Array[7];
            }
            else if (moduleID % 100 < 50)
            {
                obj_Sprite_rend.material = material_Array[9];
                child_Sprite_rend.material = material_Array[9];
            }
            else if (moduleID % 100 < 90)
            {
                obj_Sprite_rend.material = material_Array[8];
                child_Sprite_rend.material = material_Array[8];
            }
            else if (moduleID % 100 < 100)
            {
                obj_Sprite_rend.material = material_Array[12];
                child_Sprite_rend.material = material_Array[12];
            }

            if (module_ID_restircted != 0 && module_ID_restircted < 99 && ModuleID % 100 == 0)
            {
                obj_Sprite_rend.material = material_Array[11];
                child_Sprite_rend.material = material_Array[11];
            }
        }
    }
    public IEnumerator Module_Shader_Effect()
    {
        if (shader_Updown == 1) Shader_AlphaCut = 0.99f; else Shader_AlphaCut = 0.01f;
        compo_Audio.clip = Effect_Sound;
        //compo_Audio.PlayOneShot(Effect_Sound);

        while (Shader_AlphaCut <= 1.0f && Shader_AlphaCut >= 0.0)
        {
            if (shader_Updown == 1) Shader_AlphaCut -= Shader_alpha_Speed; else Shader_AlphaCut += Shader_alpha_Speed;
            obj_Sprite_rend.material.SetFloat("_AlphaCut", Shader_AlphaCut);
            child_Sprite_rend.material.SetFloat("_AlphaCut", Shader_AlphaCut);
            yield return new WaitForSeconds(0.03f);

        }
    }

    public void Get_Restricted_Module_Signal(int moduleID)
    {
        ModuleID = ModuleID - ModuleID % 100 + moduleID;
        Change_Module_Appearance(module_ID_restircted);

        StartCoroutine(Module_Shader_Effect());
        if (Only_One_Operate) module_ID_restircted = 99;//한번 바꾸면 그 다음부터는 수정 불가능 하도록
        if (MOP != null) MOP.Get_Child_Signal(ModuleID);
    }

    IEnumerator First_Signal()
    {
        yield return new WaitForSeconds(0.5f);
        if (MOP != null && ModuleID != 0 && ModuleID < 10000) MOP.Get_Child_Signal(ModuleID);
    }
}
