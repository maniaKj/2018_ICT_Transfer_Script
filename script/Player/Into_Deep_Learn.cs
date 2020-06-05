using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Into_Deep_Learn : MonoBehaviour {
    public GameObject Player;
    public Camera player_Cam;
    public Camera another_Cam;
    public Sprite interleving_Sprite;
    public GameObject cam_Pivot;
    public float image_Alpha = 0.0f;
    public float cam_FOV_speed = 5.0f;
    public float image_Alpha_speed = 10.0f;

    public bool Test_try = false;

    public float FOV_current;
    public float FOV_destination;
    public float FOV_image_insert_Timing;

    public float pivot_x_rotation;
    public float pivot_y_rotation;
    public float rotate_sensitivity = 10.0f;

    //private
    bool cam_Change_state = false;
    float xRotationX;
    float yRotationY;
    float lookSmoothness;
    float xRotation;
    float yRotation;

	void Awake () {
        Player = GameObject.FindGameObjectWithTag("player");
        another_Cam = GetComponent<Camera>();
        another_Cam.enabled = false;
        player_Cam = Camera.main.GetComponent<Camera>();
        FOV_current = player_Cam.fieldOfView;
        cam_Pivot = transform.parent.gameObject;
        if (Player == null || another_Cam == null || player_Cam == null || FOV_destination == 0 || cam_Pivot == null || rotate_sensitivity == 0) Debug.Log("From Into_Deep_Learn 야 스크립트 설정좀 똑바로 해라 " + this.gameObject);
    }
	
	void Update () {
        if (Test_try)
        {
            Test_try = false;
            StartCoroutine(Cam_Change(!cam_Change_state));
        }
        if (cam_Change_state) Cam_pivot_rotate();
	}

    IEnumerator Cam_Change(bool onOff)
    {
        if (onOff)
        {
            while (player_Cam.fieldOfView > FOV_destination)
            {
                player_Cam.fieldOfView -= cam_FOV_speed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            Player.SetActive(false);
            another_Cam.enabled = true;
        }
        else
        {
            Player.SetActive(true);
            another_Cam.enabled = false;
            while (player_Cam.fieldOfView < FOV_current)
            {
                player_Cam.fieldOfView += cam_FOV_speed * Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
        cam_Change_state = onOff;
    }//이미지 이펙트도 필요 나중에 애니메이션으로 적용시키는 게 좋겠다.

    void Cam_pivot_rotate()
    {
        yRotation += Input.GetAxis("Mouse X") * rotate_sensitivity;
        xRotation -= Input.GetAxis("Mouse Y") * rotate_sensitivity;

        xRotation = Mathf.Clamp(xRotation, -90, 100);

        pivot_x_rotation = Mathf.SmoothDamp(pivot_x_rotation, xRotation, ref xRotationX, lookSmoothness);
        pivot_y_rotation = Mathf.SmoothDamp(pivot_y_rotation, yRotation, ref yRotationY, lookSmoothness);

        cam_Pivot.transform.rotation = Quaternion.Euler(pivot_x_rotation, pivot_y_rotation, 0);
    }
}
