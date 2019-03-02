using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Print_Node_Filter : MonoBehaviour {
    [ExecuteInEditMode]
    //필터 노드(부모 오브젝트 자식 말구)에 적용한 뒤, 프리팹을 씬화면에서 좌표 0,0,0으로 맞춘다 그 다음 러닝 모드에서 스크립트 적요
    public bool write_trigger = false;

	void FixedUpdate () {
        if (write_trigger) All_Node();
	}

    void All_Node()
    {
        write_trigger = false;
        FileStream fsa = File.Create("Filter_Node_Value.txt");
        fsa.Close();

        StreamWriter sw = new StreamWriter("Filter_Node_Value.txt");
        string write_Line;

        for (int i = 0; i < transform.childCount; i++)
        {
            write_Line = " " + Mathf.Round(this.transform.GetChild(i).position.x * 100)/100 + " " + Mathf.Round(this.transform.GetChild(i).position.y * 100) / 100 + " " + Mathf.Round(this.transform.GetChild(i).position.z * 100) / 100 + " " + this.transform.GetChild(i).GetComponent<Puzzle_Node>().Node_ID + "  : x,y,z 좌표 + 타입 값";
            sw.WriteLine(write_Line);
        }
        sw.Close();
    }
}
