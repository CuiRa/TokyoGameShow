using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;                //����Ă��邩
        [SerializeField] private float m_WalkSpeed;               //�������
        [SerializeField] private float m_RunSpeed;                //���鑬��
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;             //���s���̗h��
        [SerializeField] private float m_JumpSpeed;               //�W�����v�͂�...�ł����˂�
        [SerializeField] private float m_StickToGroundForce;      //���n�␳?
        [SerializeField] private float m_GravityMultiplier;       //�d�͕␳?
        [SerializeField] private MouseLook m_MouseLook;           //
        [SerializeField] private bool m_UseFovKick;               //
        [SerializeField] private FOVKick m_FovKick = new FOVKick();             //
        [SerializeField] private bool m_UseHeadBob;               //����̃J�����̗h��
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();             //
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();             //
        [SerializeField] private float m_StepInterval;            //���� �W�����v ���n��
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.


        [SerializeField] private Transform myself;                // ����
        [SerializeField] private float DistanceToPlayerM = 0f;    // �J�����ƃv���C���[�Ƃ̋���[m]
        [SerializeField] private float HeightM = 1.2f;            // �����_�̍���[m]
        [SerializeField] private float RotationSensitivity = 100f;// ���x
        [SerializeField] private GameObject _child;

        
        private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);

            if (myself == null)
            {
                Debug.LogError("�^�[�Q�b�g���ݒ肳��Ă��Ȃ�");
                Application.Quit();
            }
            _child = transform.Find("FirstPersonCharacter").gameObject;


        }


        // Update is called once per frame
        private void Update()
        {
            RotateView();   //���_
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump && !m_Jumping)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;

            if(m_Jumping)
            {
                m_MoveDir.x = desiredMove.x * speed;
                m_MoveDir.z = desiredMove.z * speed;
            }
            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal1P1");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical1P1");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
//            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
            m_IsWalking = !Input.GetButton("DashP1");     //����
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()      //�J��������
        {
            //            m_MouseLook.LookRotation (transform, m_Camera.transform);   //�}�E�X�Ŏ��_�ύX
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal2P1") * Time.deltaTime * RotationSensitivity;
            float vertical = -CrossPlatformInputManager.GetAxis("Vertical2P1") * Time.deltaTime * RotationSensitivity;
            var lookAt = myself.position + Vector3.up * HeightM;
           
            transform.RotateAround(lookAt, Vector3.up, horizontal);    // ��]
            // �J�������v���C���[�̐^���^���ɂ���Ƃ��ɂ���ȏ��]�����Ȃ��悤�ɂ���
            if (_child.transform.forward.y > 0.9f && vertical < 0)
            {
                vertical = 0;
            }
            if (_child.transform.forward.y < -0.9f && vertical > 0)
            {
                vertical = 0;
            }
            _child.transform.RotateAround(lookAt, transform.right, vertical);

            // �J�����ƃv���C���[�Ƃ̊Ԃ̋����𒲐� 
            _child.transform.position = lookAt - _child.transform.forward * DistanceToPlayerM;

            // �����_�̐ݒ�
            _child.transform.LookAt(lookAt);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
