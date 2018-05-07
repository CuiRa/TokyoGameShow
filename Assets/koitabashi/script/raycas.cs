using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class raycas : MonoBehaviour
    {
        private Health m_Health;
        [SerializeField] private RigidbodyFirstPersonController m_RigidbodyFirstPersonController;
        [SerializeField] private Camera cam;
        [SerializeField] private string p_Num;

        // Use this for initialization
        void Start()
        {
            m_Health = transform.root.gameObject.GetComponent<Health>();
            m_RigidbodyFirstPersonController = transform.root.gameObject.GetComponent<RigidbodyFirstPersonController>();
            p_Num = m_RigidbodyFirstPersonController.movementSettings.PlayerNum;
        }

        // Update is called once per frame
        void Update()
        {
            GetInput();
        }


        private void GetInput()
        {
            if (Input.GetButtonDown("Circle" + p_Num) || Input.GetMouseButton(0))
            {
                float distance = 2; // 飛ばす&表示するRayの長さ
                float duration = 1;   // 表示期間（秒）
                Vector3 center = new Vector3(0.5f,0.5f);
                Ray ray = cam.ViewportPointToRay(center);
                Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, duration, false);

                RaycastHit hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit, distance))
                {
                    GameObject hitObject = hit.collider.gameObject;

                    if (hit.collider.tag == "Player")
                    {
                        m_Health = hitObject.GetComponent<Health>();
                        m_Health.TakeDamage(1);
                        Debug.Log("RayがPlayerに当たった");
                    }


                }
            }
        }
    }
}
