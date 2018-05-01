using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (Rigidbody))]
    [RequireComponent(typeof (CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [Serializable]
        public class MovementSettings
        {
            public float ForwardSpeed = 8.0f;   // Speed when walking forward
            public float BackwardSpeed = 4.0f;  // Speed when walking backwards
            public float StrafeSpeed = 4.0f;    // Speed when walking sideways
            public float RunMultiplier = 2.0f;   // Speed when sprinting
            public float CrouchMultiplier = 0.5f;   // Speed when sprinting
            //	        public KeyCode RunKey = KeyCode.LeftShift;
            public float JumpForce = 30f;
            public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
            [HideInInspector] public float CurrentTargetSpeed = 8f;

            [SerializeField] private string playerNum;

#if !MOBILE_INPUT
            private bool m_Running;
            private bool crouching;
#endif

            public void UpdateDesiredTargetSpeed(Vector2 input)
            {
	            if (input == Vector2.zero) return;
				if (input.x > 0 || input.x < 0)
				{
					//strafe
					CurrentTargetSpeed = StrafeSpeed;
				}
				if (input.y < 0)
				{
					//backwards
					CurrentTargetSpeed = BackwardSpeed;
				}
				if (input.y > 0)
				{
					//forwards
					//handled last as if strafing and moving forward at the same time forwards speed should take precedence
					CurrentTargetSpeed = ForwardSpeed;
				}
#if !MOBILE_INPUT
                if (Input.GetButton("Crouch" + playerNum))
                {
                    CurrentTargetSpeed *= CrouchMultiplier;
//                    crouching = true;
                }
                else
                {
//                    crouching = false;
                }

                if (Input.GetButton("Dash" + playerNum))
                {
		            CurrentTargetSpeed *= RunMultiplier;
		            m_Running = true;
	            }
	            else
	            {
		            m_Running = false;
	            }
#endif
            }

#if !MOBILE_INPUT
/*            public bool Crouching
            {
                get { return crouching; }
            }*/
            public bool Running
            {
                get { return m_Running; }
            }
#endif
            public string PlayerNum
            {
                get { return playerNum; }
            }
        }
/// <summary>
/// ////+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
/// </summary>

        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }


        public Camera cam;
        public MovementSettings movementSettings = new MovementSettings();
//        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        Rigidbody m_RigidBody;
        CapsuleCollider m_Capsule;
        float m_CapsuleHeight;
        Vector3 m_CapsuleCenter;
//        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;
        private bool Crouching;        //しゃがむ
        private bool m_Crouching;     //しゃがみ続けるためのbool
        const float k_Half = 0.5f;     //半分


        [SerializeField]
        private Transform myself;                // 自分自身
        private GameObject _child;               //子(メインカメラ)
        [SerializeField] private float DistanceToPlayerM = 0f;    //playerとカメラの距離
        [SerializeField] private float HeightM = 0.6f;            // カメラの高さ
        private float m_HeightM;
        [SerializeField] private float RotationSensitivity = 100f;// カメラの旋回速度

        private string p_Num;

//        private Vector3 velocity;
        //　段差を昇る為のレイを飛ばす位置
        [SerializeField]
        private Transform stepRay;
        //　レイを飛ばす距離
        [SerializeField]
        private float stepDistance = 0.5f;
        //　昇れる段差
        [SerializeField]
        private float stepOffset = 0.3f;
        //　昇れる角度
        [SerializeField]
        private float slopeLimit = 65f;
        //　昇れる段差の位置から飛ばすレイの距離
        [SerializeField]
        private float slopeDistance = 1f;
        // ヒットした情報を入れる場所
        private RaycastHit stepHit;


        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }
/*
        public bool Crouching
        {
            get
            {
#if !MOBILE_INPUT
                return movementSettings.Crouching;
#else
	            return false;
#endif
            }
        }
        */
        public bool Running
        {
            get
            {
 #if !MOBILE_INPUT
				return movementSettings.Running;
#else
	            return false;
#endif
            }
        }


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            m_CapsuleHeight = m_Capsule.height;  ////////////
            m_CapsuleCenter = m_Capsule.center;  ////////////
            m_HeightM = HeightM;

//            velocity = Vector3.zero;

            //            mouseLook.Init (transform, cam.transform);
            if (myself == null)
            {
                Debug.LogError("私は誰");
                Application.Quit();
            }
            _child = transform.Find("MainCamera").gameObject;
            p_Num = movementSettings.PlayerNum;
            if (p_Num == null)
            {
                Debug.LogError("p_Num未入力");
                Application.Quit();
            }
        }


        private void Update()
        {
            RotateView();

            if (CrossPlatformInputManager.GetButtonDown("Jump" + p_Num) && !m_Jump)
            {
                m_Jump = true;
            }
        }


        private void FixedUpdate()
        {
            GroundCheck();
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward*input.y + cam.transform.right*input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                desiredMove.x = desiredMove.x*movementSettings.CurrentTargetSpeed;
                desiredMove.z = desiredMove.z*movementSettings.CurrentTargetSpeed;
                desiredMove.y = desiredMove.y*movementSettings.CurrentTargetSpeed;
                if (m_RigidBody.velocity.sqrMagnitude <
                    (movementSettings.CurrentTargetSpeed*movementSettings.CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove*SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, movementSettings.JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }

                ScaleCapsuleForCrouching();  //////////////////   しゃがみ時に判定を狭く
                PreventStandingInLowHeadroom();    /////////////////////// 上に障害物があるか
            }
            else
            {
                m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;


        }

        void ScaleCapsuleForCrouching()   //////////////////   しゃがみ時に判定を狭く
        {
            if (m_IsGrounded && Crouching)
            {
                if (m_Crouching) return;
                m_Capsule.height = m_Capsule.height / 2f;
                m_Capsule.center = m_Capsule.center / 2f;
                HeightM = HeightM / 2f;
                m_Crouching = true;
            }
            else
            {
                Ray crouchRay = new Ray(m_RigidBody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                    return;
                }
                m_Capsule.height = m_CapsuleHeight;
                m_Capsule.center = m_CapsuleCenter;
                HeightM = m_HeightM;
                m_Crouching = false;
            }
        }

        void PreventStandingInLowHeadroom()  /////////////////////// 上に障害物があるか
        {
            // prevent standing up in crouch-only zones
            if (!m_Crouching)
            {
                Ray crouchRay = new Ray(m_RigidBody.position + Vector3.up * m_Capsule.radius * k_Half, Vector3.up);
                float crouchRayLength = m_CapsuleHeight - m_Capsule.radius * k_Half;
                if (Physics.SphereCast(crouchRay, m_Capsule.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore))
                {
                    m_Crouching = true;
                }
            }
        }

        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return movementSettings.SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }


        private Vector2 GetInput()
        {
                Vector2 input = new Vector2
                {
                    x = CrossPlatformInputManager.GetAxis("Horizontal1" + p_Num),
                    y = CrossPlatformInputManager.GetAxis("Vertical1" + p_Num)
                };

            if (Input.GetButton("Crouch" + p_Num))
            {
                Crouching = true;
            }
            else
            {
                Crouching = false;
            }
            ///////////////////4月27日 段差///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //登れる段差を表示
            Debug.DrawLine(transform.position + new Vector3(0f, stepOffset, 0f), transform.position + new Vector3(0f, stepOffset, 0f) + transform.forward * slopeDistance, Color.green);
            
            //ステップ用のレイが地面に接触しているか
            if (Physics.Linecast(stepRay.position, stepRay.position + stepRay.forward * stepDistance, out stepHit, LayerMask.GetMask("Field")))
            {
                Debug.DrawRay(stepRay.position, stepRay.position + stepRay.forward * stepDistance, Color.blue, 0.1f, false);
                // 進行方向の地面の角度が指定以下、または登れる段差より下だった場合の移動

                if (Vector3.Angle(transform.up, stepHit.normal) <= slopeLimit
                        || (Vector3.Angle(transform.up, stepHit.normal) > slopeLimit
                            && !Physics.Linecast(transform.position + new Vector3(0f, stepOffset, 0f), transform.position + new Vector3(0f, stepOffset, 0f) + transform.forward * slopeDistance, LayerMask.GetMask("Field", "Block")) && m_IsGrounded)
                        )
                {
                    m_RigidBody.velocity = new Vector3(0f, ((Quaternion.FromToRotation(Vector3.up, stepHit.normal) * transform.forward) * 3f).y, 0f) + transform.forward * 1.5f;
                    Debug.Log(Vector3.Angle(transform.up, stepHit.normal));

                }
                else
                {
//                    movementSettings.UpdateDesiredTargetSpeed(input);
                }

                               Debug.Log(Vector3.Angle(transform.up, stepHit.normal));

                //ステップ用のレイが地面に接していなければ
            }
            else
            {
//                movementSettings.UpdateDesiredTargetSpeed(input);
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            movementSettings.UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()             //カメラ
        {
            //            m_MouseLook.LookRotation (transform, m_Camera.transform);   //
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal2" + p_Num) * Time.deltaTime * RotationSensitivity;
            float vertical = -CrossPlatformInputManager.GetAxis("Vertical2" + p_Num) * Time.deltaTime * RotationSensitivity;
            var lookAt = myself.position + Vector3.up * HeightM;

            transform.RotateAround(lookAt, Vector3.up, horizontal);    //
            // 
            if (_child.transform.forward.y > 0.9f && vertical < 0)
            {
                vertical = 0;
            }
            if (_child.transform.forward.y < -0.9f && vertical > 0)
            {
                vertical = 0;
            }
            _child.transform.RotateAround(lookAt, transform.right, vertical);

            //
            _child.transform.position = lookAt - _child.transform.forward * DistanceToPlayerM;

        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height/2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }
    }
}
