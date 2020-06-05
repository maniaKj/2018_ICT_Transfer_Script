using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 : Puzzle_Part_Hologram, Obj_Module_Operation, Puzzle_Hologram
public class Puzzle_Hologram_repository : MonoBehaviour {
    public GameObject Hologram_Puzzle_Controller;
    public Transform pivot_Obj;
    public GameObject C_hologram_Part;

    //private
    public GameObject moduleObj;
    Puzzle_Hologram script_Puzzle_Hologram;

    void Reset() { Awake(); }

    void Awake()
    {
        if (C_hologram_Part == null || C_hologram_Part.tag == "Pivot") Debug.Log("From Puzzle_Hologram_repository 홀로그램 파트 적용 안됐음! pivot 오브젝트 말고 홀로그램 본체여야 함." + this.gameObject);
        Hologram_Puzzle_Controller = C_hologram_Part.transform.parent.parent.gameObject;
        script_Puzzle_Hologram = Hologram_Puzzle_Controller.GetComponent<Puzzle_Hologram>();
        moduleObj = GetComponent<Obj_Module_Operation>().child_Module[0];
        if (C_hologram_Part.GetComponent<Puzzle_Part_Hologram>() != null) C_hologram_Part.GetComponent<Puzzle_Part_Hologram>().module_Linked = moduleObj;
        else Debug.Log("From Puzzle_Hologram_repository 오브젝트 설정 제대로 안됨. 홀로그램 본체(피봇아님)을 집어 넣을 것 : " + this.gameObject);
        for (int i = 0; i < transform.childCount; i++) if (transform.GetChild(i).tag == "Pivot") pivot_Obj = transform.GetChild(i); //피봇 오브젝트 참조

        StartCoroutine(Wait_And_Awake());
    }

    public void Get_Module_Signal(int get_moduleID)
    {
        GameObject[] puzzle_Section = script_Puzzle_Hologram.puzzle_Section_pivot;
        int target_Num = 0;
        foreach(GameObject temp in puzzle_Section)
        {
            if (temp.transform.GetChild(0).GetComponent<Puzzle_Part_Hologram>().hologram_Part_ID == get_moduleID % 100)
            {
                script_Puzzle_Hologram.Hologram_positioning(target_Num, pivot_Obj.position, pivot_Obj.rotation, moduleObj);
                break;
            } 
            target_Num++;
        } //모듈값 홀로그램 위치시키기 
    }

    IEnumerator Wait_And_Awake()
    {
        yield return new WaitForSeconds(0.1f);
        Get_Module_Signal(C_hologram_Part.GetComponent<Puzzle_Part_Hologram>().hologram_Part_ID);
    }
}
