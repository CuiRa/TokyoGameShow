using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {


    private bool area;   //falseで閉まった状態
    private bool omega; //コライダーの範囲内判定
    private bool aaa; //ストッパー


    public GameObject door;
    public float x = 0.0f;
    public float y = 0.0f; 


	// Use this for initialization
	void Start ()
    {

        area = false;
        omega = false;
        aaa = false;

	}
	
	// Update is called once per frame
	void Update ()
    {
        if (aaa == false)
        {
            if (Input.GetKeyDown("space"))
            {
                if (omega)
                {
                    if (area == false)
                    { 

                        door.gameObject.transform.Translate(x, 0, 0); //閉→開
                        aaa = true;
                        area = true;

                    }
                    else
                    {

                        door.gameObject.transform.Translate(y, 0, 0); //開→閉
                        aaa = true;
                        area = false;

                    }
                }
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {

        omega = true;
        aaa = false;

    }

    void OnTriggerExit(Collider col)
    {

        omega = false;
         
    }
}


//　ボタンを押すとドアの開閉
//　キャラが範囲内にいるかの確認
//　開いているか閉じてるかの判断