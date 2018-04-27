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
    //Canvasの取得
    [SerializeField]
    private List<GameObject> CanvasObject;

    [SerializeField]
    private List<GameObject> OgreObject;

    //Canvasに割り当てる用のCameraの取得
    [SerializeField]
    private List<GameObject> CameraObject;

    [SerializeField]
    private List<GameObject> test;

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

        do
        {
            CanvasObj[Counter] = CanvasObject[Counter].GetComponent<CanvasControl>();

            if (Counter >= ActivePlayer)
            {
                CameraObject[Counter].transform.parent = null;
                test[Counter].SetActive(false);
                CanvasObj[Counter].Black.SetActive(true);
            }

            CanvasObj[Counter].Player.text = Counter + 1 + "P";

            Counter--;

        } while (Counter >= 0);

        //動作可能にする
        test[Ogre].GetComponent<Test>().OgreF = true;

        OgreObject[Ogre].SetActive(true);
        CanvasObject[Ogre].SetActive(false);



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

            //鬼が入れ替わっていた時のCanvasの入れ替え
            if (!test[Ogre].GetComponent<Test>().OgreF)
            {
                if (test[Counter].GetComponent<Test>().OgreF)
                {
                    OgreObject[Ogre].SetActive(false);
                    CanvasObject[Ogre].SetActive(true);

                    OgreObject[Counter].SetActive(true);
                    CanvasObject[Counter].SetActive(false);

                    Ogre = Counter;
                }
            }

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

            //DefaultInspectorの表示
            DrawDefaultInspector();


            //UIAllControlの参照
            UIAllControl UICon = target as UIAllControl;


            //CanvasObjectのカウント用、要素数のカウント
            int CanCount, Canlen = UICon.CanvasObject.Count;

            //CanvasObjectのカウント用、要素数のカウント
            int OgreCount, Ogrelen = UICon.OgreObject.Count;

            //CameraObjectのカウント用、要素数のカウント
            int CamCount, Camlen = UICon.CameraObject.Count;

            //CPlayerObjectのカウント用、要素数のカウント
            int PlayCount, Playlen = UICon.test.Count;


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


            EditorGUILayout.PrefixLabel("Ogreオブジェクト");
            // リスト表示
            for (OgreCount = 0; OgreCount < Ogrelen; ++OgreCount)
            {
                UICon.OgreObject[OgreCount] = EditorGUILayout.ObjectField(UICon.OgreObject[OgreCount], typeof(GameObject), true) as GameObject;
            }
            if (UICon.OgreObject.Count <= 3)
            {
                GameObject Ogr = EditorGUILayout.ObjectField("追加", null, typeof(GameObject), true) as GameObject;
                if (Ogr != null)
                    UICon.OgreObject.Add(Ogr);
            }


            //CameraObjectのInspector拡張
            EditorGUILayout.PrefixLabel("Cameraオブジェクト");
            // リスト表示
            for (CamCount = 0; CamCount < Camlen; ++CamCount)
            {
                UICon.CameraObject[CamCount] = EditorGUILayout.ObjectField(UICon.CameraObject[CamCount], typeof(GameObject), true) as GameObject;
            }
            if (UICon.CameraObject.Count <= 3)
            {
                GameObject Cam = EditorGUILayout.ObjectField("追加", null, typeof(GameObject), true) as GameObject;
                if (Cam != null)
                    UICon.CameraObject.Add(Cam);
            }


            //test(Prayer)のInspector拡張
            EditorGUILayout.PrefixLabel("Playerオブジェクト");
            // リスト表示
            for (PlayCount = 0; PlayCount < Playlen; ++PlayCount)
            {
                UICon.test[PlayCount] = EditorGUILayout.ObjectField(UICon.test[PlayCount], typeof(GameObject), true) as GameObject;
            }
            if (UICon.test.Count <= 3)
            {
                GameObject Tes = EditorGUILayout.ObjectField("追加", null, typeof(GameObject), true) as GameObject;
                if (Tes != null)
                    UICon.test.Add(Tes);
            }

            UICon.Ogre = EditorGUILayout.IntField("キャリアプレイヤー(鬼)", UICon.Ogre);
            UICon.ActivePlayer = EditorGUILayout.IntField("参加人数", UICon.ActivePlayer);


            if (GUILayout.Button("セーブ\n(押さないとアタッチの情報が保持されません)"))
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
    }
#endif

}