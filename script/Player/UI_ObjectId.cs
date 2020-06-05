using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//FirstPerson_camera / SendUiSignal() 참조 거기서 바라보는 오브젝트 정보를 받는다
public class UI_ObjectId : MonoBehaviour {
    public float Image_alpha = 0.4f;
    public bool Switch_ID_UI = false;
    bool[] array_imageShow = new bool[] { false, false, false };
    public bool[] array_func = new bool[] { false, false, false };
    public GameObject Obj_Text;
    public GameObject[] Obj_Image_A;
    public GameObject Obj_Target;
    Color StartColor;
    public Color TargetColor;

    void Start()
    {
        StartColor = Obj_Image_A[0].GetComponent<Image>().color;
        TargetColor = new Vector4(StartColor.r, StartColor.g, StartColor.b, Image_alpha);
    }

	/*void Update () {
        if (Switch_ID_UI)
        {
            //나중에 여기 거리 수정
            Collider[] hit = Physics.OverlapSphere(Obj_Target.transform.position, 15.0f);
            foreach(Collider hitObj in hit)
            {
                if (hitObj.GetComponent<Tower_test>() != null)
                {
                    switch (hitObj.GetComponent<Tower_test>().TowerFunc)
                    {
                        case Tower_test.TowerStyle.Electric:
                            //Debug.Log("electric check");
                            array_imageShow[0] = true;
                            Obj_Image_A[0].GetComponent<Image>().color = StartColor;
                            break;
                        case Tower_test.TowerStyle.visionID:
                            //Debug.Log("vision check");
                            array_imageShow[1] = true;
                            Obj_Image_A[1].GetComponent<Image>().color = StartColor;
                            break;
                        case Tower_test.TowerStyle.HologramBurst:
                            //Debug.Log("hologram check");
                            array_imageShow[2] = true;
                            Obj_Image_A[2].GetComponent<Image>().color = StartColor;
                            break;
                        default:
                            break;
                    }
                }
            }
            for(int i =0;i<3;i++)
            {
                if(!array_imageShow[i]) Obj_Image_A[i].GetComponent<Image>().color = TargetColor;
                array_imageShow[i] = false;
            }
        }
	}*/

    public void UI_On(GameObject target)
    {
        Obj_Target = target;
        if (Obj_Target.GetComponent<InteractiveObj_MultiTag>() != null)
        {
            Switch_ID_UI = true;
            Obj_Text.GetComponent<Text>().text = Obj_Target.GetComponent<InteractiveObj_MultiTag>().Obj_Name;
            Obj_Text.SetActive(true);

            if (Obj_Target.GetComponent<InteractiveObj_MultiTag>().Is_canbePulled || Obj_Target.GetComponent<InteractiveObj_MultiTag>().Platform_Pull) array_func[0] = true;
            else array_func[0] = false;
            /*if (Obj_Target.GetComponent<InteractiveObj_MultiTag>().Is_photoIdentify) array_func[1] = true;
            else array_func[1] = false;
            if (Obj_Target.GetComponent<InteractiveObj_MultiTag>().Is_canCreateHologram) array_func[2] = true;
            else array_func[2] = false;*/

            for (int i = 0; i < 3; i++)
                if (array_func[i]) Obj_Image_A[i].SetActive(true);
                else Obj_Image_A[i].SetActive(false);
        }
        else
        {
            Obj_Text.SetActive(false);
            Switch_ID_UI = true;
        }
    }

    public void UI_Off()
    {
        Obj_Text.SetActive(false);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if(Obj_Target != null) Gizmos.DrawWireSphere(Obj_Target.transform.position, 15.0f);
    }
}
