using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//player_shot Photo_Identification 이랑 연동
public class player_RMfunction : MonoBehaviour
{
    public GameObject hand;
    public GameObject Identy_Beam_Effect_Obj;
    public GameObject[] Photo_Effect_Image_Obj;
    public bool HandOn = false;
    public bool Cnn_Server_Running = false;
    public bool Scanning_Hologram_Obj = false;

    public Object[] Photo_Effect_All_Image;
    public int[] Photo_Change_Delay_x01f;

    player_shot script_Shot;
    Animator animation_controll;
    float Cnn_Taken_Time;
    bool result_Get = false;
    string result_text;
    public SpriteRenderer[] Image_SpRend;
    public AudioClip Identify_Effect_Sound;

    AudioSource compoAudio;
    // Use this for initialization
    void Awake()
    {
        animation_controll = hand.GetComponent<Animator>();
        script_Shot = GetComponent<player_shot>();
        compoAudio = GetComponent<AudioSource>();
        Identify_Effect_Sound = GameObject.Find("Repository_Obj").GetComponent<Repository_sprite_material>().identify_Effect_Sound;
        Image_SpRend = new SpriteRenderer[Photo_Effect_Image_Obj.Length];
        for (int i = 0; i < Image_SpRend.Length; i++) Image_SpRend[i] = Photo_Effect_Image_Obj[i].GetComponent<SpriteRenderer>();

        Photo_Effect_All_Image = Resources.LoadAll("photo/Cnn_Photo_Effect", typeof(Sprite));
        if (Photo_Effect_Image_Obj.Length != Photo_Change_Delay_x01f.Length) Debug.Log("From player_RMfunction 이미지 오브젝트 배열과 딜레이 배열 크기 불일치! 수정할 것");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !GetComponent<player_shot>().grab_check)
        {
            animation_controll.SetInteger("state", 1);
            HandOn = true;
            Identy_Beam_Effect_Obj.SetActive(true);
            GetComponent<player_shot>().hand_check = true;
            StopAllCoroutines();
            for (int i = 0; i < Image_SpRend.Length; i++) Photo_Effect_Image_Obj[i].SetActive(false);
            StartCoroutine(Photo_Change_Effect2());
            StartCoroutine(Photo_Identify_Coroutine());
            
            //Debug.Log("Form player_RMfunction Mid mouse button held Down");
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            animation_controll.SetInteger("state", 2);
            HandOn = false;
            Identy_Beam_Effect_Obj.SetActive(false);
            GetComponent<player_shot>().hand_check = false;
            //Debug.Log("Form player_RMfunction Mid mouse button up");
        }

    }

    public void Get_Cnn_Result_Signal(string result, float time)
    {
        if(result == "None" && time == 0)
        {
            //Debug.Log("From player_RMfunction GetNone Signal");
        }
        else
        {
            Cnn_Taken_Time = time;
            result_text = result;
            result_Get = true;
        }
    }

    IEnumerator Photo_Identify_Coroutine()
    {
        float Identification_Start_Time = 0.0f;
        compoAudio.PlayOneShot(Identify_Effect_Sound);
        int t_Cycle = 0;
        while (HandOn)
        {
            if(t_Cycle >= 10)
            {
                t_Cycle = 0;
                compoAudio.Stop();
                compoAudio.PlayOneShot(Identify_Effect_Sound);
            }
            yield return new WaitForSeconds(0.5f); t_Cycle++;
            if (script_Shot.Cnn_Obj_Check && script_Shot.hit.transform.GetComponent<Data_Hologram>() && !script_Shot.hit.transform.GetComponent<Data_Hologram>().CNN_Identified && !result_Get)
            {
                script_Shot.hit.transform.GetComponent<Data_Hologram>().IgnoreBox.SetActive(false);
                script_Shot.hit.transform.GetComponent<Data_Hologram>().Get_Signal();

                yield return new WaitForSeconds(0.1f);
                if (!Scanning_Hologram_Obj)
                {
                    GetComponent<Photo_Identification>().Send_Photo_To_CNN_Server(script_Shot.hit.transform.gameObject);
                    Identification_Start_Time = Time.time; //시작 시간 수정할 것
                    Debug.Log("From player_RMfunction Sending Signal to Data Hologram");
                    Scanning_Hologram_Obj = true;
                }
                
                /*if (!Cnn_Server_Running)
                {
                    GetComponent<Photo_Identification>().Send_Photo_To_CNN_Server();
                    Identification_Start_Time = Time.time; //시작 시간 수정할 것
                }*/
            }
            else if (result_Get && script_Shot.Cnn_Obj_Check)
            {
                //time_Check = false;
                result_Get = false;
                Scanning_Hologram_Obj = false;

                if (Mathf.Abs(Time.time - Identification_Start_Time - Cnn_Taken_Time) <= 1.0f)
                {
                    script_Shot.hit.transform.GetComponent<Data_Hologram>().CNN_Identified = true;
                    if (result_text == script_Shot.hit.transform.GetComponent<Data_Hologram>().StringID)
                        script_Shot.hit.transform.GetComponent<Data_Hologram>().Is_Cnn_Correct = true;
                    Debug.Log("From player_RMfunction Cnn Taken Time : " + (Time.time - Identification_Start_Time) + ", Real Taken Time : " + Cnn_Taken_Time + ", Time Gap is : " + (Time.time - Identification_Start_Time - Cnn_Taken_Time));
                    StartCoroutine(Identfy_Result_Show_Delay());
                    for (int i = 0; i < Image_SpRend.Length; i++) Photo_Effect_Image_Obj[i].SetActive(false);
                }
                else Debug.Log("From player_RMfunction Don't Recognotion Cnn Taken Time : " + (Time.time - Identification_Start_Time) + ", Real Taken Time : " + Cnn_Taken_Time + ", Time Gap is : " + (Time.time - Identification_Start_Time - Cnn_Taken_Time));
            }
            else if (script_Shot.Cnn_Obj_Check && script_Shot.hit.transform.GetComponent<Data_Hologram>().CNN_Identified)
            {
                
                //if (GetComponent<player_shot>().module_Buffer == 0)
                GetComponent<player_shot>().Get_Hologram_Data(GetComponent<player_shot>().hit.transform.gameObject);
                //time_Check = false;
                Identification_Start_Time = 0.0f;
                Scanning_Hologram_Obj = false;
                for (int i = 0; i < Image_SpRend.Length; i++) Photo_Effect_Image_Obj[i].SetActive(false);
            }
            else
            {
                //time_Check = false;
                //Debug.Log("Cnn Taken Time : " + (Time.time - Identification_Start_Time));
                Identification_Start_Time = 0.0f;
                //Debug.Log("From player_RMfunction CNN Else Case");
                Scanning_Hologram_Obj = false;
                for (int i = 0; i < Image_SpRend.Length; i++) Photo_Effect_Image_Obj[i].SetActive(false);
            }
        }
        Scanning_Hologram_Obj = false;
        compoAudio.Stop();
    }

    IEnumerator Photo_Change_Effect2()
    {
        int cycle = 0;
        //bool time_Check = false;
        while (HandOn)
        {
            yield return new WaitForSeconds(0.1f); // 0.1 seconds loop
            if(Scanning_Hologram_Obj) for (int i = 0; i < Image_SpRend.Length; i++) Photo_Effect_Image_Obj[i].SetActive(true);
            else for (int i = 0; i < Image_SpRend.Length; i++) Photo_Effect_Image_Obj[i].SetActive(false);
            for (int i = 0; i < Image_SpRend.Length; i++)
            {
                if (cycle % Photo_Change_Delay_x01f[i] == 0)
                    Image_SpRend[i].sprite = Photo_Effect_All_Image[Mathf.RoundToInt(Random.Range(0.0f, Photo_Effect_All_Image.Length - 1))] as Sprite;
            }
        }
    }

    IEnumerator Identfy_Result_Show_Delay()
    {
        yield return new WaitForSeconds(0.3f);
        GetComponent<Show_Identify_Result>().Get_Result_Signal(GetComponent<Photo_Identification>().Identify_Result, GetComponent<Photo_Identification>().Highest_Percentage);
    }
}
