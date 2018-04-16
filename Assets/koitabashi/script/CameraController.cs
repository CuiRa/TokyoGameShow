using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform Target;
//    [SerializeField]
//    private float DistanceToPlayerM = 0f;    // カメラとプレイヤーとの距離[m]
//    [SerializeField]
//    private float SlideDistanceM = 0f;       // カメラを横にスライドさせる；プラスの時右へ，マイナスの時左へ[m]
    [SerializeField]
    private float HeightM = 1.2f;            // 注視点の高さ[m]
    [SerializeField]
    private float RotationSensitivity = 100f;// 感度

    // エディタで通常時のシェーダと透明用のシェーダを指定する
//    public Shader targetShader;
//    public Shader defaultShader; // 通常時
//    public Shader alphaShader; // 透明時

    void Start()
    {
        if (Target == null)
        {
            Debug.LogError("ターゲットが設定されていない");
            Application.Quit();
        }
    }

    void FixedUpdate()
    {
//        var rotX = Input.GetAxis("Horizontal2") * Time.deltaTime * RotationSensitivity;
        var rotY = -Input.GetAxis("Vertical2") * Time.deltaTime * RotationSensitivity;

        var lookAt = Target.position + Vector3.up * HeightM;

        // 回転
//        transform.RotateAround(lookAt, Vector3.up, rotX);
        // カメラがプレイヤーの真上や真下にあるときにそれ以上回転させないようにする
        if (transform.forward.y > 0.9f && rotY < 0)
        {
            rotY = 0;
        }
        if (transform.forward.y < -0.9f && rotY > 0)
        {
            rotY = 0;
        }
        transform.RotateAround(lookAt, transform.right, rotY);
/*
        // カメラとプレイヤーとの間の距離を調整 
        transform.position = lookAt - transform.forward * DistanceToPlayerM;

        // 注視点の設定
        transform.LookAt(lookAt);

        // カメラを横にずらして中央を開ける  tps
        transform.position = transform.position + transform.right * SlideDistanceM;


        float distance = 1.6f; // 飛ばす&表示するRayの長さ
        float duration = 0;   // 表示期間（秒）

        Ray ray = new Ray(transform.position, transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, duration, false);

        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit, distance))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hit.collider.tag == "wall")
            {
                // Ray が当たっている間は対象オブジェクトのシェーダを alphaShader にする
                Shader targetShader = hitObject.GetComponent<Renderer>().material.shader;
                if (targetShader != alphaShader)
                {
                    targetShader = alphaShader;
                    Debug.Log(hitObject);
                }
                else
                {
                    targetShader = defaultShader;
                }
            }

        }
        */

    }
    
}