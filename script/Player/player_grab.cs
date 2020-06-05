using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//player_shot이랑 연동
public class player_grab : MonoBehaviour {
    public GameObject position_Obj;
    public bool State_Grab = false;
    public float throw_speed = 5.0f;
    public float position_speed = 10.0f;
    
    //private
    Transform liftingPosition;
    GameObject target_Obj;
    Collider[] hit;
    Vector3 scale_store;
    bool No_rigidbody_Obj = false;
    float get_distance;

    // Use this for initialization
    void Awake() {
        liftingPosition = position_Obj.transform;
        get_distance = Vector3.Distance(liftingPosition.position, Camera.main.transform.position);
    }

    // Update is called once per frame
    void Update() {
         if (!State_Grab && Input.GetKeyDown(KeyCode.F)) Grab_Obj();
         else if(State_Grab)
        {
            Vector3 Direc_position = liftingPosition.position - target_Obj.transform.position;

            target_Obj.GetComponent<Rigidbody>().velocity = Direc_position * position_speed;
            target_Obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (Input.GetMouseButtonDown(0)) StartCoroutine(Throw_Obj());
            if (Input.GetKeyDown(KeyCode.F)) StartCoroutine(PutDown_Obj());
        }
    }

    /*private void FixedUpdate()
    {
        if(State_Grab)
        {
            Vector3 Direc_position = liftingPosition.position - target_Obj.transform.position;

            target_Obj.GetComponent<Rigidbody>().velocity = Direc_position * position_speed;
            target_Obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            if (Input.GetMouseButtonDown(0)) StartCoroutine(Throw_Obj());
            if (Input.GetKeyDown(KeyCode.F)) StartCoroutine(PutDown_Obj());
        }
    }*/

    void Grab_Obj()
    {
        Get_target();
        if (target_Obj != null)
        {
            State_Grab = true;
            GetComponent<player_shot>().grab_check = true;
            scale_store = target_Obj.transform.localScale;
            target_Obj.transform.rotation = Quaternion.identity;
            target_Obj.transform.SetParent(liftingPosition);
            target_Obj.transform.position = liftingPosition.position;
            target_Obj.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    IEnumerator Throw_Obj()
    {
        State_Grab = false;
        target_Obj.GetComponent<Rigidbody>().useGravity = true;
        target_Obj.transform.parent = null;
        target_Obj.GetComponent<Rigidbody>().velocity = liftingPosition.forward * throw_speed;
        target_Obj.transform.localScale = scale_store;
        target_Obj = null;
        yield return new WaitForSeconds(GetComponent<player_shot>().fire_delay);
        GetComponent<player_shot>().grab_check = false;
    }

    IEnumerator PutDown_Obj()
    {
        State_Grab = false;
        target_Obj.GetComponent<Rigidbody>().useGravity = true;
        target_Obj.transform.parent = null;
        target_Obj.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target_Obj.transform.localScale = scale_store;
        target_Obj = null;
        yield return new WaitForSeconds(GetComponent<player_shot>().fire_delay);
        GetComponent<player_shot>().grab_check = false;
    }

    void Get_target()
    {
        target_Obj = null;
        RaycastHit hit_obj;
        Ray temp_Ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if(Physics.Raycast(temp_Ray, out hit_obj, get_distance))
            if(hit_obj.transform.GetComponent<InteractiveObj_MultiTag>() != null && hit_obj.transform.GetComponent<InteractiveObj_MultiTag>().Canbe_Grab)
                target_Obj = hit_obj.transform.gameObject;
    }

    void Script_Off()
    {
        this.enabled = false;
    }
}
