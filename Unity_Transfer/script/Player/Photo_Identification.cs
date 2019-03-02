using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


//연계  : player_RMfunction
public class Photo_Identification : MonoBehaviour {
    public string new_filepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
    public string TruePath = "file://c://Users//김진표//Documents//screenshot.jpg";
    public string file_path;
    //public WWW new_wwwpath = new WWW("file://" +  Application.dataPath + "//screenshot.jpg");
    string Server_Address = "http://35.229.255.144/unity_object/upload.php";               //업로드 php 주소
    //string Server_Address = "http://unity_Object";
    int num = 0;

    public bool False_Identification_On = false;

    public Camera Cam_for_Capture;
    public string Result_Text;
    public float Taken_Time;

    [Header("All_Result : ")]
    [Space(5)]
    public string Identify_Result;
    public float Highest_Percentage = 0.00f;
    public float Dron, DNA, Car, None, Cat, Dog, Falcon, Key, Dolphin;


    public string[] String_Analysis;
    [HideInInspector]public string data_Path;
    
    private string[] String_Seperator = { "QQQ", "QQQQ" };


    // 입력 : ../../test.jpg


    private void Awake()
    {
        string tmp_char = "" + (char)92;
        string tmp = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        file_path = "file://" + tmp.Replace(tmp_char, "//") + "//screenshot.jpg";
    }

    public IEnumerator Send_Photo_Signal()
    {
        GetComponent<player_RMfunction>().Cnn_Server_Running = true;
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
            data_Path = Application.persistentDataPath;
            //Debug.Log(upload.text);                                         //결과로 나온 php 페이지 값을 출력해준다. 해당 php 파일의 결과 값은 단어 하나로 출력된다. ex) ship
        }
        else
        {
            Debug.Log("Error during upload: " + upload.error);
        }

        Taken_Time = Time.time - Start_Time;
        Debug.Log("From Photo_Identification CNN _Time : " + Taken_Time + ", Result : " + Identify_Result + ", " + Highest_Percentage * 100);
        GetComponent<player_RMfunction>().Cnn_Server_Running = false;
        if(Highest_Percentage == 0.00f) GetComponent<player_RMfunction>().Get_Cnn_Result_Signal("None", 0); // 응답이 없을 때
        else GetComponent<player_RMfunction>().Get_Cnn_Result_Signal(Result_Text, Taken_Time);
    }

    IEnumerator False_Identification(GameObject target)
    {
        GetComponent<player_RMfunction>().Cnn_Server_Running = true;
        float Start_Time = Time.time;

        yield return new WaitForSeconds(3.0f);
        Identify_Result = target.GetComponent<Data_Hologram>().StringID;
        Highest_Percentage = Random.Range(0.50f, 1.00f);
        Taken_Time = Time.time - Start_Time;
        Debug.Log("From Photo_Identification CNN _Time : " + Taken_Time + ", Result : " + Identify_Result + ", " + Highest_Percentage * 100);
        GetComponent<player_RMfunction>().Cnn_Server_Running = false;
        GetComponent<player_RMfunction>().Get_Cnn_Result_Signal(Result_Text, Taken_Time);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2) && !GetComponent<player_RMfunction>().Cnn_Server_Running)
        {
            StartCoroutine(Send_Photo_Signal());
        }
        if (Input.GetKeyDown(KeyCode.F7) && !GetComponent<player_RMfunction>().Cnn_Server_Running)
        {
            RendTex_Read();
            string screenshot_Name = "Original_screenshot" + num++ + ".jpg";
            Debug.Log("Captureshot");
            data_Path = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), screenshot_Name);
        }

    }

    public void Send_Photo_To_CNN_Server(GameObject target)
    {
        if (!False_Identification_On)
        {
            Refined_Screenshot_Capture();
            StartCoroutine(Send_Photo_Signal());
            //Debug.Log("Why?");
        }
        else
        {
            Refined_Screenshot_Capture();
            StopAllCoroutines();
            StartCoroutine(False_Identification(target));
        }
    }

    void RendTex_Read()
    {
        RenderTexture renderTexture = Cam_for_Capture.targetTexture;
        Texture2D tex2d = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        tex2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex2d.Apply();


        byte[] bytes = tex2d.EncodeToPNG();
        string another_capture_Method = "Refined_Screenshot" + num + ".jpg";
        another_capture_Method = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), another_capture_Method);
        File.WriteAllBytes(another_capture_Method, bytes);
    }

    void Refined_Screenshot_Capture()
    {
        RenderTexture renderTexture = Cam_for_Capture.targetTexture;
        Texture2D tex2d = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        tex2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex2d.Apply();

        if (!False_Identification_On)
        {
            byte[] bytes = tex2d.EncodeToPNG();
            string another_capture_Method = "screenshot.jpg";
            another_capture_Method = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), another_capture_Method);
            //Write out the PNG. Of course you have to substitute your_path for something sensible
            File.WriteAllBytes(another_capture_Method, bytes);
        }//진짜 인식
        else
        {
            byte[] bytes = tex2d.EncodeToPNG();
            string another_capture_Method = "screenshot.jpg";
            another_capture_Method = System.IO.Path.Combine(Application.dataPath, another_capture_Method);
            File.WriteAllBytes(another_capture_Method, bytes);
        }//위장 인식


    }

    private string Result_Text_Analysis(string result_text)
    {

        float tmp = 0.00f;
        string identified_Result = "None";
        string[] split_string = result_text.Split(String_Seperator, System.StringSplitOptions.RemoveEmptyEntries);
        Highest_Percentage = 0.00f;

        String_Analysis = split_string; // debug
        
        for(int i = 0;i < split_string.Length;i++)
        {
            switch (split_string[i])
            {
                case "dron" :
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
