using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 FirstPerson_camera
public class FirstPerson_move : MonoBehaviour {

    [Header("Variable For Adjusting")]
    public float speed = 7.0f;
    public float sideSpeed = 4.0f;
    public float slideSpeed = 5.0f;
    public float speed_Mul_while_Running = 1.6f;
    [Range(0,1.0f)]public float speed_Mul_while_Sliding = 0.4f;
    public float jumpSpeed = 7.0f;
    [Range(0, 0.5f)] public float speed_Mul_In_Air = 0.05f;
    public float Max_MoveSpeed_InAir = 3.0f;
    public float gravity = 0.5f;
    [SerializeField, Range(0, 90)] float slide_slope_Limit = 30.0f;
    public float next_Foot_Position = 1.0f;
    
    [Space(10)]
    [Header("Variable For Function Check")]
    public bool Move_Control = true;
    [SerializeField] bool isGrounded = true, compo_CC_Grounded = false;
    [SerializeField] bool Slope_Check, Sliding_State; // 거지같았던 슬라이딩
    public LayerMask Player_Land_Check_Layer;
    public Vector3 grouded_CheckBox_Scale = new Vector3(0.5f, 0.1f, 0.5f);
    public Vector3 compo_RigidB_velocity;
    public Vector3 moveDirection;
    public float moveDirection_sqrMagnitude;
    

    //for Applying Object and File
    [Space(15)]
    [Header("Object and File Applying")]
    public GameObject cameraObj;
    public GameObject BottomObj;
    [SerializeField] private float walking_sound_delay;
    [SerializeField] private AudioClip[] Clip_FootstepSound;
    [SerializeField] private AudioClip Clip_JumpSound;
    [SerializeField] private AudioClip Clip_LandSound;
    

    //for Debuging && Hide
    [Header("Debuging Check Variable")]
    public bool camSetting = true;
    public bool drawCircle, drawCube;

    //Hide in inspector & private
    Rigidbody compo_RigidB;
    CharacterController compo_CC;
    Vector3 Temp_MoveDir_InAir;
    private AudioSource compo_Audio;
    Transform bottomTransform;
    private bool walking_sound_on = false;
    private bool Temp_Jump_memory = false;
    private float Original_Speed, Original_Side_Speed;
    public Collider[] Platform_Check;

    //거지같았던 슬라이딩 변수 시리즈
    Vector3 Slope_Normal_Vec, Normal_Slope_Vec_toApply, slideDirection;
    bool Sliding_State_Change_Delay = false;
    

    void Start () {
        if (camSetting)
        {
            transform.GetChild(0).GetComponent<FirstPerson_camera>().enabled = false;
            transform.GetChild(0).GetComponent<FirstPerson_camera>().enabled = true;
        }//이렇게 카메라 스크립트 껏다켰다 하면 캠 떨리는 현상이 사라짐 이유는 모르겄다
        bottomTransform = BottomObj.GetComponent<Transform>();
        if(GetComponent<Rigidbody>() != null) compo_RigidB = GetComponent<Rigidbody>();
        if (GetComponent<CharacterController>() != null) compo_CC = GetComponent<CharacterController>();
        compo_Audio = GetComponent<AudioSource>();
        Original_Side_Speed = sideSpeed; Original_Speed = speed;
        compo_CC.slopeLimit = slide_slope_Limit;
    }
	
	void FixedUpdate () {
        
    }

