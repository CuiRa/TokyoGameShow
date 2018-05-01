using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CanvasControl : MonoBehaviour
{
    //各Canvasを取得
    public GameObject NomalCanvas;
    public GameObject OgreCanvas;


    //Staminaスライダーを取得
    public Slider Stamina;

    //鬼用スライダーを取得
    public Slider Ogre;

    //Player表示を取得
    public Text Player;


    //CanvasTwo,CanvasThreeのみ作成した暗幕用のImageをアタッチ
    public GameObject Black;


    //各PlayerのScriptをpublicで取得し、ステータスを取得して常にSliderに反映
    //通常Canvas
    [System.NonSerialized]
    public float StaminaGage = 0;


    //鬼用Canvas
    [System.NonSerialized]
    public float OgreGage = 0;








    /* ---- ここからInspector拡張コード ---- */
#if UNITY_EDITOR

    //　カスタマイズするクラスを設定
    [CustomEditor(typeof(CanvasControl))]
    //　Editorクラスを継承してクラスを作成
    public class MyDataEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            //SliderControlの参照
            CanvasControl CanvasCon = target as CanvasControl;


            EditorGUILayout.PrefixLabel("Canvasオブジェクト");

            // リスト表示
            CanvasCon.NomalCanvas = EditorGUILayout.ObjectField("逃走用Canvasの取得", CanvasCon.NomalCanvas, typeof(GameObject), true) as GameObject;
            CanvasCon.OgreCanvas = EditorGUILayout.ObjectField("鬼用Canvasの取得", CanvasCon.OgreCanvas, typeof(GameObject), true) as GameObject;
            CanvasCon.Stamina = EditorGUILayout.ObjectField("スタミナゲージ", CanvasCon.Stamina, typeof(Slider), true) as Slider;
            CanvasCon.Ogre = EditorGUILayout.ObjectField("鬼ゲージ", CanvasCon.Ogre, typeof(Slider), true) as Slider;
            CanvasCon.Player = EditorGUILayout.ObjectField("テキスト", CanvasCon.Player, typeof(Text), true) as Text;

            EditorGUILayout.HelpBox("CanvasTwoとCanvasThreeのみ以下に暗幕用Imageをアタッチ", MessageType.Warning);

            CanvasCon.Black = EditorGUILayout.ObjectField("未参加Player暗幕", CanvasCon.Black, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("セーブ\n(押さないとアタッチの情報が保持されません)"))
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
    }
#endif
}