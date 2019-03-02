using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Test_Cam_Capture : MonoBehaviour {

    WebCamTexture webCamTexture;
    public Camera cam_Obj;
    
    public RenderTexture renderTexture;
    public Texture2D test_Texture;
    public bool Test_Start = false, rendTex_Test = false;
    public Vector2Int photoSize = new Vector2Int(128, 128);

    void FixedUpdate()
    {
        if (Test_Start) Photo_CaptureTest();
        if (rendTex_Test) RendTex_Read();
        
    }

    void Photo_CaptureTest()
    {
        Test_Start = false;

        //Texture2D photo = new Texture2D(512,512);
        Texture2D texture = new Texture2D(photoSize.x, photoSize.y);
        GetComponent<Renderer>().material.mainTexture = texture;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                //Color color = ((x & y) != 0 ? Color.white : Color.gray);
                int tmp_x = x * Mathf.FloorToInt(test_Texture.width/ photoSize.x);
                int tmp_y = y * Mathf.FloorToInt(test_Texture.height/ photoSize.y);
                texture.SetPixel(x, y, test_Texture.GetPixel(tmp_x,tmp_y));
            }
        }
        texture.Apply();
        Debug.Log("cam render texture size.x :" + cam_Obj.targetTexture.width);
        //photo.SetPixels(webCamTexture.GetPixels());
        //photo.Apply();

        //Encode to a PNG
        byte[] bytes = texture.EncodeToPNG();
        byte[] bytes2 = RTImage(cam_Obj).EncodeToJPG();
        string another_capture_Method = "another_Capture.jpg";
        another_capture_Method = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), another_capture_Method);
        string another_capture_Method2 = "render_Capture.jpg";
        another_capture_Method = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), another_capture_Method2);
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        File.WriteAllBytes(another_capture_Method, bytes);
        File.WriteAllBytes(another_capture_Method2, bytes2);
    }

    void RendTex_Read()
    {
        rendTex_Test = false;
        renderTexture = cam_Obj.targetTexture;
        Texture2D tex2d = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        RenderTexture.active = renderTexture;
        tex2d.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex2d.Apply();

        
        byte[] bytes = tex2d.EncodeToPNG();
        string another_capture_Method = "Render_Capture.jpg";
        another_capture_Method = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), another_capture_Method);
        //Write out the PNG. Of course you have to substitute your_path for something sensible
        File.WriteAllBytes(another_capture_Method, bytes);
    }

    Texture2D RTImage(Camera cam)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = currentRT;
        return image;
    }
}
