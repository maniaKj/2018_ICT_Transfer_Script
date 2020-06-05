using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Photo_Identification : MonoBehaviour {

    [SerializeField]
    private Camera m_Camera;

    [SerializeField]
    private float m_ServerResponseTime;
    public float ServerResponseTime
    {
        get
        {
            return m_ServerResponseTime;
        }
    }

    [SerializeField]
    private player_RMfunction m_PlayerHand;

    [SerializeField]
    private string m_ResultText = string.Empty;
    public string ResultText
    {
        get
        {
            return m_ResultText;
        }
    }

    [SerializeField]
    private float m_HighestResultPercentage = 0.0f;
    public float HighestResultPercentage
    {
        get
        {
            return m_HighestResultPercentage;
        }
    }

    [SerializeField]
    private bool m_IsServerRunning = false;
    public bool IsServerRunning
    {
        get
        {
            return m_IsServerRunning;
        }
    }

    private string m_FilePath = string.Empty;

    private string m_ServerAddress = "http://35.229.255.144/unity_object/upload.php";

    private int m_CaptureCount = 0;

    private string[] m_StringSeperator = { "QQQ", "QQQQ" };

    private void Awake()
    {
        string tmp_char = "" + (char)92;
        string tmp = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        m_FilePath = "file://" + tmp.Replace(tmp_char, "//") + "//screenshot.jpg";
    }

    public IEnumerator SendWWWformToServer()
    {
        m_IsServerRunning = true;

        float startTime = Time.time;

        WWW localFile = new WWW(m_FilePath);
        yield return localFile;

        WWWForm postForm = new WWWForm();
        postForm.AddBinaryData("myfile", localFile.bytes, "mini_bus.jpg");
        WWW upload = new WWW(m_ServerAddress, postForm);
        yield return upload;


        if (upload.error == null)
        {
            m_HighestResultPercentage = 0.00f;
            ProcessServerMessage(upload.text);
        }
        else
        {
            Debug.Log("Error during upload: " + upload.error);
        }

        m_ServerResponseTime = Time.time - startTime;

        m_IsServerRunning = false;
    }

    public void CaptureAndUploadScreenShot()
    {
        CaptureScreenShot();
        StartCoroutine(SendWWWformToServer());
    }

    private void CaptureScreenShot()
    {
        m_CaptureCount++;

        var renderTexture = m_Camera.targetTexture;
        var texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        RenderTexture.active = renderTexture;

        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture.Apply();

        string screenShotName = "screenshot.jpg";
        screenShotName = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), screenShotName);

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(screenShotName, bytes);
    }

    private void UpdateHighestPercentageResult(string category, string message)
    {
        float percentage = float.Parse(message);
        if (percentage > m_HighestResultPercentage)
        {
            m_HighestResultPercentage = percentage;
            m_ResultText = category;
        }
    }

    private void ProcessServerMessage(string serverMessage)
    {
        var messages = serverMessage.Split(m_StringSeperator, System.StringSplitOptions.RemoveEmptyEntries);

        for(int i = 0;i < messages.Length; i += 2)
        {
            switch (messages[i])
            {
                case "dron" : UpdateHighestPercentageResult("Dron", messages[i + 1]); break;
                case "dna": UpdateHighestPercentageResult("DNA", messages[i + 1]); break;
                case "car": UpdateHighestPercentageResult("Car", messages[i + 1]); break;
                case "none": UpdateHighestPercentageResult("None", messages[i + 1]); break;
                case "cat": UpdateHighestPercentageResult("Cat", messages[i + 1]); break;
                case "key": UpdateHighestPercentageResult("Key", messages[i + 1]); break;
                case "dog": UpdateHighestPercentageResult("Dog", messages[i + 1]); break;
                case "falcon": UpdateHighestPercentageResult("Falcon", messages[i + 1]); break;
                case "dolphin": UpdateHighestPercentageResult("Dolphin", messages[i + 1]); break;
            }
        }
    }
}
