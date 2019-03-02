using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classifier_Cam_Control : MonoBehaviour {
    public GameObject pivot_Obj;
    public GameObject Camera_Obj;
    public float lookSensitivity = 5.0f;

    //private
    GameObject player_Cam_Obj;
    FirstPerson_camera script_Cam;
    float normal_Rotation_x, normal_Rotation_y, normal_Rotation_z;
    float CurrentYRotation;
    float yRotation;
    float xRotation;
    float CurrentXRotation;
    float yRotationY;
    float xRotationX;
    float lookSmoothness = 0.05f;

    void Awake()
    {
        player_Cam_Obj = GameObject.FindGameObjectWithTag("MainCamera");
        script_Cam = player_Cam_Obj.GetComponent<FirstPerson_camera>();
        normal_Rotation_x = pivot_Obj.transform.rotation.eulerAngles.x;
        normal_Rotation_y = pivot_Obj.transform.rotation.eulerAngles.y;
        normal_Rotation_z = pivot_Obj.transform.rotation.eulerAngles.z;
        this.enabled = false;
        Camera_Obj.SetActive(false);
    }

    void FixedUpdate () {
        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;

        xRotation = Mathf.Clamp(xRotation, -90, 100);

        CurrentXRotation = Mathf.SmoothDamp(CurrentXRotation, xRotation, ref xRotationX, lookSmoothness);
        CurrentYRotation = Mathf.SmoothDamp(CurrentYRotation, yRotation, ref yRotationY, lookSmoothness);

        pivot_Obj.transform.rotation = Quaternion.Euler(normal_Rotation_x + CurrentXRotation, normal_Rotation_y + CurrentYRotation, normal_Rotation_z);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) Camera_Control_Terminate();
        //if (Input.GetKeyDown(KeyCode.L)) GetComponent<Line_Connection>().ScriptOnOff(true);
        //if (Input.GetKeyDown(KeyCode.M)) GetComponent<Line_Connection>().ScriptOnOff(false);
    }

    public void Get_Camera_Control_Signal()
    {
        this.enabled = true;
        StartCoroutine(script_Cam.Get_Classifier_Camera_Control(false));
        Camera_Obj.SetActive(true);
    }

    public void Camera_Control_Terminate()
    {
        StartCoroutine(script_Cam.Get_Classifier_Camera_Control(true));
        Camera_Obj.SetActive(false);
        this.enabled = false;
        //본체로 돌아가야 함
    }
}
