using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door_To_Next_Scene : MonoBehaviour {
    [SerializeField] bool Next_Scene_Move_On = false;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Check : " + other.transform.tag);
        if (other.transform.tag == "Player" || other.transform.tag == "player")
        {
            if (Next_Scene_Move_On)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
