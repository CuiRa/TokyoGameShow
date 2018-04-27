using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door2 : MonoBehaviour {

    private bool area; //falesで閉
    private bool omega; //コライダーの判定
//    private bool aaa; //ストッパー

    public GameObject door;
    public float x = 0.0f;
    public float y = 0.0f;


	// Use this for initialization
	void Start () {

        area = false;
        omega = false;
//        aaa = false;

	}
	
	// Update is called once per frame
	void Update ()
    {
//		if(aaa == false)
//        {
            if (Input.GetKeyDown("space"))
            {
                if (omega)
                {
                    if(area == false)
                    {

                        door.gameObject.transform.Rotate(0, x, 0);
//                        aaa = true;
                        area = true;

                    }
                    else
                    {

                        door.gameObject.transform.Rotate(0, y, 0);
//                       aaa = true;
                        area = false;

                    }
                }
            }
//        }

	}

    void OnTriggerEnter(Collider other)
    {

        omega = true;
//        aaa = false;

    }

    void OnTriggerExit(Collider other)
    {

        omega = false;

    }

}
