using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Video_Subtitle_effect : MonoBehaviour {
    [Header("Variable For Adjust")]
    [SerializeField] float subtitle_Show_Time = 4.0f;
    [SerializeField] float subtitle_Move_speed = 2.0f;
    [SerializeField] float subtitle_Alpha_speed = 0.1f;
    [SerializeField] Vector2 subtitle_Move_Direction = new Vector2(1f, 0f);

    [Header("Variable For Test")]
    [SerializeField] bool Test_Subtitle_Effect = false;

    //hide
    Text ui_Text;
    RectTransform ui_TransForm;
    Vector2 text_Origin_poisition;

    private void Awake()
    {
        ui_Text = GetComponent<Text>();
        ui_TransForm = GetComponent<RectTransform>();

        ui_Text.color = new Color(ui_Text.color.r, ui_Text.color.g, ui_Text.color.b, 0f);
        text_Origin_poisition = ui_TransForm.anchoredPosition;
    }

    private void FixedUpdate()
    {
        if (Test_Subtitle_Effect)
        {
            Test_Subtitle_Effect = false;
            StopAllCoroutines();
            StartCoroutine(Subtitle_Effect());
        }
    }

    IEnumerator Subtitle_Effect()
    {
        ui_Text.enabled = true;
        ui_TransForm.anchoredPosition = text_Origin_poisition;
        ui_Text.color = new Color(ui_Text.color.r, ui_Text.color.g, ui_Text.color.b, 0f);

        StartCoroutine(Subtitle_Move_Fade(1));
        yield return new WaitForSeconds(subtitle_Show_Time);
        StartCoroutine(Subtitle_Move_Fade(-1));
    }

    IEnumerator Subtitle_Move_Fade(int textOnOff)
    {
        while (ui_Text.color.a >= 0.0f && ui_Text.color.a <= 1.0f)
        {
            ui_Text.color = new Color(ui_Text.color.r, ui_Text.color.g, ui_Text.color.b , ui_Text.color.a + textOnOff * subtitle_Alpha_speed);
            ui_TransForm.anchoredPosition += subtitle_Move_Direction * subtitle_Move_speed * textOnOff;
            yield return new WaitForSeconds(0.04f);
        }
        ui_Text.color = new Color(ui_Text.color.r, ui_Text.color.g, ui_Text.color.b, Mathf.Clamp01(ui_Text.color.a));
        if (ui_Text.color.a == 0.0f) ui_Text.enabled = false;
    }
}
