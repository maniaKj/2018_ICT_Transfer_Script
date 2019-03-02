using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class photo_Identify : MonoBehaviour {
    public enum Animal {cat, dog};
    public Animal CatOrDog;
    public int Identify_level = 1;
    public GameObject Obj_Text;
    public float rangeDetectTime = 1.0f;
    string text_ID;
    public bool overLap_Check = false;
    bool check_delay = false;

    void Start()
    {
        if (CatOrDog == Animal.cat) text_ID = "cat";
        else text_ID = "dog";
    }

    void Update()
    {
        if(!check_delay)
        StartCoroutine(Overlap_Check());
    }

    public void Identification(int LearnLevel)
    {
        Debug.Log("ing");
        if (Identify_level <= LearnLevel) TextOn(true);
        else TextOn(false);
    }

    public void TextOn(bool isCorrect)
    {
        if (isCorrect) Obj_Text.GetComponent<TextMesh>().text = text_ID;
        else Obj_Text.GetComponent<TextMesh>().text = "can't understand";
        Obj_Text.gameObject.SetActive(true);
    }

    IEnumerator Overlap_Check()
    {
        check_delay = true;
        yield return new WaitForSeconds(1.0f);
        if(!overLap_Check) Obj_Text.gameObject.SetActive(false);
        overLap_Check = false;
        check_delay = false;
    }
}
