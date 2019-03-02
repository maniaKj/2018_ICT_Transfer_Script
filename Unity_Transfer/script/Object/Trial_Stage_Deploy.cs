using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trial_Stage_Deploy : MonoBehaviour {
    public GameObject Deploy_Object;
    public GameObject Hologram_Building;
    [Range (0.0f,1.0f)] public float material_Value = 0.0f;
    public float material_Speed = 0.03f;
    public List<GameObject> All_Deco_Obj = new List<GameObject>();

    void Awake()
    {
        Hologram_Building = GameObject.Find("Building_Hologram");
        Find_Renderer_Obj(Deploy_Object);
    }
	public void Get_Module_Signal(int module_ID)
    {
        if (GetComponent<Obj_Module_Operation>().module_Consistency)
        {
            Deploy_Object.SetActive(true);
            Hologram_Building.GetComponent<Trial_Stage_Building_Holo>().Get_Signal();
            StartCoroutine(Holo_Deco_Change());
        }
    }

    IEnumerator Holo_Deco_Change()
    {
        while(material_Value < 1.0f)
        {
            for (int i = 0; i < All_Deco_Obj.Count; i++) {
                All_Deco_Obj[i].GetComponent<Renderer>().material.SetFloat("_AlphaCut", 1 - material_Value);
                All_Deco_Obj[i].GetComponent<Renderer>().material.SetFloat("_Alpha", material_Value);
            }
            material_Value += material_Speed;
            yield return new WaitForSeconds(0.03f);
        }
    }

    private void Find_Renderer_Obj(GameObject target)
    {
        for (int i = 0; i < target.transform.childCount; i++)
        {
            if (target.transform.GetChild(i).GetComponent<Renderer>() != null) All_Deco_Obj.Add(target.transform.GetChild(i).gameObject);
            if (target.transform.GetChild(i).childCount != 0)
            {
                Find_Renderer_Obj(target.transform.GetChild(i).gameObject);
            }
        }
    }
}
