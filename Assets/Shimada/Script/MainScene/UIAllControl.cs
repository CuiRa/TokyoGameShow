using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif

public class UIAllControl : MonoBehaviour
{
    //以下INspectorに表示
    //Canvasの取得、最初の要素([0])は必ず鬼用のCanvasを入れること
    [SerializeField]
    private List<GameObject> CanvasObject;

    //Canvasに割り当てる用のCameraの取得
    [SerializeField]
    private List<Camera> CameraObject;

    //前Sceneから鬼の取得
    [SerializeField]
    private int Ogre;

    [SerializeField]
    private int ActivePlayer;

    private CanvasControl[] CanvasObj = new CanvasControl[4];

    //汎用カウンター
    private int Counter = 0;



    void Start()
    {

        //前SceneでDestroyされなかったオブジェクトを取得
        GameObject Object = GameObject.FindGameObjectWithTag("Don'tDestroyObject");

        //鬼を取得
        Ogre = Object.GetComponent<PlayerSelect>().Ogre;
        //参加人数を取得
        ActivePlayer = Object.GetComponent<PlayerSelect>().ActivePlayer;

        //破棄
        Destroy(Object);


        //各CanbasのSliderControlの参照
        Counter = CanvasObject.Count - 1;
        bool CarriaPush = false;

        do
        {
            //CanvasCountrolの取得
            CanvasObj[Counter] = CanvasObject[Counter].GetComponent<CanvasControl>();

            //参加人数外のCanvasを見えなくする
            if (Counter >= ActivePlayer)
                CanvasObj[Counter].Black.SetActive(true);

            //Cameraの配置分け
            switch (Counter)
            {
                case 0:
                    if (Counter == Ogre)
                    {
                        CameraObject[0].rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                        CarriaPush = true;
                    }

                    else
                    {
                        //CanvasをCameraへ振り分け
                        if (CarriaPush)
                            CameraObject[Counter + 1].rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);

                        else
                            CameraObject[Counter].rect = new Rect(0.0f, 0.5f, 0.5f, 0.5f);
                    }
                    break;


                case 1:
                    if (Counter == Ogre)
                    {
                        CameraObject[0].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                        CarriaPush = true;
                    }

                    else
                    {
                        //CanvasをCameraへ振り分け
                        if (CarriaPush)
                            CameraObject[Counter + 1].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);

                        else
                            CameraObject[Counter].rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                    }
                    break;


                case 2:
                    if (Counter == Ogre)
                    {
                        CameraObject[0].rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                        CarriaPush = true;
                    }

                    else
                    {
                        //CanvasをCameraへ振り分け
                        if (CarriaPush)
                            CameraObject[Counter + 1].rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);

                        else
                            CameraObject[Counter].rect = new Rect(0.0f, 0.0f, 0.5f, 0.5f);
                    }
                    break;


                case 3:
                    if (Counter == Ogre)
                    {
                        CameraObject[0].rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);
                        CarriaPush = true;
                    }

                    else
                    {
                        //CanvasをCameraへ振り分け
                        if (CarriaPush)
                            CameraObject[Counter + 1].rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);

                        else
                            CameraObject[Counter].rect = new Rect(0.5f, 0.0f, 0.5f, 0.5f);

                    }
                    break;
            }


            //Playerのカウント
            if (Counter != Ogre)
            {
                if (CarriaPush)
                    CanvasObj[Counter + 1].Player.text = Counter + 1 + "P";

                else
                    CanvasObj[Counter].Player.text = Counter + 1 + "P";
            }

            Counter--;

        } while (Counter >= 0);

        Ogre += 1;
    }






    void Update()
    {
        //現在のプレイヤー数を取得
        Counter = ActivePlayer-1;
        
        //各CanbasのSliderControlの参照
        do
        {
            //CanvasControlを取得し、変数に代入
            CanvasObj[Counter] = CanvasObject[Counter].GetComponent<CanvasControl>();

            //StaminaGageの増加
            CanvasObj[Counter].StaminaGage += 0.01f;
            CanvasObj[Counter].Stamina.value = CanvasObj[Counter].StaminaGage;

            // 最大を超えたら0に戻す
            if (CanvasObj[Counter].StaminaGage > 1)
                CanvasObj[Counter].StaminaGage = 0;


            Counter--;
            
        } while (Counter >= 0);
    }














    /* ---- ここからInspector拡張コード ---- */
#if UNITY_EDITOR

    //　カスタマイズするクラスを設定
    [CustomEditor(typeof(UIAllControl))]
    //　Editorクラスを継承してクラスを作成
    public class MyDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //UIAllControlの参照
            UIAllControl UICon = target as UIAllControl;

            //CanvasObjectのカウント用、要素数のカウント
            int CanCount, Canlen = UICon.CanvasObject.Count;


            //CameraObjectのカウント用、要素数のカウント
            int CamCount, Camlen = UICon.CameraObject.Count;


            //CanvasObjectのInspector拡張
            EditorGUILayout.PrefixLabel("Canvasオブジェクト");
            // リスト表示
            for (CanCount = 0; CanCount < Canlen; ++CanCount)
            {
                UICon.CanvasObject[CanCount] = EditorGUILayout.ObjectField(UICon.CanvasObject[CanCount], typeof(GameObject), true) as GameObject;
            }
            if (UICon.CanvasObject.Count <= 3)
            {
                GameObject Can = EditorGUILayout.ObjectField("追加", null, typeof(GameObject), true) as GameObject;
                if (Can != null)
                    UICon.CanvasObject.Add(Can);
            }


            //CameraObjectのInspector拡張
            EditorGUILayout.PrefixLabel("Cameraオブジェクト");
            // リスト表示
            for (CamCount = 0; CamCount < Camlen; ++CamCount)
            {
                UICon.CameraObject[CamCount] = EditorGUILayout.ObjectField(UICon.CameraObject[CamCount], typeof(Camera), true) as Camera;
            }
            if (UICon.CameraObject.Count <= 3)
            {
                Camera Cam = EditorGUILayout.ObjectField("追加", null, typeof(Camera), true) as Camera;
                if (Cam != null)
                    UICon.CameraObject.Add(Cam);
            }

            UICon.Ogre = EditorGUILayout.IntField("キャリアプレイヤー(鬼)", UICon.Ogre);
            UICon.ActivePlayer = EditorGUILayout.IntField("参加人数", UICon.ActivePlayer);


            if (GUILayout.Button("セーブ\n(押さないとアタッチの情報が保持されません)"))
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
    }
#endif

}