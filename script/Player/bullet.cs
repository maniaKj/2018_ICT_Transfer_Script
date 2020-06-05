using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//연계 Obj_Module_Deploy, InteractiveObj_MultiTag, player_shot
//총알 인식 잘 안되는 현상은 어떻게 해결해야 할지
public class bullet : MonoBehaviour {
    public float bulletSpeed = 20.0f;
    public float DestroyTime = 1.0f;
    public enum bulletState { trigger, AI_electric, AI_classifier, AI_manage};
    public bulletState bullet_State;
    public int bullet_id;

    public GameObject bullet_Particle;

    //test
    public GameObject test_point;

    //private
    Transform myT;
    GameObject Player;
    Vector3 bullet_Destination;
    public Vector3 particle_normal;


	void Awake () {
        Player = GameObject.FindGameObjectWithTag("player");
        myT = transform;
        Destroy(gameObject, DestroyTime);
        switch (bullet_State)
        {
            case bulletState.AI_electric:
                bullet_id = 1;
                break;
            case bulletState.AI_classifier:
                bullet_id = 2;
                break;
            case bulletState.AI_manage:
                bullet_id = 3;
                break;
            case bulletState.trigger:
                bullet_id = 0;
                break;
        }
    }
	
	void FixedUpdate () {
        myT.position += myT.forward * bulletSpeed * Time.deltaTime;
	}
    
    void OnCollisionEnter(Collision col)
    {
        //Debug.Log("From bullet Collision point : " + col.contacts[0].point);// 총 조준 확인용
        //Instantiate(ParticleObj, myT.position, myT.rotation);  
        //나중에 파티클 효과 추가할 것, 벽에 부딪혀서 사라질 때, 오브텍트에 부딪혀서 효과가 적용될 때
        //총알 잘 안나가는 버그 플레이어랑 콜리션 영역 아슬아슬하게 걸쳐서 발생되는 것으로 추정
        if (bullet_id != 200)
        {
            if (col.gameObject.GetComponent<Obj_Module_Deploy>() != null) col.gameObject.GetComponent<Obj_Module_Deploy>().Get_Signal();
            else if (col.gameObject.GetComponent<Classifier_Cam_Control>() != null) col.gameObject.GetComponent<Classifier_Cam_Control>().Get_Camera_Control_Signal();
            else if (col.gameObject.GetComponent<MagneticMachine>() != null) col.gameObject.GetComponent<MagneticMachine>().Get_Signal(Player, Vector3.zero);
            //else if (col.gameObject.GetComponent<Tower_test>() != null && col.gameObject.GetComponent<Tower_test>().TowerFunc == Tower_test.TowerStyle.Electric) col.gameObject.GetComponent<Tower_test>().Get_Signal(Player, Vector3.zero);
            else
            {
                GameObject tmp = Instantiate(bullet_Particle, bullet_Destination, Quaternion.FromToRotation(Vector3.zero, particle_normal));
            }
            //tmp.transform.LookAt(Player.transform);
            //tmp.transform.localEulerAngles = 
            //tmp.transform.localEulerAngles = new Vector3(Mathf.Asin(particle_normal.z), Mathf.Asin(particle_normal.y), 0);
        }
        /*if(bullet_id == 1)
        {
            Vector3 contact_Point = col.contacts[0].point;
            Instantiate(test_point, col.contacts[0].point, Quaternion.identity);
            Debug.Log("From bullet contact point : " + col.contacts[0].point + " contacted obj name(this) : " + col.contacts[0].thisCollider.name + "other Obj name : " + col.contacts[0].otherCollider.name);
        }*/
        /*if (bullet_id == 1 && col.gameObject.GetComponent<InteractiveObj_MultiTag>() != null)
        {
            Vector3 contact_Point = col.contacts[0].point;
            col.gameObject.GetComponent<InteractiveObj_MultiTag>().Get_Signal2(bullet_Destination);
        }
        if (bullet_id == 0 && col.gameObject.GetComponent<InteractiveObj_MultiTag>() != null) col.gameObject.GetComponent<InteractiveObj_MultiTag>().Get_Signal();*/
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        //col.SendMessage()
        if (bullet_id == 2)
        {
            if(col.gameObject.GetComponent<Obj_Module_Deploy>() != null) col.gameObject.GetComponent<Obj_Module_Deploy>().Get_Signal();
            //홀로그램 충돌 파티클 뿌려랏
            Bullet_Destroy();
        }
        if (bullet_id == 1 && col.gameObject.GetComponent<Puzzle_Part_Hologram>() != null)
        {
            Vector3 contact_Point = this.gameObject.transform.position;
            col.gameObject.GetComponent<InteractiveObj_MultiTag>().Get_Signal2(bullet_Destination);
        }
    }

    public void Get_Signal(Vector3 destination, Vector3 normal)
    {
        bullet_Destination = destination;
        particle_normal = normal;
    }

    void Bullet_Destroy()
    {
        //particle Here
        Destroy(gameObject);
    }
}
