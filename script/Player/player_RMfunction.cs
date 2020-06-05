using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_RMfunction : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Hand;

    [SerializeField]
    private GameObject m_BeamEffect;

    [SerializeField]
    private SpriteRenderer[] m_HandEffectSprite;

    [SerializeField]
    private Show_Identify_Result m_ResultView;

    [SerializeField]
    private Photo_Identification m_Identifier;

    [SerializeField]
    private bool m_IsHandOn = false;

    [SerializeField]
    private bool m_IsScanning = false;

    [SerializeField]
    private Sprite[] m_EffectImages;

    [SerializeField]
    private int[] m_PhotoChangeDelay;

    [SerializeField]
    private player_shot m_PlayerShot;

    [SerializeField]
    private Animator m_Animator;

    [SerializeField]
    private AudioClip m_ScanningSound;

    [SerializeField]
    private AudioSource m_AudioSource;

    private bool m_IsResultValid = false;

    void Awake()
    {
        m_Animator = m_Hand.GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        m_ScanningSound = GameObject.Find("Repository_Obj").GetComponent<ResourceRepository>().identify_Effect_Sound;

        m_EffectImages = Resources.LoadAll<Sprite>("photo/Cnn_Photo_Effect");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !GetComponent<player_shot>().grab_check)
        {
            m_Animator.SetInteger("state", 1);
            m_IsHandOn = true;
            m_BeamEffect.SetActive(true);
            GetComponent<player_shot>().hand_check = true;
            StopAllCoroutines();
            for (int i = 0; i < m_HandEffectSprite.Length; i++) m_HandEffectSprite[i].gameObject.SetActive(false);
            StartCoroutine(Photo_Change_Effect2());
            StartCoroutine(Photo_Identify_Coroutine());
            
            //Debug.Log("Form player_RMfunction Mid mouse button held Down");
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            m_Animator.SetInteger("state", 2);
            m_IsHandOn = false;
            m_BeamEffect.SetActive(false);
            GetComponent<player_shot>().hand_check = false;
            //Debug.Log("Form player_RMfunction Mid mouse button up");
        }

    }

    public void SetServerResult(string result, float time)
    {
        if(result == "None" && time == 0)
        {
            //Debug.Log("From player_RMfunction GetNone Signal");
        }
        else
        {
            m_IsResultValid = true;
        }
    }

    IEnumerator Photo_Identify_Coroutine()
    {
        float Identification_Start_Time = 0.0f;
        m_AudioSource.PlayOneShot(m_ScanningSound);
        int t_Cycle = 0;
        while (m_IsHandOn)
        {
            if(t_Cycle >= 10)
            {
                t_Cycle = 0;
                m_AudioSource.Stop();
                m_AudioSource.PlayOneShot(m_ScanningSound);
            }
            yield return new WaitForSeconds(0.5f); t_Cycle++;
            if (m_PlayerShot.Cnn_Obj_Check && m_PlayerShot.hit.transform.GetComponent<HologramObject>() && !m_PlayerShot.hit.transform.GetComponent<HologramObject>().IsIdentified && !m_IsResultValid)
            {
                m_PlayerShot.hit.transform.GetComponent<HologramObject>().OnIdentificationStart();

                yield return new WaitForSeconds(0.1f);
                if (!m_IsScanning)
                {
                    GetComponent<Photo_Identification>().CaptureAndUploadScreenShot();
                    Identification_Start_Time = Time.time; //시작 시간 수정할 것
                    Debug.Log("From player_RMfunction Sending Signal to Data Hologram");
                    m_IsScanning = true;
                }
            }
            else if (m_IsResultValid && m_PlayerShot.Cnn_Obj_Check)
            {
                m_IsResultValid = false;
                m_IsScanning = false;

                if (Mathf.Abs(Time.time - Identification_Start_Time - m_Identifier.ServerResponseTime) <= 1.0f)
                {
                    m_PlayerShot.hit.transform.GetComponent<HologramObject>().OnIdentify();
                    if (m_Identifier.ResultText == m_PlayerShot.hit.transform.GetComponent<HologramObject>().Name)
                    StartCoroutine(Identfy_Result_Show_Delay());
                    for (int i = 0; i < m_HandEffectSprite.Length; i++)
                    {
                        m_HandEffectSprite[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (m_PlayerShot.Cnn_Obj_Check && m_PlayerShot.hit.transform.GetComponent<HologramObject>().IsIdentified)
            {
                
                GetComponent<player_shot>().Get_Hologram_Data(GetComponent<player_shot>().hit.transform.gameObject);
                Identification_Start_Time = 0.0f;
                m_IsScanning = false;
                for (int i = 0; i < m_HandEffectSprite.Length; i++)
                {
                    m_HandEffectSprite[i].gameObject.SetActive(false);
                }
            }
            else
            {
                Identification_Start_Time = 0.0f;
                m_IsScanning = false;
                for (int i = 0; i < m_HandEffectSprite.Length; i++)
                {
                    m_HandEffectSprite[i].gameObject.SetActive(false);
                }
            }
        }
        m_IsScanning = false;
        m_AudioSource.Stop();
    }

    IEnumerator Photo_Change_Effect2()
    {
        int cycle = 0;
        while (m_IsHandOn)
        {
            yield return new WaitForSeconds(0.1f);
            if (m_IsScanning)
            {
                for (int i = 0; i < m_HandEffectSprite.Length; i++)
                {
                    m_HandEffectSprite[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < m_HandEffectSprite.Length; i++)
                {
                    m_HandEffectSprite[i].gameObject.SetActive(true);
                }
            }

            for (int i = 0; i < m_HandEffectSprite.Length; i++)
            {
                if (cycle % m_PhotoChangeDelay[i] == 0)
                {
                    m_HandEffectSprite[i].sprite = m_EffectImages[Mathf.RoundToInt(Random.Range(0.0f, m_EffectImages.Length - 1))];
                }
            }
        }
    }

    IEnumerator Identfy_Result_Show_Delay()
    {
        yield return new WaitForSeconds(0.3f);
        m_ResultView.Get_Result_Signal(m_Identifier.ResultText, m_Identifier.HighestResultPercentage);
    }
}
