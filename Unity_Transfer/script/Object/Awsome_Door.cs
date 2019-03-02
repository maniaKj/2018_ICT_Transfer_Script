using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awsome_Door : MonoBehaviour {

    public List<GameObject> Upper_Pivot_Obj = new List<GameObject>();
    public List<GameObject> Bottom_Pivot_Obj = new List<GameObject>();

    public float door_Open_angle;
    public float Door_speed;
    public float next_door_delay;

    //public bool Door_Activation = false;
    public bool Test_Door_Signal = false;
    public bool Door_Opened = false;

    //private
    public bool Door_Activation = false;
    public bool door_Signal_Received = false;
    public int door_receive_Signal = 0;

    AudioSource compo_Audio;
    void Awake()
    {
        compo_Audio = GetComponent<AudioSource>();
        compo_Audio.clip = GameObject.Find("Repository_Obj").GetComponent<Repository_sprite_material>().door_Sound;

        for (int i = 0; i< this.gameObject.transform.childCount; i++)
        {
            if (this.gameObject.transform.GetChild(i).tag == "Pivot") Upper_Pivot_Obj.Add(this.gameObject.transform.GetChild(i).gameObject); 
        }
        for (int i = 0; i < Upper_Pivot_Obj.Count; i++)
        {
            for(int j = 0; j < Upper_Pivot_Obj[i].transform.childCount; j++)
            {
                if (Upper_Pivot_Obj[i].transform.GetChild(j).tag == "Pivot") Bottom_Pivot_Obj.Add(Upper_Pivot_Obj[i].transform.GetChild(j).gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (Test_Door_Signal)
        {
            Door_Opened = !Door_Opened;
            Door_Signal(Door_Opened);
        }
    }

    public void Door_Signal(bool onoff)
    {
        if(onoff) door_receive_Signal = 2; else door_receive_Signal = 1;
        if (!door_Signal_Received)
        {
            door_Signal_Received = true;
            StartCoroutine(Door_Signal_Received_Check());
        }
        Test_Door_Signal = false;
    }

    public void Get_Circuit_Signal_ANOTHER(int signal_ID)
    {
        if(signal_ID == 9) Door_Signal(true); else Door_Signal(false);
    }

    IEnumerator Door_Signal_Received_Check()
    {
        while(true)
        {
            //Debug.Log("DoorCheck");
            if (!Door_Activation)
            {
                if (Door_Opened && door_receive_Signal == 1) StartCoroutine(Send_Door_Signal(false));
                else if (!Door_Opened && door_receive_Signal == 2) StartCoroutine(Send_Door_Signal(true));
                else door_receive_Signal = 0;
                door_Signal_Received = false;
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator Door_OnOff(int door_part_Num, bool onoff)
    {
        //transform.localRotation = Quaternion.Euler(CurrentXRotation, 0, 0);
        //Door_Opened = !Door_Opened;
        float Rotation_X = Mathf.Clamp(-Upper_Pivot_Obj[door_part_Num].transform.localRotation.x * Mathf.Rad2Deg * 2.3f, 0, door_Open_angle);
        //Debug.Log("어디보자 : " + Mathf.Clamp(-Upper_Pivot_Obj[door_part_Num].transform.localRotation.x, 0, door_Open_angle) + " plus real : " + -Upper_Pivot_Obj[door_part_Num].transform.localRotation.x * Mathf.Rad2Deg * 2.3f);
        while (Rotation_X <= door_Open_angle && Rotation_X >= 0)
        {
            if(onoff) Rotation_X += Door_speed; else Rotation_X -= Door_speed;
            Upper_Pivot_Obj[door_part_Num].transform.localRotation = Quaternion.Euler(-Rotation_X, 0, 0);
            Bottom_Pivot_Obj[door_part_Num].transform.localRotation = Quaternion.Euler(Rotation_X * 2, 0, 0);
            yield return new WaitForSeconds(0.02f);
        }
        
        if (door_part_Num == Upper_Pivot_Obj.Count - 1)
        {
            compo_Audio.Stop();
            Door_Activation = false;
            Door_Opened = onoff;
            //door_Signal_Received = false;
        }
        
    }

    IEnumerator Send_Door_Signal(bool onoff)
    {
        Door_Activation = true;
        compo_Audio.Play();
        for (int i = 0; i < Upper_Pivot_Obj.Count; i++)
        {
            StartCoroutine(Door_OnOff(i,onoff));
            //Debug.Log("어디보자 : " + i);
            yield return new WaitForSeconds(next_door_delay);
        }
    }
}
