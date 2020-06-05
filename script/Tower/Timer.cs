using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {
    [Header("Variable For Adjust")]
    [Range(0, 20)] public int Timer_Time;
    public int Start_Time_Signal, End_Time_Signal;

    [Header("Variable For Check")]
    [SerializeField] int Remain_Time;
    public bool Timer_Running = false;

    [Header("Apply Obj & Variable")]
    [SerializeField] TextMesh Timer_Text;
    [SerializeField] Sprite Chronometer_Sprite;
    [SerializeField] Sprite Timer_Runngin_Sprite;

    AudioSource compo_Audio;

    private void Awake()
    {
        compo_Audio = GetComponent<AudioSource>();
    }

    IEnumerator Timer_Start()
    {
        Timer_Running = true;
        SendMessage("Get_Circuit_Signal", Start_Time_Signal);
        GetComponent<SpriteRenderer>().sprite = Timer_Runngin_Sprite;
        Remain_Time = Timer_Time;
        while (Remain_Time != 0) {
            compo_Audio.Play();
            Timer_Text.text = Remain_Time + "";
            yield return new WaitForSeconds(1.0f);
            Remain_Time--;
        }
        SendMessage("Get_Circuit_Signal", End_Time_Signal);
        Timer_Text.text = "";
        GetComponent<SpriteRenderer>().sprite = Chronometer_Sprite;
        Timer_Running = false;
    }

    public void Get_Signal()
    {
        StopAllCoroutines();
        StartCoroutine(Timer_Start());
    }
}
