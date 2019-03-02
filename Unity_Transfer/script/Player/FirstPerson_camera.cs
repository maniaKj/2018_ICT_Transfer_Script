using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//연계 player_shot, FirstPerson_move
public class FirstPerson_camera : MonoBehaviour {

    
    public float lookSensitivity = 5.0f;
    public float zoom_Value = 10.0f;
    public float zoom_Speed = 140.0f;
    public GameObject GunObj;
    public float Gun_Move_Width;
    public float Gun_Move_Speed;
    [HideInInspector] public bool Camera_Control = true;
    [HideInInspector] public float CurrentYRotation;

    //private
    Camera player_Cam;
    GameObject player;
    GameObject UiObj;
    FirstPerson_move script_move;
    player_grab script_grab;
    player_RMfunction script_RM;
    player_shot script_shot;
    int UI_layer = 1;
    float yRotation;
    float xRotation;
    float CurrentXRotation;
    float yRotationY;
    float xRotationX;
    public float Gun_x_Move = 0, Gun_y_Move = 0;
    float Gun_X_Current, Gun_Y_Current;
    float lookSmoothness = 0.05f;
    float FOV_current;
    bool Key_T_Down = false;
    /*float bobSpeed = 1.0f;
    float stepCounter;
    float bobAmountX = 0.1f;
    float bobAmountY = 0.1f;
    Vector3 lastPosition;
    float heightRatio = 0.9f;
    float aimingTrue = 1.0f;
    float cameraDefault = 60.0f;
    float targetCamera = 60.0f;
    float cameraZoom = 1.0f;
    float cameraZoomTo;
    float cameraZoomSpeed = 0.1f;*/

    void Awake () {
        player = GameObject.FindGameObjectWithTag("player");
        CurrentYRotation = transform.parent.rotation.eulerAngles.y;
        yRotation = transform.parent.rotation.eulerAngles.y;
        Gun_X_Current = GunObj.transform.localPosition.x;
        Gun_Y_Current = GunObj.transform.localPosition.y;
        UiObj = GameObject.Find("/Canvas");
        script_move = transform.parent.GetComponent<FirstPerson_move>();
        script_grab = transform.parent.GetComponent<player_grab>();
        script_RM = transform.parent.GetComponent<player_RMfunction>();
        script_shot = transform.parent.GetComponent<player_shot>();
        player_Cam = GetComponent<Camera>();
        FOV_current = player_Cam.fieldOfView;
	}
	
	void FixedUpdate () {
        if (Camera_Control)
        {
            yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
            xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
            

            xRotation = Mathf.Clamp(xRotation, -90, 100);

            CurrentXRotation = Mathf.SmoothDamp(CurrentXRotation, xRotation, ref xRotationX, lookSmoothness);
            CurrentYRotation = Mathf.SmoothDamp(CurrentYRotation, yRotation, ref yRotationY, lookSmoothness);

            transform.localRotation = Quaternion.Euler(CurrentXRotation, 0, 0);

            Gun_x_Move -= Input.GetAxis("Mouse X") * Gun_Move_Width; if (Mathf.Abs(Gun_x_Move) > 0.0001f) Gun_x_Move *= Gun_Move_Speed;
            Gun_y_Move -= Input.GetAxis("Mouse Y") * Gun_Move_Width; if (Mathf.Abs(Gun_y_Move) > 0.0001f) Gun_y_Move *= Gun_Move_Speed;

            GunObj.transform.localPosition = new Vector3(Gun_X_Current + Gun_x_Move, Gun_Y_Current + Gun_y_Move, GunObj.transform.localPosition.z);

            SendUiSignal();
        }

        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Key_T_Down = true;
            lookSensitivity = 1;
            StartCoroutine(Zoom_Change(true));
        }//줌인
        if (Input.GetMouseButtonUp(2))
        {
            Key_T_Down = false;
            lookSensitivity = 5;
            StartCoroutine(Zoom_Change(false));
        }//줌아웃
        /*if (Input.GetKeyUp(KeyCode.P))
        {
            Debug.Log("ScreenShot_Capture + 위치 : " + Application.persistentDataPath);
            ScreenCapture.CaptureScreenshot("SomeLevel.png");
        }//스크린샷*/

    }

    void SendUiSignal()
    {
        /*RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out hit,Mathf.Infinity,UI_layer)) UiObj.GetComponent<UI_ObjectId>().UI_On(hit.transform.gameObject);
        else UiObj.GetComponent<UI_ObjectId>().UI_Off();*/
        //여기 나중에 레이어 넣어서 구분할 것
    }

    public IEnumerator Get_Classifier_Camera_Control(bool onOff)
    {
        Camera_Control = onOff;
        script_move.Player_Stop();
        script_move.enabled = onOff;
        script_RM.enabled = onOff;
        script_grab.enabled = onOff;
        this.gameObject.GetComponent<Camera>().enabled = onOff;
        if(!onOff) script_shot.enabled = false;
        yield return new WaitForSeconds(0.1f);
        if(onOff) script_shot.enabled = true;
    }

    IEnumerator Zoom_Change(bool onOff)
    {
        if (onOff)
        {
            while (player_Cam.fieldOfView > zoom_Value && Key_T_Down)
            {
                player_Cam.fieldOfView -= zoom_Speed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            player_Cam.fieldOfView = zoom_Value;
        }
        else
        {
            while (player_Cam.fieldOfView < FOV_current && !Key_T_Down)
            {
                player_Cam.fieldOfView += zoom_Speed * 3 * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            player_Cam.fieldOfView = FOV_current;
        }
    }//이미지 이펙트도 필요 나중에 애니메이션으로 적용시키는 게 좋겠다.
}


