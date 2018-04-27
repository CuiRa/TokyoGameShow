using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {
    Vector3 position;
    Vector3 location;
    public bool OgreF = false;

    float time = 0;


    // Update is called once per frame
    void Update()
    {
        if (OgreF)
        {
            time += Time.deltaTime;

            transform.eulerAngles = location;

            if (Input.GetKey(KeyCode.W))
                transform.Translate(0.0f, 0.0f, 0.1f);

            if (Input.GetKey(KeyCode.A))
                location.y -= 2f;

            if (Input.GetKey(KeyCode.D))
                location.y += 2f;

            if (Input.GetKey(KeyCode.S))
                transform.Translate(0.0f, 0.0f, -0.1f);
        }
    }


    //当たり判定(鬼移し)
    void OnCollisionStay(Collision collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            if (OgreF && time > 1)
            {
                collider.gameObject.GetComponent<Test>().OgreF = true;
                OgreF = false;
                time = 0;
            }

        }
    }
}
