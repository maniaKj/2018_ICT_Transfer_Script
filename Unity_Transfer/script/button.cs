using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class button : MonoBehaviour {
    public int buttonCase = 0;
    public GameObject leftdoor;
    public GameObject lightdoor;

    void OnCollisionEnter (Collision obj) {
		if(obj.gameObject.tag == "bullet" && buttonCase == 0)
        {
            Destroy(leftdoor);
            Destroy(lightdoor);
        }

        if (obj.gameObject.GetComponent<InteractiveObj_MultiTag>().ObjType == InteractiveObj_MultiTag.AllType.NormalCube && buttonCase == 1)
        {
            switch (buttonCase)
            {
                case 1:
                    SceneManager.LoadScene(0);
                    break;
                case 2:
                    SceneManager.LoadScene(1);
                    break;
                default:
                    break;
            }
        }
	}
}
