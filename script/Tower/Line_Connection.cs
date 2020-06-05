using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//연계 funciton_CNN, Classifier_Cam_Control
public class Line_Connection : MonoBehaviour {
    public GameObject LR_Obj;
    public float tiling_Speed = 0.05f;
    public bool line_is_On = false;



    Monitor script_M;
    LineRenderer obj_lineRenderer;
    Renderer obj_Renderer;
    float counter;
    float dist;
    float tile_offset = 0.0f;

    Transform origin;
    Transform destination;

    float lineDrawSpeed = 6f;

    void Awake()
    {
        obj_lineRenderer = LR_Obj.GetComponent<LineRenderer>();
        obj_Renderer = LR_Obj.GetComponent<Renderer>();
        origin = GetComponent<Classifier_Cam_Control>().pivot_Obj.transform;
        
        obj_lineRenderer.enabled = false;

        obj_lineRenderer.startWidth = 0.45f;
        obj_lineRenderer.endWidth = 0.45f;

        StartCoroutine(Wait_And_Awake());
    }
    IEnumerator Wait_And_Awake()
    {
        yield return new WaitForSeconds(0.2f);
        destination = GetComponent<function_CNN>().Connected_Monitor.GetComponent<Monitor>().antenna_Obj.transform;
        //StartCoroutine(Connect_Line());
    }

    IEnumerator Connect_Line()
    {
        while (line_is_On)
        {
            obj_lineRenderer.SetPosition(0, origin.position);
            obj_lineRenderer.SetPosition(1, destination.position);
            if (tile_offset < 10.0f)
            {
                tile_offset += tiling_Speed;
                obj_Renderer.material.SetTextureOffset("_MainTex", new Vector2(tile_offset, 0));
            }
            else tile_offset = 0.0f;
            //Debug.Log("From Line_Connection Coroutine test");
            yield return new WaitForSeconds(0.03f);
        }
    }
    

    public void ScriptOnOff(bool onOff)
    {
        if (onOff && !line_is_On)
        {
            line_is_On = true;
            obj_lineRenderer.enabled = true;
            StartCoroutine(Connect_Line());
        }
        else if(!onOff)
        {
            line_is_On = false;
            obj_lineRenderer.enabled = false;
        }
    }
}
