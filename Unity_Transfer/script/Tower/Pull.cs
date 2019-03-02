using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//해당 타워의 Tower_Test와 연동
public class Pull : MonoBehaviour {
    [HideInInspector] public GameObject target;
    public GameObject midObj;
    public GameObject obj_Lightning;
    public GameObject Axis_Obj;
    public GameObject Particle_Obj;
    public AudioClip Collision_Sound;
    public AudioClip Pulling_Sound;
    public float pulling_sound_Delay = 0.5f;
    public float Axis_Rotation_Speed = 0.1f;
    bool upCheck = false;
    public float up_Time = 0.1f;
    bool criticalSection_Check = false;
    public float dropDist = 3.0f;
    public float pullSpeed = 80.0f;
    public float upSpeed = 2.0f;
    public float line_Renderer_aim_down_value = 10.0f;

    //for check
    float objSpeed;
    Vector3 objSpeedVector;
    float tower_detectRange;
    float distance_obj;
    Vector3 velocity_Direction;
    bool RigidBody_Remove = false;

    //private
    Rigidbody rb;
    DigitalRuby.LightningBolt.LightningBoltScript script_L;
    AudioSource compo_Audio;

	void Awake()
    {
        this.enabled = false;
        tower_detectRange = GetComponent<Tower_test>().Target_DetectRange + 1.0f;
        for (int i = 0; i < transform.childCount; i++) if (transform.GetChild(i).tag == "Pivot") midObj = transform.GetChild(i).gameObject;
        if (midObj == null) Debug.Log("From Pull 피봇 오브젝트 없어요 // " + this.gameObject);
        script_L = obj_Lightning.GetComponent<DigitalRuby.LightningBolt.LightningBoltScript>();
        if(script_L == null) Debug.Log("From Pull 라이트닝 라인 렌더러 오브젝트 없어요 // " + this.gameObject);
        obj_Lightning.SetActive(false);

        if (GetComponent<AudioSource>() == null) this.gameObject.AddComponent<AudioSource>();
        compo_Audio = GetComponent<AudioSource>();
        StartCoroutine(Axis_Rotation());
    }
	
	void FixedUpdate () {
        if (criticalSection_Check) ObjPull();
        distance_obj = Vector3.Distance(midObj.transform.position, target.transform.position);

    }

    IEnumerator ObjUp()
    {
        float current_time = Time.time;
        while (Time.time - current_time < up_Time)
        {
            velocity_Direction.y += upSpeed;
            rb.AddForce(target.transform.up * upSpeed);
            yield return new WaitForFixedUpdate();
        }
        
        //Debug.Log("From Pull 위로 힘 작용" + this.gameObject);
    }

    IEnumerator Pulling_Sound_Play()
    {
        
        while (criticalSection_Check)
        {
            compo_Audio.clip = Pulling_Sound;
            compo_Audio.PlayOneShot(compo_Audio.clip);
            yield return new WaitForSeconds(pulling_sound_Delay);
        }
    }

    IEnumerator Axis_Rotation()
    {
        while (true)
        {
            if (Axis_Obj != null) Axis_Obj.transform.Rotate(0, 0, Axis_Rotation_Speed);
            yield return new WaitForSeconds(0.03f);
        }
    }

    void ObjPull()
    {
        //나중에 시간 추가해서 일정 시간이 지나면 그냥 놔버리도록 수정(버그 대안)
        //float distance = Vector3.Distance(midObj.transform.position, target.transform.position);
        //Debug.Log("From Pull 당기기 함수 작용" + this.gameObject);
        if (distance_obj > dropDist && distance_obj < tower_detectRange)
        {
            //Debug.Log("From Pull 당기기 작용" + this.gameObject);
            Vector3 Direction = (midObj.transform.position - target.transform.position).normalized;
            rb.MovePosition(target.transform.position + Direction * pullSpeed * Time.deltaTime);
            objSpeed = rb.velocity.magnitude;
            objSpeedVector = rb.velocity;
            script_L.StartPosition = midObj.transform.position;
            script_L.EndPosition = target.transform.position;
            if (target.tag == "player") script_L.EndPosition += Vector3.down * line_Renderer_aim_down_value;
        }
        else if (distance_obj > tower_detectRange)
        {
            Debug.Log("From Pull 거리가 너무 멀다");
            //Particle_Obj.GetComponent<ParticleSystem>().Play();
            scriptOff(); //여기 나중에 수정할 것
        }
        else
        {
            compo_Audio.clip = Collision_Sound;
            compo_Audio.PlayOneShot(compo_Audio.clip);
            //Debug.Log("From Pull object pulling Ended");
            Particle_Obj.GetComponent<ParticleSystem>().Play();
            target.GetComponent<Rigidbody>().velocity = Vector3.zero;
            scriptOff();
        }
    }

    public void GetSignal(GameObject receiver)
    {
        target = receiver;
        distance_obj = Vector3.Distance(midObj.transform.position, target.transform.position);
        if (!criticalSection_Check && distance_obj < tower_detectRange)
        {
            //Debug.Log("From Pull object pulling Started");
            if (target.transform.position.y > transform.position.y) upCheck = false;
            else upCheck = true;
            if (target.GetComponent<CharacterController>() != null) target.GetComponent<CharacterController>().enabled = false;
            if (target.GetComponent<Rigidbody>() == null)
            {
                target.AddComponent<Rigidbody>();
                target.GetComponent<Rigidbody>().freezeRotation = false;
                RigidBody_Remove = true;
            }
            rb = target.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            criticalSection_Check = true;
            if (target.GetComponent<FirstPerson_move>() != null) target.GetComponent<FirstPerson_move>().enabled = false;
            if (upCheck) StartCoroutine(ObjUp());
            obj_Lightning.SetActive(true);
            StartCoroutine(Pulling_Sound_Play());
        }
        else
        {
            Debug.Log("From Pull 거리가 너무 멀거나 중복 작용됨");
            scriptOff();
        }
    }

    public void Get_Rotate_Signal()
    {
        StartCoroutine(Axis_Rotation());
    }

    void scriptOff()
    {
        if (RigidBody_Remove) Destroy(target.GetComponent<Rigidbody>());
        if (target.GetComponent<FirstPerson_move>() != null) target.GetComponent<FirstPerson_move>().enabled = true;
        if (target.GetComponent<CharacterController>() != null) target.GetComponent<CharacterController>().enabled = true;
        criticalSection_Check = false;
        obj_Lightning.SetActive(false);
        RigidBody_Remove = false;
        this.enabled = false;
    }
}
