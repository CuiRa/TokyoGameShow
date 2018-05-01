using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DebugDisplayLog : MonoBehaviour
{
    static public List<string> displayLog = new List<string>();

    private void Start()
    {
        this.oneSecondTime = 0f;
    }

    private void Update()
    {
        // FPS
        if (this.oneSecondTime >= 1f)
        {
            this.fps = this.fpsCounter;
            this.fixedFps = this.fixedFpsCounter;

            // reset
            this.fpsCounter = 0;
            this.fixedFpsCounter = 0;
            this.oneSecondTime = 0f;
        }
        else
        {

            this.fpsCounter++;
            this.oneSecondTime += Time.deltaTime;
        }

        // structure debug string
        this.debugString = "";
        int count = DebugDisplayLog.displayLog.Count;

        for (int i = 0; i < DebugDisplayLog.displayLog.Count; i++)
        {
            this.debugString += DebugDisplayLog.displayLog[i];
            this.debugString += "\n";
        }
        DebugDisplayLog.displayLog.Clear();
    }

    private void FixedUpdate()
    {
        this.fixedFpsCounter++;
    }

    private void OnGUI()
    {
        GUI.Label(
            new Rect(0f, 0f, Screen.width, Screen.height),
            "FPS: " + this.fps + "  FixedUpdate: " + this.fixedFps + "\n" + this.debugString);
    }

    // FPS
    private int fps;
    private int fpsCounter;

    // Fixed FPS
    private int fixedFps;
    private int fixedFpsCounter;

    // Debug Log
    private string debugString;

    // Timer
    private float oneSecondTime;
}