using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raycas: MonoBehaviour
{
    [SerializeField]
    private Health m_Health;

    // Use this for initialization
    void Start () {
 
    }
	
	// Update is called once per frame
	void Update () {
        GetInput();
    }


    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Circle"))
        {
            float distance = 2; // 飛ばす&表示するRayの長さ
            float duration = 2;   // 表示期間（秒）
            Vector3 center = new Vector3(Screen.width / 2, Screen.height / 2);
            Ray ray = Camera.main.ScreenPointToRay(center);
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
