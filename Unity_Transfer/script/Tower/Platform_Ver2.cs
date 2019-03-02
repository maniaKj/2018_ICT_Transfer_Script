using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Ver2 : MonoBehaviour {

    [Header("Variable For Adjusting")]
    public float Move_Speed;
    [SerializeField] float sound_Volume_Down_Speed = 0.05f;


    [Space(15)]
    [Header("Object and File Applying")]
    public Transform Start_Transform;
    public Transform Destination_Transform;
    [SerializeField] Renderer platform_Rail;
    [SerializeField] AudioClip Engine_Start_Sound;

    //for Debuging
    [Space(15)]
    [Header("Debuging Check Variable")]
    [SerializeField] Transform moveFrom;
    [SerializeField] Vector3 moveDirection;
    [SerializeField] int moveForward = -1;
    [SerializeField] float From_To_Distance;
    [SerializeField] float Check_Distance;
    [SerializeField] bool Moving_onOff = false;

    //Hide
    Material Origianl_Mat;
    Material Null_Mat;
    AudioSource compo_Audio;

    void Awake()
    {
        compo_Audio = GetComponent<AudioSource>();

        moveFrom = Start_Transform;
        moveDirection = (Destination_Transform.position - Start_Transform.position).normalized;
        From_To_Distance = Vector3.Distance(Destination_Transform.position, Start_Transform.position);
        Origianl_Mat = platform_Rail.material;
        Null_Mat = GameObject.Find("Repository_Obj").GetComponent<Repository_sprite_material>().I_HAVE_MATERIAL[2];

        StartCoroutine(Check_And_Turn_Path());
        if (Moving_onOff) Get_Circuit_Signal_ANOTHER(3); else Get_Circuit_Signal_ANOTHER(0);
        
    }
    private void FixedUpdate()
    {
        if(Moving_onOff) transform.Translate(moveDirection * Move_Speed * moveForward * Time.fixedDeltaTime);
    }

   /* public void Get_Signal(bool Onoff)
    {
        Moving_onOff = Onoff;
        if (Onoff) platform_Rail.material = Origianl_Mat; else platform_Rail.material = Null_Mat;
        //StopAllCoroutines();
        //StartCoroutine(Check_And_Turn_Path());
    }*/

    public void Get_Circuit_Signal_ANOTHER(int signal)
    {
        if (signal == 0)
        {
            Moving_onOff = false;
            StartCoroutine(Sound_Off());
            platform_Rail.material = Null_Mat;
        }
        else
        {
            Moving_onOff = true;
            StartCoroutine(Sound_Start());
            compo_Audio.volume = 1.0f;
            platform_Rail.material = Origianl_Mat;
        }

    }

    IEnumerator Check_And_Turn_Path()
    {
        while (true)
        {
            Check_Distance = Vector3.Distance(transform.position, moveFrom.position);
            if (Check_Distance > From_To_Distance)
            {
                moveForward *= -1;
                if (moveFrom == Start_Transform) moveFrom = Destination_Transform; else moveFrom = Start_Transform;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator Sound_Off()
    {
        while(compo_Audio.volume > 0.05f)
        {
            compo_Audio.volume -= sound_Volume_Down_Speed;
            yield return new WaitForSeconds(0.05f);
        }
        compo_Audio.Stop();
        compo_Audio.volume = 1.0f;
    }

    IEnumerator Sound_Start()
    {
        compo_Audio.PlayOneShot(Engine_Start_Sound);
        yield return new WaitForSeconds(0.8f);
        compo_Audio.Play();
    }

}
