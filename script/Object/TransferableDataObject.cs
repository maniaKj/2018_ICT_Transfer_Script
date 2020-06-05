using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferableDataObject : MonoBehaviour {
    [SerializeField]
    private int m_ID;
    public int ID
    {
        get
        {
            return m_ID;
        }
    }

    [SerializeField, Range(0.0f, 1.0f)]
    private float m_CurrentAlpha = 0.0f;

    [SerializeField]
    private float m_AlphaSpeed = 0.03f;

    private int m_AlphaDirection = 1;

    [SerializeField]
    private bool m_IsStaticSprite = false;

    [SerializeField]
    private bool m_IsStaticColor = false;

    [SerializeField]
    private bool m_IsSingleUse = false;

    [SerializeField]
    private bool m_IsTransferPossible = true;
    public bool IsTransferPossible
    {
        get
        {
            return m_IsTransferPossible;
        }
    }

    [SerializeField]
    private MagneticMachine m_ConnectedMachine = null;
    public MagneticMachine ConnectedMachine
    {
        get
        {
            return m_ConnectedMachine;
        }
    }

    [SerializeField]
    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private SpriteRenderer m_ChildSpriteRenderer;

    [SerializeField]
    private BoxCollider m_Collider;

    [SerializeField]
    private Obj_Module_Operation m_ModuleOperator;

    [SerializeField]
    private AudioSource m_AudioSource;

    [SerializeField]
    private AudioClip m_EffectSound;

    private void Awake()
    {
        ChangeAppearence(m_ID);
    }

    private void Start()
    {
        m_ModuleOperator.OnChildChange(ID);
    }

    public void OnDataTransfer(int targetID)
    {
        m_ID = targetID;
        ChangeAppearence(m_ID);
        if (m_ModuleOperator)
        {
            m_ModuleOperator.OnChildChange(m_ID);
        }
    } //신호받으면 외관, m_ID 바꾸고 부모 오브젝트로 신호

    public void OnMachineDataTransfer(MagneticMachine machine)
    {
        m_ConnectedMachine = machine;
    }

    public void OnActivate(bool isActive)
    {
        m_Collider.enabled = isActive;
        m_SpriteRenderer.enabled = isActive;
        m_ChildSpriteRenderer.enabled = isActive;
    }

    public void DetachConnectedMachine()
    {
        m_ConnectedMachine = null;
    }

    private void SetSprite(Sprite s)
    {
        m_SpriteRenderer.sprite = s;
        m_ChildSpriteRenderer.sprite = s;
    }

    private void SetMaterial(Material m)
    {
        m_SpriteRenderer.material = m;
        m_ChildSpriteRenderer.material = m;
    }

    public void ChangeAppearence(int index)
    {
        if (!m_IsStaticSprite)
        {
            SetSprite(ResourceRepository.Instance.TransferableDataSprite[index]);

            StopAllCoroutines();
            StartCoroutine(DissolveEffectRoutine());
        }

        if (!m_IsStaticColor)
        {
            if (index < -1)
            {
                SetMaterial(ResourceRepository.Instance.Materials[11]);
            }
            else
            {
                SetMaterial(ResourceRepository.Instance.Materials[10]);
            }
        }
    }

    private IEnumerator DissolveEffectRoutine()
    {
        m_CurrentAlpha = m_AlphaDirection > 0 ? 0.01f : 0.99f;
        m_AudioSource.clip = m_EffectSound;

        while (m_CurrentAlpha <= 1.0f && m_CurrentAlpha >= 0.0)
        {
            m_CurrentAlpha += m_AlphaDirection * m_AlphaSpeed;
            m_SpriteRenderer.material.SetFloat("_AlphaCut", m_CurrentAlpha);
            m_ChildSpriteRenderer.material.SetFloat("_AlphaCut", m_CurrentAlpha);
            yield return new WaitForEndOfFrame();
        }
    }
}
