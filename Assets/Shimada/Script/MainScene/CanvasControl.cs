using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;      //!< デプロイ時にEditorスクリプトが入るとエラーになるので UNITY_EDITOR で括ってね！
#endif

public class CanvasControl : MonoBehaviour
{
    public Slider Stamina;
    

    public Text Player;

    //CanvasTwo,CanvasThreeのみ作成した暗幕用のImageをアタッチ
    public GameObject Black;


    //各PlayerのScriptをpublicで取得し、ステータスを取得して常にSliderに反映
    [System.NonSerialized]
    public float StaminaGage = 0;

    [System.NonSerialized]
    public float InfectionGage = 0;

    [System.NonSerialized]
    public float FungusGage = 0;










    
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
            CanvasCon.Stamina = EditorGUILayout.ObjectField("スタミナゲージ", CanvasCon.Stamina, typeof(Slider), true) as Slider;
            CanvasCon.Player = EditorGUILayout.ObjectField("テキスト", CanvasCon.Player, typeof(Text), true) as Text;

            EditorGUILayout.HelpBox("CanvasTwoとCanvasThreeのみ以下に暗幕用Imageをアタッチ", MessageType.Warning);

            CanvasCon.Black = EditorGUILayout.ObjectField("未参加Player暗幕", CanvasCon.Black, typeof(GameObject), true) as GameObject;

            if (GUILayout.Button("セーブ\n(押さないとアタッチの情報が保持されません)"))
                EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        }
    }
#endif
}