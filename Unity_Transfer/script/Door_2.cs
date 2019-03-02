using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_2 : MonoBehaviour {

    public GameObject Connected_GameObj;
    Puzzle_Hologram script_P;
    Puzzle_Hologram_2 script_P2;

    void Awake()
    {
        if (Connected_GameObj.GetComponent<Puzzle_Hologram>() != null) script_P = Connected_GameObj.GetComponent<Puzzle_Hologram>();
        if (Connected_GameObj.GetComponent<Puzzle_Hologram_2>() != null) script_P2 = Connected_GameObj.GetComponent<Puzzle_Hologram_2>();
        if (script_P == null && script_P2 == null) Debug.Log("From Door_2 변수 오류 " + this.gameObject);
        StartCoroutine(Cyclic_Check());
    }

    IEnumerator Cyclic_Check()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f);
            if (script_P.Assemble_Complete)
            {
                this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                this.gameObject.GetComponent<BoxCollider>().enabled = false;

            }
            else
            {
                this.gameObject.GetComponent<MeshRenderer>().enabled = true;
                this.gameObject.GetComponent<BoxCollider>().enabled = true;
            }
        }
    }

}
