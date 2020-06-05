using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//연계 Circuit_Node
public class Circuit : MonoBehaviour {
    public int signal_Type = 0;
    public Material[] line_Mat;

    //private
    GameObject repository_Obj;

    void Awake()
    {
        repository_Obj = GameObject.Find("Repository_Obj");
        line_Mat = repository_Obj.GetComponent<ResourceRepository>().Materials;
        Get_Circuit_Signal(0); 
    }

    public void Get_Circuit_Signal(int signal)
    {
        signal_Type = signal;
        if (signal_Type != 0 && signal_Type != 7 && signal_Type != 8) GetComponent<Renderer>().material = line_Mat[1];
        else if(signal_Type == 0 || signal_Type == 7) GetComponent<Renderer>().material = line_Mat[0];
        else if(signal_Type == 8) GetComponent<Renderer>().material = line_Mat[2];
        
    }//신호 받으면 색이랑 신호값 변경
}
