using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower_AIchange : MonoBehaviour {
    public GameObject color_Sphere;

    //private
    Material mt;
    Tower_test tower_test_script;
	// Use this for initialization
	void Awake () {
        for (int i = 0; i < transform.childCount; i++) if (transform.GetChild(i).tag == "Marker") color_Sphere = transform.GetChild(i).gameObject;
        mt = color_Sphere.GetComponent<Renderer>().material;
        tower_test_script = GetComponent<Tower_test>();
	}
	
	// Update is called once per frame

    public void GetSignal(int bullet_state)
    {
        //State_change(bullet_state);
    }

    public void Get_Module_Signal(int moduleID)
    {
        if (moduleID % 100 < 10) State_change(moduleID % 10);
    } //모듈 신호 받으면 타워 기능 변화

    void State_change(int state_into)
    {
        switch (state_into)
        {
            case 4:
                //mt.color = Color.blue;
                tower_test_script.TowerFunc = Tower_test.TowerStyle.CNN;
                break;
            case 8:
                //mt.color = Color.yellow;
                tower_test_script.TowerFunc = Tower_test.TowerStyle.Electric;
                break;
            /*case 3:
                //mt.color = Color.green;
                tower_test_script.TowerFunc = Tower_test.TowerStyle.LightManage;
                break;
            case 9:
                //mt.color = Color.white;
                tower_test_script.TowerFunc = Tower_test.TowerStyle.NULL;
                break;*/
            default:
                break;
        }
    } // 상태 변화

    void Script_off()
    {
        this.enabled = false;
    }
}
