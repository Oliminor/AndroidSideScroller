using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public static PlayerSettings instance;

    [SerializeField] private int controllerType = 1;
    private float renderScale = 1.0f;
    private bool isOptionIsOn = false;
    private bool isBloomOn = false;
    private bool isHDROn = true;
    private int resolutionIndex = 0;
    private Vector2 nativeResolution; 


    public bool IsOptionIsOn { get { return isOptionIsOn; } set { isOptionIsOn = value; } }
    public int ResolutionIndex { get { return resolutionIndex; } set { resolutionIndex = value; } }
    public int ControllerType { get { return controllerType; } set { controllerType = value; } }
    public bool IsBloomOn { get { return isBloomOn; } set { isBloomOn = value; } }
    public bool IsHDROn { get { return isHDROn; } set { isHDROn = value; } }
    public float RenderScale { get { return renderScale; } set { renderScale = value; } }
    public Vector2 GetNativeResolution { get { return GetNativeResolution; } private set { } }

    void Awake()
    {
        nativeResolution.x = Screen.width;
        nativeResolution.y = Screen.height;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(this);
    }
}
