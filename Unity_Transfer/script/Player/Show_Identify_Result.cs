using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Show_Identify_Result : MonoBehaviour {
    [Header("Variable For Adujust")]
    [SerializeField] float Shader_Effect_Speed = 7.0f;
    [SerializeField] float showing_Result_Time = 4.0f;
    [SerializeField] float showing_Next_Text_Term = 0.6f;

    [Header("Variable for Check")]
    [SerializeField] GameObject child_Text_Obj;
    [SerializeField] string identify_Result_String;
    [SerializeField] float identify_Result_Percentage;

    [Header("Applying Variable")]
    [SerializeField] GameObject Result_Text_Obj;

    //hide
    Renderer parent_Text_Rend, child_Text_Rend;
    TextMesh child_Text;

    private void Awake()
    {
        parent_Text_Rend = Result_Text_Obj.GetComponent<Renderer>();
        child_Text_Obj = Result_Text_Obj.transform.GetChild(0).gameObject;
        child_Text_Rend = child_Text_Obj.GetComponent<Renderer>();
        child_Text = child_Text_Obj.GetComponent<TextMesh>();

        parent_Text_Rend.material.SetFloat("_DissolveValue", 0.00f);
        child_Text_Rend.material.SetFloat("_DissolveValue", 0.00f);

        parent_Text_Rend.enabled = false;
        child_Text_Rend.enabled = false;
    }

    public void Get_Result_Signal(string result_Name, float result_Percentage)
    {
        identify_Result_String = result_Name;
        identify_Result_Percentage = result_Percentage;
        child_Text.text = result_Name + " : " + Mathf.Round(result_Percentage * 100f) + "%";

        StopAllCoroutines();
        StartCoroutine(Showing_Text_Control());
    }

    IEnumerator Showing_Text_Control()
    {
        parent_Text_Rend.enabled = true;
        child_Text_Rend.enabled = true;
        parent_Text_Rend.material.SetFloat("_DissolveValue", 0.00f);
        child_Text_Rend.material.SetFloat("_DissolveValue", 0.00f);

        StartCoroutine(Result_Text_Shader_Effect(parent_Text_Rend, 1));
        yield return new WaitForSeconds(showing_Next_Text_Term);
        StartCoroutine(Result_Text_Shader_Effect(child_Text_Rend, 1));
        yield return new WaitForSeconds(showing_Result_Time -  showing_Next_Text_Term);
        StartCoroutine(Result_Text_Shader_Effect(parent_Text_Rend, -1));
        StartCoroutine(Result_Text_Shader_Effect(child_Text_Rend, -1));

        //parent_Text_Rend.enabled = false;
        //child_Text_Rend.enabled = false;
    }

    IEnumerator Result_Text_Shader_Effect(Renderer target, int text_OnOff)
    {
        while (target.material.GetFloat("_DissolveValue") <= 1.00f && target.material.GetFloat("_DissolveValue") >= 0.00f)
        {
            target.material.SetFloat("_DissolveValue", target.material.GetFloat("_DissolveValue") + Shader_Effect_Speed * Time.fixedDeltaTime * text_OnOff);
            yield return new WaitForFixedUpdate();
        }
        target.material.SetFloat("_DissolveValue", Mathf.Clamp01(target.material.GetFloat("_DissolveValue")));
        if (target.material.GetFloat("_DissolveValue") == 0.00f) target.enabled = false;
    }
}