    void Update()
    {
        compo_CC_Grounded = compo_CC.isGrounded;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= speed_Mul_while_Running;
            sideSpeed *= speed_Mul_while_Running;
        }//Run
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = Original_Speed;
            sideSpeed = Original_Side_Speed;
        }//Run -> Walk
        if (Move_Control) move();

    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Slope_Normal_Vec = hit.normal;
        if (Vector3.Angle(Vector3.up, hit.normal) > slide_slope_Limit && Vector3.Angle(Vector3.up, hit.normal) < 70)
        {
            Slope_Check = true;
            //Sliding_State = true;
            StopAllCoroutines();
            StartCoroutine(Sliding_State_Change_Delay_Coroutine());
        }
        else
        {
            Slope_Check = false;
            if (!Sliding_State_Change_Delay) Sliding_State = false;
        }//Sliding Slope Check
    }
    void move()
    {
        //Character Ground Check
        //isGrounded = Physics.CheckSphere(bottomTransform.position, grouded_CheckBox_Scale.x, Player_Land_Check_Layer);
        isGrounded = Physics.CheckBox(bottomTransform.position, grouded_CheckBox_Scale, transform.rotation, Player_Land_Check_Layer);

        //Rotation
        transform.rotation = Quaternion.Euler(0, cameraObj.GetComponent<FirstPerson_camera>().CurrentYRotation, 0);

        // Sound
        if (!walking_sound_on && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)) StartCoroutine(Walk_Sound()); 

        
        if (compo_CC_Grounded && !Sliding_State)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal") * sideSpeed,0, Input.GetAxis("Vertical") * speed);
            moveDirection = transform.TransformDirection(moveDirection);
        }//Moving in Ground
        else if(!compo_CC_Grounded)
        {
            Temp_Jump_memory = false;
            Temp_MoveDir_InAir = new Vector3(Input.GetAxis("Horizontal") * sideSpeed * speed_Mul_In_Air, gravity, Input.GetAxis("Vertical") * speed * speed_Mul_In_Air);
            Temp_MoveDir_InAir = transform.TransformDirection(Temp_MoveDir_InAir);
            if (new Vector2(moveDirection.x, moveDirection.z).sqrMagnitude > Mathf.Pow(Max_MoveSpeed_InAir,2.0f))
            {
                if (Mathf.Abs(moveDirection.x + Temp_MoveDir_InAir.x) < Mathf.Abs(moveDirection.x))
                {
                    moveDirection.x += Temp_MoveDir_InAir.x;
                    moveDirection.z += Temp_MoveDir_InAir.z;
                }
            }
            else
            {
                moveDirection.x += Temp_MoveDir_InAir.x;
                moveDirection.z += Temp_MoveDir_InAir.z;
            }
            //점프할 때 프레임시간 곱하지 않으면 프레임 낮을 때 더 높이 점프하는 현상이 있음!! 주의!!
            moveDirection.y -= gravity * Time.deltaTime;
        }//Moving in Air
        else if(compo_CC_Grounded && Sliding_State)
        {
            if (Slope_Check) Normal_Slope_Vec_toApply = Slope_Normal_Vec;
            //Normal_Slope_Vec_toApply = Slope_Normal_Vec;
            moveDirection = new Vector3(Input.GetAxis("Horizontal") * sideSpeed * speed_Mul_while_Sliding, -5, Input.GetAxis("Vertical") * speed * speed_Mul_while_Sliding);
            moveDirection = transform.TransformDirection(moveDirection);
            slideDirection.x = Normal_Slope_Vec_toApply.x * slideSpeed;
            slideDirection.z = Normal_Slope_Vec_toApply.z * slideSpeed;

            moveDirection.x += slideDirection.x;
            moveDirection.z += slideDirection.z;
        }//Moving in Sliding

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            moveDirection.y += jumpSpeed;
            PlayJumpSound();
            Temp_MoveDir_InAir = Vector3.zero;
            Temp_Jump_memory = false;
        }//Jump
        compo_CC.Move(moveDirection * Time.deltaTime);
        moveDirection_sqrMagnitude = new Vector2(moveDirection.x, moveDirection.z).sqrMagnitude;

        //Move With Platform, Platform Check
        Platform_Check = Physics.OverlapSphere(bottomTransform.position, grouded_CheckBox_Scale.y);
        bool IsGroundedOn_Platform = false;
        foreach (Collider col in Platform_Check)
        {
            if (col.GetComponent<Platform_Ver2>() != null)
            {
                IsGroundedOn_Platform = true;
                transform.parent = col.transform;
            }
        }
        if (!IsGroundedOn_Platform) transform.parent = null;
    }

    public void Player_Stop()
    {
        //compo_RigidB.velocity = Vector3.zero;
    }

    private void PlayJumpSound()
    {
        compo_Audio.clip = Clip_JumpSound;
        compo_Audio.Play();
    }

    private void PlayFootStepAudio()
    {
        if (!isGrounded)
        {
            return;
        }

        int n = Random.Range(1, Clip_FootstepSound.Length);
        compo_Audio.clip = Clip_FootstepSound[n];
        compo_Audio.PlayOneShot(compo_Audio.clip);

        Clip_FootstepSound[n] = Clip_FootstepSound[0];
        Clip_FootstepSound[0] = compo_Audio.clip;
    }

    IEnumerator Walk_Sound()
    {
        walking_sound_on = true;
        PlayFootStepAudio();
        yield return new WaitForSeconds(walking_sound_delay);
        walking_sound_on = false;
    }

    IEnumerator Sliding_State_Change_Delay_Coroutine()
    {
        Sliding_State_Change_Delay = true;
        yield return new WaitForSeconds(0.3f);
        Sliding_State_Change_Delay = false;
    }

    void OnDrawGizmos()
    {
       if(drawCircle)Gizmos.DrawSphere(BottomObj.transform.position, grouded_CheckBox_Scale.x);
       if(drawCube)Gizmos.DrawCube(BottomObj.transform.position, grouded_CheckBox_Scale);
    }
}
