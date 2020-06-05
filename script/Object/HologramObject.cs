using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramObject : MonoBehaviour {
    public enum HologramTypes
    {
        Dron = 0,
        Car,
        DNA,
        Key,
        Cat,
        Dog,
        Falcon,
        Dolphin
    };
    public class HologramTypeComparer : IEqualityComparer<HologramTypes>
    {
        public bool Equals(HologramTypes x, HologramTypes y)
        {
            return x == y;
        }
        public int GetHashCode(HologramTypes obj)
        {
            return (int)obj;
        }
    }

    static private readonly Dictionary<HologramTypes, string> s_HologramTypeNameDic = new Dictionary<HologramTypes, string>(new HologramTypeComparer())
    {
        {HologramTypes.Dron,  "dron"},
        {HologramTypes.Car,  "car"},
        {HologramTypes.DNA,  "dna"},
        {HologramTypes.Key,  "key"},
        {HologramTypes.Cat,  "cat"},
        {HologramTypes.Dog,  "dog"},
        {HologramTypes.Falcon,  "falcon"},
        {HologramTypes.Dolphin,  "dolphin"}
    };
    public const float IDENTIFY_TIME = 1.5f;

    [SerializeField]
    private HologramTypes m_Type;
    public HologramTypes Type
    {
        get
        {
            return m_Type;
        }
    }

    [SerializeField]
    private int m_ID;
    public int ID
    {
        get
        {
            return m_ID;
        }
    }

    private string m_Name;
    public string Name
    {
        get
        {
            return m_Name;
        }
    }

    [SerializeField]
    private bool m_IsIdentified = false;
    public bool IsIdentified
    {
        get
        {
            return m_IsIdentified;
        }
    }

    private bool m_IsIdentificationProcessing = false;

    [SerializeField]
    private BoxCollider m_Collider;

    [SerializeField]
    private GameObject m_RaycastingBarrier;

    [SerializeField]
    private AudioClip m_DissolveEffectSound;

    [SerializeField]
    private AudioSource m_AudioSource;

    [SerializeField, Range(0.0f, 1.0f)]
    private float m_CurrentAlpha = 1.0f;

    [SerializeField]
    private float m_AlphaSpeed = 0.05f;

    [SerializeField]
    private List<Renderer> m_ChildRenderers = new List<Renderer>();

    private void Awake()
    {
        m_Name = s_HologramTypeNameDic[m_Type];
        m_RaycastingBarrier.SetActive(true);
    }

    public void OnIdentify()
    {
        m_IsIdentified = true;
    }

    private void OnActivate(bool isActive)
    {
        m_CurrentAlpha = isActive ? 0.0f : 1.0f;
        m_Collider.enabled = isActive;
        m_RaycastingBarrier.SetActive(true);

        int alphaDirection = isActive ? 1 : -1;
        StartCoroutine(DissolveEffectRoutine(alphaDirection));
    }

    public void OnDisappear()
    {
        OnActivate(false);
    }

    public void OnAppear()
    {
        OnActivate(true);
        for (int i = 0; i < m_ChildRenderers.Count; i++)
        {
            m_ChildRenderers[i].enabled = true;
        }
    }

    private IEnumerator DissolveEffectRoutine(int alphaDirection)
    {
        m_AudioSource.PlayOneShot(m_DissolveEffectSound);
        while (m_CurrentAlpha >= 0.0f && m_CurrentAlpha <= 1.0f)
        {
            m_CurrentAlpha += alphaDirection * m_AlphaSpeed;
            for (int i = 0; i < m_ChildRenderers.Count; i++)
            {
                for (int j = 0; j < m_ChildRenderers[i].materials.Length; j++)
                {
                    m_ChildRenderers[i].materials[j].SetFloat("_AlphaCut", m_CurrentAlpha);
                }
            }
            yield return new WaitForEndOfFrame();
        }
        if (m_CurrentAlpha > 1.0f)
        {
            for (int i = 0; i < m_ChildRenderers.Count; i++)
            {
                m_ChildRenderers[i].enabled = false;
            }
        }
        
    }

    public void OnIdentificationStart()
    {
        m_RaycastingBarrier.SetActive(false);

        m_IsIdentificationProcessing = true;

        Invoke("OnIdentificationEnd", IDENTIFY_TIME);
    }

    private void OnIdentificationEnd()
    {
        m_IsIdentificationProcessing = false;

        m_RaycastingBarrier.SetActive(true);
    }
}
