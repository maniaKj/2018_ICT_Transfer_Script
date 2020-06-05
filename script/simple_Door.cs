using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//연계 Tower_test
public class simple_Door : MonoBehaviour {
    
    public bool temp_ = true;
    public bool Dissolving_Door = true;
    public int Alpha_updown = -1;
    public float alpha_Speed = 0.04f;
    public float alpha_Value = 0.0f;

    [Header("For Check")]public bool Door_OpenOrNot = false;

    //private
    MeshRenderer mr;
    BoxCollider bc;
    Transform TF;
    AudioSource compo_Audio;

	void Awake () {
        TF = this.transform;
        compo_Audio = GetComponent<AudioSource>();
        mr = GetComponent<MeshRenderer>();
        bc = GetComponent<BoxCollider>();
	}

    void ScriptOff()
    {
        this.enabled = false;
    }

    void Door_Switch(bool onoff)
    {
        if (onoff)
        {
            if(mr != null) mr.enabled = true;
            if(bc != null) bc.enabled = true;
        }
        else
        {
            if (mr != null) mr.enabled = false;
            if (bc != null) bc.enabled = false;
        }
    }

    void Obj_Func_Signal(int signal)
    {
        //Door_Switch(door_OnOff);
        //if (consistency_Num / 10 == module_signal_Num && consistency_Num % 10 == circuit_signal_Num) if (Moving_Door_ornot) StartCoroutine(Door_Down()); else Door_Switch(false);
        /*if (consistency_Num == circuit_signal_Num)
        {
            if (Moving_Door_ornot) StartCoroutine(Door_Down()); else Door_Switch(!temp_);

        }
        else if (!Moving_Door_ornot) Door_Switch(true);*/
        if (compo_Audio != null) compo_Audio.Play(); else Debug.Log("From simple_Door " + this.gameObject + " Doesn't Have AudioSource");
        if (temp_)
        {
            if (Dissolving_Door)
            {
                if (signal != 0) OnTransferToPlayer(); else Get_Appear_Signal();
            }
            else
            {
                if (signal != 0) Door_Switch(false); else Door_Switch(true);
            }
            
        }
        else
        {
            if (Dissolving_Door)
            {
                if (signal == 0) OnTransferToPlayer(); else Get_Appear_Signal();
            }
            else
            {
                if (signal == 0) Door_Switch(false); else Door_Switch(true);
            }
        }

    }

    public void Get_Module_Signal(int module_ID)
    {
       // if (GetComponent<Obj_Module_Operation>().module_Consistency) Obj_Func_Signal(module_ID);
    }

    public void Get_Circuit_Signal_ANOTHER(int signal_ID)
    {
        Obj_Func_Signal(signal_ID);
        //if (signal_ID == 10) Door_Hacked = true; else Door_Hacked = false;
        //if (controlled_By_Circuit) circuit_signal_Num = signal_ID;
        //Obj_Func_Signal();

    }

    public void OnTransferToPlayer()
    {
        Door_OpenOrNot = false;
        Alpha_updown = 1;
        alpha_Value = 0.0f;
        GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(Dissolve_Effect());
    }

    public void Get_Appear_Signal()
    {
        Door_OpenOrNot = true;
        Alpha_updown = -1;
        alpha_Value = 1.0f;
        mr.enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        StartCoroutine(Dissolve_Effect());
    }

    private IEnumerator Dissolve_Effect()
    {
        while (alpha_Value >= 0.0f && alpha_Value <= 1.0f)
        {
            alpha_Value += Alpha_updown * alpha_Speed;
            GetComponent<Renderer>().material.SetFloat("_AlphaCut", alpha_Value);
            yield return new WaitForSeconds(0.03f);
        }
        if(alpha_Value > 1.0f) mr.enabled = false;

    }
}