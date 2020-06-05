using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module_in_Gun : MonoBehaviour {
    public int ModuleID;
    [Range(0.0f, 1.0f)] public float Shader_AlphaCut = 0.0f;
    public float Shader_alpha_Speed = 0.03f;
    public AudioClip Dissolve_Effect_Sound;
    //public int Module_Type;

    //private
    GameObject repository_Obj;
    Material[] material_Array;
    Sprite[] normal_sprite_Array;
    Sprite[] hologram_sprite_Array;
    Sprite[] Assemble_hologram_sprite_Array;
    Sprite[] Machine_sprite_Array;
    SpriteRenderer child_Sprite_rend;
    SpriteRenderer obj_Sprite_rend;
    AudioSource compo_Audio;

    void Awake()
    {
        repository_Obj = GameObject.Find("Repository_Obj");
        material_Array = repository_Obj.GetComponent<ResourceRepository>().Materials;
        normal_sprite_Array = repository_Obj.GetComponent<ResourceRepository>().BaseSprites;
        hologram_sprite_Array = repository_Obj.GetComponent<ResourceRepository>().TransferableDataSprite;
        Assemble_hologram_sprite_Array = repository_Obj.GetComponent<ResourceRepository>().HologramSprite;
        Machine_sprite_Array = repository_Obj.GetComponent<ResourceRepository>().MachineSprites;
        obj_Sprite_rend = GetComponent<SpriteRenderer>();
        child_Sprite_rend = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
        compo_Audio = this.gameObject.AddComponent<AudioSource>();
        compo_Audio.clip = Dissolve_Effect_Sound;

        Change_Module_Appearance(ModuleID);
    }

    public void GetSignal(int get_moduleID)
    {
        bool onOff;
        if (get_moduleID == 0) onOff = false; else onOff = true;
        //Debug.Log("From Module_Lable Get_Signal");
        ModuleID = ModuleID - ModuleID % 100 + get_moduleID % 100;
        StopAllCoroutines();
        StartCoroutine(Module_Shader_Effect(onOff));
    } //신호받으면 외관, ID 바꾸고 부모 오브젝트로 신호

    public void Change_Module_Appearance(int moduleID)
    {
        if (moduleID % 100 == 0)
        {

        }
        else if (moduleID % 100 < 30)
        {
            obj_Sprite_rend.sprite = normal_sprite_Array[moduleID % 100];
            child_Sprite_rend.sprite = normal_sprite_Array[moduleID % 100];
            obj_Sprite_rend.material = material_Array[7];
            child_Sprite_rend.material = material_Array[7];
        }
        else if (moduleID % 100 < 50)
        {
            obj_Sprite_rend.sprite = hologram_sprite_Array[(moduleID % 100) % 30];
            child_Sprite_rend.sprite = hologram_sprite_Array[(moduleID % 100) % 30];
            obj_Sprite_rend.material = material_Array[9];
            child_Sprite_rend.material = material_Array[9];
        }
        else if (moduleID % 100 < 90)
        {
            obj_Sprite_rend.sprite = Assemble_hologram_sprite_Array[moduleID % 50];
            child_Sprite_rend.sprite = Assemble_hologram_sprite_Array[moduleID % 50];
            obj_Sprite_rend.material = material_Array[8];
            child_Sprite_rend.material = material_Array[8];
        }
        else if (moduleID % 100 < 100)
        {
            obj_Sprite_rend.sprite = Machine_sprite_Array[(moduleID % 100) % 90];
            child_Sprite_rend.sprite = Machine_sprite_Array[(moduleID % 100) % 90];
            obj_Sprite_rend.material = material_Array[12];
            child_Sprite_rend.material = material_Array[12];
        }
        
    }
    public IEnumerator Module_Shader_Effect(bool shader_Updown)
    {
        compo_Audio.PlayOneShot(Dissolve_Effect_Sound);
        if (shader_Updown)
        {
            Shader_AlphaCut = 0.99f;
            Change_Module_Appearance(ModuleID);
        }
        else Shader_AlphaCut = 0.01f;

        while (Shader_AlphaCut <= 1.0f && Shader_AlphaCut >= 0.0)
        {
            if (shader_Updown) Shader_AlphaCut -= Shader_alpha_Speed; else Shader_AlphaCut += Shader_alpha_Speed;
            obj_Sprite_rend.material.SetFloat("_AlphaCut", Shader_AlphaCut);
            child_Sprite_rend.material.SetFloat("_AlphaCut", Shader_AlphaCut);
            yield return new WaitForSeconds(0.03f);
        }
        Change_Module_Appearance(ModuleID);
    }
}
