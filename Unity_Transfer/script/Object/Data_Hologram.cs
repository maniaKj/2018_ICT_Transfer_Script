using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data_Hologram : MonoBehaviour {
    public enum HologramType { Dron, Car, DNA, Key, Cat, Dog, Falcon, Dolphin };
    public HologramType Obj_hologram_Type;
    public int moduleID;
    public string StringID;
    public bool CNN_Identified = false;
    public bool CNN_Identifiying = false;
    public bool Is_Cnn_Correct = false;
    public GameObject IgnoreBox;
    public AudioClip Dissolve_Effect_Sound;

    int Alpha_updown = -1;
    [Range(0.0f, 1.0f)] public float alpha_Value = 1.0f;
    public float alpha_Speed = 0.05f;

    public List<Renderer> All_Child_Rend = new List<Renderer>();
    int testVar = 0;
    AudioSource compo_Audio;

    IEnumerator cnn_Coroutine;

    public void Awake()
    {
        cnn_Coroutine = CNN_Running();
        switch (Obj_hologram_Type)
        {
            case HologramType.Dron:
                StringID = "dron";
                break;
            case HologramType.Car:
                StringID = "car";
                break;
            case HologramType.DNA:
                StringID = "dna";
                break;
            case HologramType.Key:
                StringID = "key";
                break;
            case HologramType.Cat:
                StringID = "cat";
                break;
            case HologramType.Dog:
                StringID = "dog";
                break;
            case HologramType.Falcon:
                StringID = "falcon";
                break;
            case HologramType.Dolphin:
                StringID = "dolphin";
                break;
        }
        Find_All_Child_rend(this.gameObject);
        IgnoreBox.SetActive(true);
        Dissolve_Effect_Sound = GameObject.Find("Repository_Obj").GetComponent<Repository_sprite_material>().Hologram_Dissolve_Sound;
        compo_Audio = this.gameObject.AddComponent<AudioSource>();
    }
    void Find_All_Child_rend(GameObject target){
        for(int i = 0;i < target.transform.childCount; i++)
        {
            if (target.transform.GetChild(i).GetComponent<Renderer>() != null) All_Child_Rend.Add(target.transform.GetChild(i).GetComponent<Renderer>());
            Find_All_Child_rend(target.transform.GetChild(i).gameObject);
        }
    }

    public void Get_Consume_Signal()
    {
        SendMessageUpwards("Get_Module_Signal", testVar);
        //this.gameObject.SetActive(false);
        Alpha_updown = 1;
        alpha_Value = 0.0f;
        GetComponent<BoxCollider>().enabled = false;
        IgnoreBox.SetActive(true);
        StartCoroutine(Dissolve_Effect());
    }

    public void Get_Appear_Signal()
    {
        Alpha_updown = -1;
        alpha_Value = 1.0f;
        for (int i = 0; i < All_Child_Rend.Count; i++) All_Child_Rend[i].enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        IgnoreBox.SetActive(true);
        StartCoroutine(Dissolve_Effect());
    }

    private IEnumerator Dissolve_Effect()
    {
        compo_Audio.PlayOneShot(Dissolve_Effect_Sound);
        while (alpha_Value >= 0.0f && alpha_Value <= 1.0f)
        {
            alpha_Value += Alpha_updown * alpha_Speed;
            for (int i = 0; i < All_Child_Rend.Count; i++)
                for(int j = 0; j < All_Child_Rend[i].materials.Length;j++) All_Child_Rend[i].materials[j].SetFloat("_AlphaCut", alpha_Value);
            yield return new WaitForSeconds(0.03f);
        }
        if (alpha_Value > 1.0f) for (int i = 0; i < All_Child_Rend.Count; i++) All_Child_Rend[i].enabled = false;
        
    }

    public void Get_Signal()
    {
        IgnoreBox.SetActive(false);
        StopCoroutine(cnn_Coroutine);
        //Debug.Log("From Puzzle_Node Coroutine Stopped");
        cnn_Coroutine = CNN_Running();
        StartCoroutine(cnn_Coroutine);
    }

    public IEnumerator CNN_Running()
    {
        CNN_Identifiying = true;
        yield return new WaitForSeconds(1.5f);
        CNN_Identifiying = false;
        IgnoreBox.SetActive(true);

    }
}
