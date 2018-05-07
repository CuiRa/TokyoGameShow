using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour {

    public bool area = false;

    public string tag;

    public Vector3 pos;

    void OnTriggerEnter(Collider other)
    {

        area = true;
        tag = other.tag;
        pos = other.transform.position;

    }

    void OnTriggerExit(Collider other)
    {

        area = false;
        tag = "";//初期化
        pos = Vector3.zero;//初期化

    }
}
