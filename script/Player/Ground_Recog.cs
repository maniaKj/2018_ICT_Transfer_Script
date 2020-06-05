using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground_Recog : MonoBehaviour {
    public Vector3 collision_normal;
    private void OnCollisionStay(Collision collision)
    {
        collision_normal = collision.contacts[0].normal;
    }
}
