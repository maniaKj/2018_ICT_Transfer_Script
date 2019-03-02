using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hacking_Node : MonoBehaviour {

    public GameObject[] Circuit_Obj_willbeChange;
    public GameObject Shield_Node_Obj;

    void Awake()
    {
        StartCoroutine(Sending_Hacking_Signal());
    }

    public void Change_Obj_Parent()
    {
        foreach (GameObject obj in Circuit_Obj_willbeChange) obj.transform.parent = Shield_Node_Obj.transform;
        Shield_Node_Obj.GetComponent<Circuit_Node>().Awake();
    }

    IEnumerator Sending_Hacking_Signal()
    {
        yield return new WaitForSeconds(1.0f);
        GetComponent<Circuit_Node>().Get_Highpass_Signal(4);
        yield return new WaitForSeconds(1.0f);
        Change_Obj_Parent();
    }
}
