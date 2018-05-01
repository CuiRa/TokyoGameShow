using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AKT_flag : MonoBehaviour
{

    public bool omega = false;

    public string tag;

    public Vector3 pos;

    void OnTriggerEnter(Collider other)
    {

        omega = true;
        tag = other.tag;
        pos = other.transform.position;

    }


    void OnTriggerExit(Collider other)
    {

        omega = false;
        tag = "";
        pos = Vector3.zero;

    }


}

	
