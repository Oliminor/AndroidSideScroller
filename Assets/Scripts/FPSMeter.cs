using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Source: https://forum.unity.com/threads/how-can-i-display-fps-on-android-device.386250/

public class FPSMeter : MonoBehaviour
{
    // Start is called before the first frame update
    private int FramesPerSec;
    private float frequency = 1.0f;
    private string fps;



    void Start()
    {
        StartCoroutine(FPS());

        Application.targetFrameRate = 60;
    }

    private IEnumerator FPS()
    {
        for (; ; )
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(frequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it

            fps = string.Format("FPS: {0}", Mathf.RoundToInt(frameCount / timeSpan));
        }
    }


    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width - 100, 10, 150, 20), fps);
    }
}