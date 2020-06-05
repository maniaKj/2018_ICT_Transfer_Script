using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Drawing : MonoBehaviour {
    [Header("url Check")]
    public string new_filepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    public string TruePath = "file://c://Users//김진표//Documents//Drawing_Capture.jpg";
    public string file_path;


    [Header("Variable for Adjust")]
    [SerializeField] private float simplifyTolerance = 0.02f;
    [SerializeField] private bool False_Identification_On = false;

    [Header("Photo Identify Result")]
    public string Identify_Result;
    public float Highest_Percentage = 0.00f;
    [SerializeField] float Dron, DNA, Car, None, Cat, Dog, Falcon, Key, Dolphin;

    [Header("Variable for Check")]
    public string Result_Text;
    public float Taken_Time;
    public int Line_Obj_Num = -1;
    public List<GameObject> All_Line_Obj = new List<GameObject>();
    [SerializeField] bool is_Identifying = false;

    [Header("Apply Obj & Variable")]
    [SerializeField] Camera Cam_For_Capture;
    public GameObject Line_Rend_Obj, Loading_Effect_Obj;
    [SerializeField] AudioClip button_Sound, result_On_Sound;

    //Hide
    string Server_Address = "http://35.229.255.144/unity_object/upload.php";
    string[] String_Analysis;
    string[] String_Seperator = { "QQQ", "QQQQ" };
    [SerializeField] player_shot script_pShot;
    LineRenderer line;
    bool Drawing_Start = false;
    AudioSource compo_Audio;

    private void Start()
    {
        //line = GetComponent<LineRenderer>();
        GameObject player_Check = GameObject.FindGameObjectWithTag("player");
        compo_Audio = GetComponent<AudioSource>();
        Loading_Effect_Obj.SetActive(false);
        script_pShot = player_Check.GetComponent<player_shot>();

        string tmp_char = "" + (char)92;
        string tmp = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        file_path = "file://" + tmp.Replace(tmp_char, "//") + "//Drawing_Capture.jpg";
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && script_pShot.hit.transform != null && script_pShot.hit.transform.gameObject == this.gameObject && !is_Identifying)
        {
            //Line_Obj_Num++;
            All_Line_Obj.Add(Instantiate(Line_Rend_Obj));
            line = All_Line_Obj[++Line_Obj_Num].GetComponent<LineRenderer>();

            line.positionCount = 1;
            line.SetPosition(0, script_pShot.hit.point);

            compo_Audio.Play();
            Drawing_Start = true;
            //line.SetPosition(line.positionCount - 1, GameObject.Find("Player").transform.position);
        }

        if (Drawing_Start && Input.GetMouseButtonUp(0))
        {
            line.Simplify(simplifyTolerance);
            compo_Audio.Stop();
            Drawing_Start = false;
        }

        
    }
    private void FixedUpdate()
    {
        if (Drawing_Start && script_pShot.hit.transform != null && script_pShot.hit.transform.gameObject == this.gameObject)
        {
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, script_pShot.hit.point);

            if ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) && !compo_Audio.isPlaying) compo_Audio.Play();
            else if (Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0) compo_Audio.Stop();
        }
    }

    void Undo()
    {
        if (!is_Identifying)
        {
            Debug.Log("From Drawing Undo");
            Destroy(All_Line_Obj[Line_Obj_Num]);
            All_Line_Obj.RemoveAt(Line_Obj_Num--);

            compo_Audio.PlayOneShot(button_Sound);
        }
    }

    void ReDrawing()
    {
        if (!is_Identifying)
        {
            Debug.Log("From Drawing Restart");
            for (int i = 0; i <= Line_Obj_Num; i++) Destroy(All_Line_Obj[i]);
            All_Line_Obj = new List<GameObject>();
            Line_Obj_Num = -1;

            compo_Audio.PlayOneShot(button_Sound);
        }
    }

    void Image_Upload()
    {
        if (!is_Identifying)
        {
            if (!False_Identification_On)
            {
                Debug.Log("From Drawing Send Image_To_Server");

                is_Identifying = true;
                Loading_Effect_Obj.SetActive(true);

                Render_Image_To_File();
                StartCoroutine(Send_Image_To_Server());
                compo_Audio.PlayOneShot(button_Sound);
            }
            else
            {
                Debug.Log("From Drawing Send 이미지 인식은 더 이상 지원하지 않습니다");

                is_Identifying = true;
                Loading_Effect_Obj.SetActive(true);

                Render_Image_To_File();
                StartCoroutine(False_Identification());
                compo_Audio.PlayOneShot(button_Sound);
            }
            
        }
    }

    void Render_Image_To_File()
    {
        RenderTexture renderTexture = Cam_For_Capture.targetTexture;
        Texture2D tex2d = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        tex2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex2d.Apply();

        if (!False_Identification_On)
        {
            byte[] bytes = tex2d.EncodeToPNG();
            string another_capture_Method = "Drawing_Capture.jpg";
            another_capture_Method = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), another_capture_Method);
            //Write out the PNG. Of course you have to substitute your_path for something sensible
            File.WriteAllBytes(another_capture_Method, bytes);
        }//진짜 인식
        else
        {
            byte[] bytes = tex2d.EncodeToPNG();
            string another_capture_Method = "Drawing_Capture.jpg";
            another_capture_Method = System.IO.Path.Combine(Application.dataPath, another_capture_Method);
            File.WriteAllBytes(another_capture_Method, bytes);
        }//위장 인식
    }
    IEnumerator Send_Image_To_Server()
    {
        float Start_Time = Time.time;
        WWW localFile = new WWW(file_path);
        yield return localFile;
        WWWForm postForm = new WWWForm();
        postForm.AddBinaryData("myfile", localFile.bytes, "mini_bus.jpg");
        WWW upload = new WWW(Server_Address, postForm);
        yield return upload;
        if (upload.error == null)
        {
            Result_Text = upload.text;
            Identify_Result = Result_Text_Analysis(Result_Text);
            GetComponent<Show_Identify_Result>().Get_Result_Signal(Identify_Result, Highest_Percentage);
            compo_Audio.PlayOneShot(result_On_Sound);
            //data_Path = Application.persistentDataPath;
            Debug.Log(upload.text);                                         //결과로 나온 php 페이지 값을 출력해준다. 해당 php 파일의 결과 값은 단어 하나로 출력된다. ex) ship
        }
        else
        {
            Debug.Log("Error during upload: " + upload.error);
        }

        Taken_Time = Time.time - Start_Time;
        is_Identifying = false;
        Loading_Effect_Obj.SetActive(false);
        Debug.Log("From Drawing CNN _Time : " + Taken_Time);
    }

    IEnumerator False_Identification()
    {
        yield return new WaitForSeconds(3.0f);
        GetComponent<Show_Identify_Result>().Get_Result_Signal("서비스 불가", 0);
        Highest_Percentage = Random.Range(0.50f, 1.00f);
        is_Identifying = false;
        Loading_Effect_Obj.SetActive(false);
    }

    public void Get_Button_Signal(int input)
    {
        if (input == 1) Undo();
        else if (input == 2) ReDrawing();
        else Image_Upload();
    }

    private string Result_Text_Analysis(string result_text)
    {

        float tmp = 0.00f;
        string identified_Result = "None";
        string[] split_string = result_text.Split(String_Seperator, System.StringSplitOptions.RemoveEmptyEntries);
        Highest_Percentage = 0.00f;

        String_Analysis = split_string; // debug

        for (int i = 0; i < split_string.Length; i++)
        {
            switch (split_string[i])
            {
                case "dron":
                    tmp = float.Parse(split_string[++i]);
                    if (tmp > Highest_Percentage)
                    {
                        Highest_Percentage = tmp;
                        identified_Result = "Dron";
                    }
                    Dron = tmp;
                    break;
                case "dna":
                    tmp = float.Parse(split_string[++i]);
                    if (tmp > Highest_Percentage)
                    {
                        Highest_Percentage = tmp;
                        identified_Result = "DNA";
                    }
                    DNA = tmp;
                    break;
                case "car":
                    tmp = float.Parse(split_string[++i]);
                    if (tmp > Highest_Percentage)
                    {
                        Highest_Percentage = tmp;
                        identified_Result = "Car";
                    }
                    Car = tmp;
                    break;
                case "none":
                    tmp = float.Parse(split_string[++i]);
                    if (tmp > Highest_Percentage)
                    {
                        Highest_Percentage = tmp;
                        identified_Result = "None";
                    }
                    None = tmp;
                    break;
                case "cat":
                    tmp = float.Parse(split_string[++i]);
                    if (tmp > Highest_Percentage)
                    {
                        Highest_Percentage = tmp;
                        identified_Result = "Cat";
                    }
                    Cat = tmp;
                    break;
                case "key":
                    tmp = float.Parse(split_string[++i]);
                    if (tmp > Highest_Percentage)
                    {
                        Highest_Percentage = tmp;
                        identified_Result = "Key";
                    }
                    Key = tmp;
                    break;
                case "dog":
                    tmp = float.Parse(split_string[++i]);
                    if (tmp > Highest_Percentage)
                    {
                        Highest_Percentage = tmp;
                        identified_Result = "Dog";
                    }
                    Dog = tmp;
                    break;
                case "falcon":
                    tmp = float.Parse(split_string[++i]);
                    if (tmp > Highest_Percentage)
                    {
                        Highest_Percentage = tmp;
                        identified_Result = "Falcon";
                    }
                    Falcon = tmp;
                    break;
                case "dolphin":
                    tmp = float.Parse(split_string[++i]);
                    if (tmp > Highest_Percentage)
                    {
                        Highest_Percentage = tmp;
                        identified_Result = "Dolphin";
                    }
                    Dolphin = tmp;
                    break;
            }
        }
        return identified_Result;
    }
}
