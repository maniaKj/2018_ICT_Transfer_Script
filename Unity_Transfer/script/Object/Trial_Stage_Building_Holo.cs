using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Trial_Stage_Building_Holo : MonoBehaviour {

    public int Data_Connected_Num = 0;
    public bool Last_Direction_StartSignal = false;
    public PlayableDirector pd;
    [SerializeField] float BBoom_Sound_Delay = 6.0f;
    [SerializeField] AudioClip BBoom_Sound, JJing_Sound, Text_Sound;

    //hide
    AudioSource compo_Audio;

    private void Awake()
    {
        StartCoroutine(Data_Connenction_Check());
        compo_Audio = GetComponent<AudioSource>();
    }

    public void Get_Signal()
    {
        Data_Connected_Num++;
    }

    IEnumerator Data_Connenction_Check()
    {
        while (!Last_Direction_StartSignal)
        {
            if (Data_Connected_Num == 3) Last_Direction_StartSignal = true;
            yield return new WaitForSeconds(2.0f);
        }
        LastAction();
        yield return new WaitForSeconds(1f);
        compo_Audio.PlayOneShot(JJing_Sound);
        yield return new WaitForSeconds(BBoom_Sound_Delay);
        compo_Audio.PlayOneShot(BBoom_Sound);
        yield return new WaitForSeconds(15 - BBoom_Sound_Delay);
        compo_Audio.PlayOneShot(Text_Sound);
    }

    void LastAction()
    {
        Debug.Log("Final Action Started !!!");
        pd.Play();
        

    }
}
