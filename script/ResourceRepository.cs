using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRepository : SingletonMonobehaviour<ResourceRepository> {

    public Material[] Materials;
    public Sprite[] BaseSprites;
    public Sprite[] MachineSprites;
    public Sprite[] TransferableDataSprite;
    public Sprite[] HologramSprite;
    public List<GameObject> Machine_Objs = new List<GameObject>();
    public List<int> Machine_id = new List<int>();
    public GameObject TOWER_CONTROLLED_BY_PLAYER;
    public AudioClip Hologram_Dissolve_Sound, identify_Effect_Sound, door_Sound;
    
    void Awake()
    {
        if (Materials.Length == 0) Debug.Log("From Repository_sprite_material 마테리얼 입력 안됨");
        if (BaseSprites.Length == 0) Debug.Log("From Repository_sprite_material 스프라이트 입력 안됨");
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
            if(TOWER_CONTROLLED_BY_PLAYER != null) TOWER_CONTROLLED_BY_PLAYER.GetComponent<MagneticMachine>().Get_Control_Signal(1);
            TOWER_CONTROLLED_BY_PLAYER = target;
        }
    }

    public void Get_Apply_Machine_Signal(GameObject obj)
    {
        if (obj.GetComponent<MagneticMachine>() != null)
        {
            Machine_Objs.Add(obj);
            
        }
        else Debug.Log("From Repository_sprite_material 머신 오브젝트가 입력되지 않음");
    }
}
