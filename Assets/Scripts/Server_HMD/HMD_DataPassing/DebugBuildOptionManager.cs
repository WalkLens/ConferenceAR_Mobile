using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugBuildOptionManager : MonoBehaviour
{
    public static DebugBuildOptionManager Instance;
    public BuildOptions buildOptions;
    
    private void Awake()
    {
        if(Instance == null) Instance = this;
        
        DontDestroyOnLoad(this);
        
        // 플랫폼별 실행 코드
#if UNITY_IOS || UNITY_ANDROID
            FileLogger.Log("📱 Running on Mobile (iOS or Android)", this);
            buildOptions = BuildOptions.Mobile;
#elif UNITY_WSA || UNITY_WINRT
            //FileLogger.Log("💻 Running on UWP (Windows Store App)", this);
            buildOptions = BuildOptions.HoloLens;
#elif UNITY_EDITOR
        Screen.SetResolution(1080, 1920, FullScreenMode.Windowed);
#endif
        
    }


    public enum BuildOptions
    {
        Mobile,
        HoloLens,
    }
}
