using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle_Drawing : MonoBehaviour {

    [Header("Variable for Adjust")]
    [SerializeField] private float simplifyTolerance = 0.02f;
    public int Connect_Signal, Reset_Signal;

    [Header("Variable for Check")]
    public int Line_Obj_Num = -1;
    public List<GameObject> All_Line_Obj = new List<GameObject>();

    [Header("Apply Obj & Variable")]
    public GameObject Line_Rend_Obj;
    public GameObject Circuit_Start_Node;
    public GameObject[] Circuit_End_Node;
    [SerializeField] AudioClip Connenct_Sound;
    //Hide

    [SerializeField] player_shot script_pShot;
    LineRenderer line;
    AudioSource compo_Audio;
    bool Drawing_Start = false;

    private void Awake()
    {
        compo_Audio = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (script_pShot != null) script_pShot = GameObject.FindGameObjectWithTag("player").GetComponent<player_shot>();
        else Debug.Log("From Drawing Player_Shot script wasn't applied");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && script_pShot.hit.transform.gameObject == Circuit_Start_Node)
        {
            //Line_Obj_Num++;
            ReDrawing();
            All_Line_Obj.Add(Instantiate(Line_Rend_Obj));
            line = All_Line_Obj[++Line_Obj_Num].GetComponent<LineRenderer>();

            line.positionCount = 1;
            line.SetPosition(0, script_pShot.hit.point);

            compo_Audio.Play();
            Drawing_Start = true;
            //line.SetPosition(line.positionCount - 1, GameObject.Find("Player").transform.position);
        }

        if (Drawing_Start && Input.GetMouseButtonUp(0))
        {
            line.Simplify(simplifyTolerance);
            compo_Audio.Stop();
            Drawing_Start = false;
        }
        


    }
    private void FixedUpdate()
    {
        if (Drawing_Start && script_pShot.hit.transform.gameObject == this.gameObject)
        {
            line.positionCount++;
            line.SetPosition(line.positionCount - 1, script_pShot.hit.point);

            if ((Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) && !compo_Audio.isPlaying) compo_Audio.Play();
            else if(Input.GetAxis("Mouse X") == 0 && Input.GetAxis("Mouse Y") == 0) compo_Audio.Stop();
        }
        else if (Drawing_Start)
        {
            compo_Audio.Stop();
            for (int i = 0; i < Circuit_End_Node.Length; i++)
                if (script_pShot.hit.transform.gameObject == Circuit_End_Node[i])
                {
                    line.Simplify(simplifyTolerance);
                    Circuit_End_Node[i].SendMessage("Get_Circuit_Signal", Connect_Signal);

                    compo_Audio.PlayOneShot(Connenct_Sound);
                    Drawing_Start = false;
                }
        }
    }

    void Undo()
    {
        Debug.Log("From Drawing Undo");
        Destroy(All_Line_Obj[Line_Obj_Num]);
        All_Line_Obj.RemoveAt(Line_Obj_Num--);
    }

    void ReDrawing()
    {
        Debug.Log("From Drawing Restart");
        for (int i = 0; i <= Line_Obj_Num; i++) Destroy(All_Line_Obj[i]);
        All_Line_Obj = new List<GameObject>();
        Line_Obj_Num = -1;
        for (int i = 0; i < Circuit_End_Node.Length; i++) Circuit_End_Node[i].SendMessage("Get_Circuit_Signal", Reset_Signal);
    }

    public void Get_Button_Signal(int input)
    {
        if (input == 1) Undo();
        else if (input == 2) ReDrawing();
    }
}
