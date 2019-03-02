using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 InteractiveObj_MultiTag, Module_Lable, player_shot, Tower_test
public class Repository_sprite_material : MonoBehaviour {//얘 이름 나중에 무조건 바꿀 것

    public Material[] I_HAVE_MATERIAL;
    public Sprite[] I_HAVE_SPRITE;
    public Sprite[] Machine_Sprite;
    public Sprite[] Data_Hologram_Sprite;
    public Sprite[] Hologram_Part_Sprite;
    public List<GameObject> Machine_Objs = new List<GameObject>();
    public List<int> Machine_id = new List<int>();
    public GameObject TOWER_CONTROLLED_BY_PLAYER;
    public AudioClip Hologram_Dissolve_Sound, identify_Effect_Sound, door_Sound;
    
    void Awake()
    {
        if (I_HAVE_MATERIAL.Length == 0) Debug.Log("From Repository_sprite_material 마테리얼 입력 안됨");
        if (I_HAVE_SPRITE.Length == 0) Debug.Log("From Repository_sprite_material 스프라이트 입력 안됨");
        //if (CNN_Classifier.Length == 0) Debug.Log("From Repository_sprite_material 인식기 입력 안됨");
    }
    
    public void Get_Tower_Control_Change_Signal(GameObject target)
    {
        if (TOWER_CONTROLLED_BY_PLAYER == target)
        {
            TOWER_CONTROLLED_BY_PLAYER = null;
        }
        else
        {
            if(TOWER_CONTROLLED_BY_PLAYER != null) TOWER_CONTROLLED_BY_PLAYER.GetComponent<Tower_test>().Get_Control_Signal(1);
            TOWER_CONTROLLED_BY_PLAYER = target;
        }
    }

    public void Get_Apply_Machine_Signal(GameObject obj)
    {
        if (obj.GetComponent<Tower_test>() != null)
        {
            Machine_Objs.Add(obj);
            
        }
        else Debug.Log("From Repository_sprite_material 머신 오브젝트가 입력되지 않음");
    }
}
