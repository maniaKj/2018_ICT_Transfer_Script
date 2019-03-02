using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Video_Cam_Move : MonoBehaviour {
    [Header("Variable For Adjust")]
    [SerializeField] float zoom_FOV_value = 70;
    [SerializeField] Quaternion cam_Pivot_Rotation_Quat;
    [SerializeField] Vector3 cam_Pivot_Rotation_Vector;
    [SerializeField] float cam_Move_Time = 1.0f;
    [SerializeField] Vector3 cam_pivot_Bias;

    [Header("Apply Value And Obj")]
    [SerializeField] GameObject Rotate_Pivot;
    [SerializeField] Transform[] Move_To_Position;
    [SerializeField] GameObject Look_Target;

    [Header("Variable For Test")]
    [SerializeField] bool Zoom_In_Out = false;
    [SerializeField] bool cam_Rotation_Fix_byValue = false;
    [SerializeField] bool cam_Look_at_Target = false;
    [SerializeField] int cam_Move_To_Target = -1;

    //Hide
    Camera compo_Cam;
    Vector3 ref_Velocity;

    private void Awake()
    {
        compo_Cam = GetComponent<Camera>();
        cam_pivot_Bias = this.gameObject.transform.localPosition;
        cam_Pivot_Rotation_Quat = Rotate_Pivot.transform.rotation;
    }

    private void Update()
    {
        this.transform.localPosition = cam_pivot_Bias;
    }

    private void FixedUpdate()
    {
        compo_Cam.fieldOfView = zoom_FOV_value;
        if (cam_Rotation_Fix_byValue) Rotate_Pivot.transform.rotation = cam_Pivot_Rotation_Quat;
        else Cam_Pivot_Rotate();
        if (cam_Look_at_Target) Cam_Look_At();
        if (cam_Move_To_Target != -1) Cam_Move_To(cam_Move_To_Target);
    }

    void Cam_Look_At()
    {
        this.transform.LookAt(Look_Target.transform);
    }

    void Cam_Pivot_Rotate()
    {
        cam_Pivot_Rotation_Quat = Rotate_Pivot.transform.rotation;
        Rotate_Pivot.transform.Rotate(cam_Pivot_Rotation_Vector);
    }

    void Cam_Zoom_In_Out()
    {

    }

    void Cam_Move_To(int target_index)
    {
        try
        {
            if (Vector3.Distance(Move_To_Position[target_index].position, Rotate_Pivot.transform.position) > 2.0f)
            {
                Rotate_Pivot.transform.position = Vector3.SmoothDamp(Rotate_Pivot.transform.position, Move_To_Position[target_index].position, ref ref_Velocity, cam_Move_Time);
            }
            else cam_Move_To_Target = -1;
        }
        catch
        {
            Debug.Log("From Video_Cam_Move cam_Move has no target : Error");
            cam_Move_To_Target = -1;
        }
    }
}
