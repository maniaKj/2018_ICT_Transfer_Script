using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPerson_move : MonoBehaviour {

    [SerializeField, Range(0.0f, 10.0f)]
    private float m_WalkingSpeed = 7.0f;

    [SerializeField, Range(0.0f, 1.0f)]
    private float m_SideSpeedMultiply = 0.6f;

    [SerializeField, Range(0.0f, 10.0f)]
    private float m_SlideSpeed = 5.0f;

    [SerializeField, Range(0.0f, 3.0f)]
    private float m_SlidingStateChangeDelay = 0.3f;

    [SerializeField, Range(0.0f, 10.0f)]
    private float m_RunningSpeedMultiply = 1.6f;

    [SerializeField, Range(0.0f, 10.0f)]
    private float m_SlidingSpeedMultiply = 0.4f;

    [SerializeField, Range(0.0f, 10.0f)]
    private float m_JumpStrength = 7.0f;

    [SerializeField, Range(0.0f, 10.0f)]
    private float m_InAirSpeedMultiply = 0.05f;

    [SerializeField, Range(0.0f, 10.0f)]
    private float m_MaxMoveSpeedInAir = 3.0f;

    [SerializeField, Range(0.0f, 10.0f)]
    private float m_Gravity = 0.5f;


    [Space(10)]

    [SerializeField]
    private bool m_IsGrounded = true;

    [SerializeField]
    private bool m_CharacterControllerGrounded = false;

    [SerializeField]
    private bool m_IsOnSlope = false;

    [SerializeField]
    private bool m_IsSliding = false;

    [SerializeField]
    private LayerMask m_GroundCollideMask;

    [SerializeField]
    private Vector3 m_GroundCheckColliderScale = new Vector3(0.5f, 0.1f, 0.5f);
    
    [Space(15)]

    [SerializeField]
    private CharacterController m_CharacterController;

    [SerializeField]
    private AudioSource m_AudioSource;

    [SerializeField]
    private Transform m_GroundedChecker;

    [SerializeField]
    private FirstPerson_camera m_Camera;

    [SerializeField, Range(0.0f, 3.0f)] 
    private float m_WalkingSoundDelay = 0.5f;

    [SerializeField]
    private AudioClip[] m_WalkingSounds;

    [SerializeField]
    private AudioClip m_JumpSound;

    [SerializeField]
    private AudioClip m_LandingSound;

    [Space(15), Header("Debuging Variable")]

    [SerializeField]
    private bool m_DrawCircle = false;

    [SerializeField]
    private bool m_DrawCube = false;


    private bool m_IsWalkingSoundEnable = false;
    private float m_CurrentSpeed = 0.0f;
    private Vector3 m_SlopeNormal = new Vector3(0.0f, 1.0f, 0.0f);
    private bool m_IsSlidingDelay = false;
    private int m_WalkingSoundIndex = 0;
	
    private void Update()
    {
        m_CharacterControllerGrounded = m_CharacterController.isGrounded;
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_CurrentSpeed = m_WalkingSpeed * m_RunningSpeedMultiply;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            m_CurrentSpeed = m_WalkingSpeed;
        }

        Move();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_SlopeNormal = hit.normal;
        if (Vector3.Angle(Vector3.up, hit.normal) > m_CharacterController.slopeLimit && Vector3.Angle(Vector3.up, hit.normal) < 70)
        {
            m_IsOnSlope = true;
            StopAllCoroutines();
            StartCoroutine(ChangeSlidingState());
        }
        else
        {
            m_IsOnSlope = false;
            if (!m_IsSlidingDelay)
            {
                m_IsSliding = false;
            }
        }//Sliding Slope Check
    }

    private void Move()
    {
        Vector3 moveDirection = Vector3.zero;

        //Character Ground Check
        m_IsGrounded = Physics.CheckBox(m_GroundedChecker.position, m_GroundCheckColliderScale, transform.rotation, m_GroundCollideMask);

        //Rotation
        transform.rotation = Quaternion.Euler(0, m_Camera.CurrentYRotation, 0);

        // Sound
        if (!m_IsWalkingSoundEnable && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            StartCoroutine(PlayWalkingSound());
        }

        
        if (m_CharacterControllerGrounded && !m_IsSliding)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal") * m_CurrentSpeed * m_SideSpeedMultiply, 0, Input.GetAxis("Vertical") * m_CurrentSpeed);
            moveDirection = transform.TransformDirection(moveDirection);
        }//Moving in Ground
        else if(!m_CharacterControllerGrounded)
        {
            Vector3 moveDirectionInAir = new Vector3
                (
                Input.GetAxis("Horizontal") * m_CurrentSpeed * m_SideSpeedMultiply * m_InAirSpeedMultiply, 
                m_Gravity, 
                Input.GetAxis("Vertical") * m_CurrentSpeed * m_InAirSpeedMultiply
                );

            moveDirectionInAir = transform.TransformDirection(moveDirectionInAir);

            if (new Vector2(moveDirection.x, moveDirection.z).sqrMagnitude > Mathf.Pow(m_MaxMoveSpeedInAir,2.0f))
            {
                if (Mathf.Abs(moveDirection.x + moveDirectionInAir.x) < Mathf.Abs(moveDirection.x))
                {
                    moveDirection.x += moveDirectionInAir.x;
                    moveDirection.z += moveDirectionInAir.z;
                }
            }
            else
            {
                moveDirection.x += moveDirectionInAir.x;
                moveDirection.z += moveDirectionInAir.z;
            }
            moveDirection.y -= m_Gravity * Time.deltaTime;
        }//Moving in Air
        else if(m_CharacterControllerGrounded && m_IsSliding)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal") * m_CurrentSpeed * m_SideSpeedMultiply * m_SlidingSpeedMultiply, -5, Input.GetAxis("Vertical") * m_CurrentSpeed * m_SlidingSpeedMultiply);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection.x += m_SlopeNormal.x * m_SlideSpeed;
            moveDirection.z += m_SlopeNormal.z * m_SlideSpeed;
        }//Moving in Sliding

        if (m_IsGrounded && Input.GetButtonDown("Jump"))
        {
            moveDirection.y += m_JumpStrength;
            PlayJumpSound();
        }//Jump


        m_CharacterController.Move(moveDirection * Time.deltaTime);


        //On Moving Platform
        var colliders = Physics.OverlapSphere(m_GroundedChecker.position, m_GroundCheckColliderScale.y);
        bool isGroundedOn_Platform = false;
        foreach (Collider col in colliders)
        {
            if (col.GetComponent<Platform_Ver2>() != null)
            {
                isGroundedOn_Platform = true;
                transform.parent = col.transform;
            }
        }
        if (!isGroundedOn_Platform)
        {
            transform.parent = null;
        }
    }

    private void PlayJumpSound()
    {
        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.Play();
    }

    private void PlayFootStepAudio()
    {
        if (!m_IsGrounded)
        {
            return;
        }

        m_WalkingSoundIndex = ++m_WalkingSoundIndex % m_WalkingSounds.Length;
        m_AudioSource.clip = m_WalkingSounds[m_WalkingSoundIndex];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
    }

    private IEnumerator PlayWalkingSound()
    {
        m_IsWalkingSoundEnable = true;
        PlayFootStepAudio();
        yield return new WaitForSeconds(m_WalkingSoundDelay);
        m_IsWalkingSoundEnable = false;
    }

    private IEnumerator ChangeSlidingState()
    {
        m_IsSlidingDelay = true;
        yield return new WaitForSeconds(m_SlidingStateChangeDelay);
        m_IsSlidingDelay = false;
    }

    void OnDrawGizmos()
    {
        if (m_DrawCircle)
        {
            Gizmos.DrawSphere(m_GroundedChecker.transform.position, m_GroundCheckColliderScale.x);
        }
        if (m_DrawCube)
        {
            Gizmos.DrawCube(m_GroundedChecker.transform.position, m_GroundCheckColliderScale);
        }
    }
}
