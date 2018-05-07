using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AKT_flag : MonoBehaviour
{

    public bool omega = false;

    public new string tag;

    public Vector3 pos;

    public GameObject obj;

    public GameObject lost;

    public Collider ogre;

    private int counter = 0;

    private bool sen = false;

    void Update()
    {

       if(sen)
        {
            counter++;
        }

        if (counter >= 300)
        {

            ogre.gameObject.GetComponent<NavMeshAgent>().speed = 5.0f;
            sen = false;
            counter = 0;
            ogre.tag = "Ogre";
            //攻撃トリガーアクティブ
        }

    }


    void OnTriggerEnter(Collider other)
    {
        if(obj.tag == "Ogre" && other.gameObject.tag == "Player")
        {
            Debug.Log("HIT");
            obj.tag = "Player";
            other.tag = "Unknown";
            //鬼になったやつを止める
            if (other.tag == "Unknown" && counter <= 300)
            {

                other.gameObject.GetComponent<NavMeshAgent>().speed = 0.0f;
                sen = true;
                ogre = other;
                //攻撃トリガー非アクティブ 
//                other.gameObject.GetComponent<AKT_flag>().
            }
        }
    }


    void OnTriggerExit(Collider other)
    {

        omega = false;
        tag = "";
        pos = Vector3.zero;

    }


}

	
