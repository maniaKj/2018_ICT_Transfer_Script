using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine_Effect : MonoBehaviour {

    public GameObject Real_Obj;
    public GameObject Fake_Effect_Obj;
    [Range(0.0f, 1.0f)] public float Dissolve_Shader_Value = 0.0f; 
    public float Dissolve_Speed;
    public int upDown = 1;

    List<Renderer> Fake_Obj_All_Rend = new List<Renderer>();


    private void Awake()
    {
        //Fake_Obj_All_Rend.Add(Fake_Effect_Obj.GetComponent<Renderer>());
        Find_All_Fake_Rend(Fake_Effect_Obj);
    }

    void Find_All_Fake_Rend(GameObject target)
    {
        for(int i = 0; i < target.transform.childCount; i++)
        {
            if (target.transform.GetChild(i).GetComponent<Renderer>() != null) Fake_Obj_All_Rend.Add(target.transform.GetChild(i).GetComponent<Renderer>());
            if (target.transform.childCount != 0) Find_All_Fake_Rend(target.transform.GetChild(i).gameObject);
        }
    }

    public void Get_Signal(bool Onoff)
    {
        if (!Onoff)
        {
            Real_Obj.SetActive(false);
            Fake_Effect_Obj.SetActive(true);
            Dissolve_Shader_Value = 0.0f;
            upDown = 1;
            //Debug.Log("Off");
        }
        else
        {
            Real_Obj.SetActive(false);
            Fake_Effect_Obj.SetActive(true);
            Dissolve_Shader_Value = 1.0f;
            upDown = -1;
            //Debug.Log("On");
        }
        StartCoroutine(Dissolve_Effect());
    }

    IEnumerator Dissolve_Effect()
    {
        while (Dissolve_Shader_Value <= 1.0f && Dissolve_Shader_Value >= 0.0f)
        {
            Dissolve_Shader_Value += upDown * Dissolve_Speed;
            for (int i = 0; i < Fake_Obj_All_Rend.Count; i++)
                for (int j = 0; j < Fake_Obj_All_Rend[i].materials.Length; j++) Fake_Obj_All_Rend[i].materials[j].SetFloat("_AlphaCut", Dissolve_Shader_Value);
            yield return new WaitForSeconds(0.03f);
        }
        Fake_Effect_Obj.SetActive(false);

        if (Dissolve_Shader_Value >= 1.0f) this.gameObject.SetActive(false);
        else if (Dissolve_Shader_Value <= 0.0f) Real_Obj.SetActive(true);
    }
}
