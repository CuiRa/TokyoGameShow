using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif

public class PlayerSelect : MonoBehaviour
{

    [SerializeField]
    private List<GameObject> Player;

    //現在の鬼を取得
    public int Ogre = 0;

    //プレイヤーの人数
    public int ActivePlayer;

    //乱数取得
    private int RandomPar;

    //Shuffleフラグ
    private bool ShuffleF = true;

    //Script破棄用フラグ
    private bool DestroyScript = false;

    //現在のScene名を取得
    private string CurrentScene = null;

    //Scriptが実行された最初のScene名を取得
    private string FastScene = null;




    float time;


    void Start()
    {
        FastScene = SceneManager.GetActiveScene().name;
        DontDestroyOnLoad(this);
        ActivePlayer = 1;
    }



    void Update()
    {
        CurrentScene = SceneManager.GetActiveScene().name;

        //参加人数の決定
        if (Input.GetKeyDown(KeyCode.Space) && ActivePlayer < 4)
        {
            Player[ActivePlayer].transform.localScale = new Vector3(20, 20, 1);
            ActivePlayer++;
        }


        if (Input.GetKeyDown(KeyCode.S) && ActivePlayer > 1)
        {
            if (ShuffleF)
            {
                //乱数で鬼を決定
                RandomPar = Random.Range(0, ActivePlayer);
                for (int i = 0; i < ActivePlayer; i++)
                {
                    if (i == RandomPar)
                    {
                        Player[i].GetComponent<SpriteRenderer>().color = new Color(0, 0, 255);
                        Ogre = i;
                    }
                    else
                        Player[i].GetComponent<SpriteRenderer>().color = new Color(0, 255, 0);

                }
                //Debug.Log(RandomPar);
                ShuffleF = false;
            }
        }

        //デモ用
        if (!ShuffleF)
        {
            time += Time.deltaTime;
            if (time > 1)
            {
                SceneManager.LoadScene("UI");
            }
            if (CurrentScene == "UI")
                time = 0;
        }


        if (FastScene != CurrentScene)
            DestroyScript = true;

        if (DestroyScript && FastScene == CurrentScene)
            Destroy(gameObject);
    }








    /* ---- ここからInspector拡張コード ---- */
#if UNITY_EDITOR
    //　カスタマイズするクラスを設定
    [CustomEditor(typeof(PlayerSelect))]
    //　Editorクラスを継承してクラスを作成
    public class MyDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //DefaultInspectorの表示
            DrawDefaultInspector();


            //PlayerSelectの参照
            PlayerSelect PlayerSel = target as PlayerSelect;


            //Playerのカウント用、要素数のカウント
            int PlayerCount, Playerlen = PlayerSel.Player.Count;



            //このスクリプトにアタッチされる各ObjectのInspector拡張
            
            //Player[]の表示
            for (PlayerCount = 0; PlayerCount < Playerlen; ++PlayerCount)
            {
                PlayerSel.Player[PlayerCount] = EditorGUILayout.ObjectField("Player" + (PlayerCount + 1), PlayerSel.Player[PlayerCount], typeof(GameObject), true) as GameObject;
            }
            //Playerの追加(4人まで)
            if (PlayerSel.Player.Count <= 3)
            {
                GameObject Player = EditorGUILayout.ObjectField("Playerの追加(4人まで)", null, typeof(GameObject), true) as GameObject;
                if (Player != null)
                    PlayerSel.Player.Add(Player);
            }


            PlayerSel.Ogre = EditorGUILayout.IntField("鬼のプレイヤー", PlayerSel.Ogre);
            PlayerSel.ActivePlayer = EditorGUILayout.IntField("ゲーム参加人数", PlayerSel.ActivePlayer);


            if (GUILayout.Button("セーブ\n(押さないとアタッチの情報が保持されません)"))
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
    }

#endif
}