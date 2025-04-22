using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.InputSystem;


public class LowLevelInputSystem : MonoBehaviour
{
    public List<UtilityAppBase> UtilityApps;

    public enum eApplicationState : int
    {
        Noone = 0,
        CardTable = 1,
        DrawCanvas = 2
    }

    static GameObject gameObject_screenCanvas;

    static ScreenCanvas screenCanvas;
    static ScreenCanvas minimapCanvas;
    static CoverScreen coverScreen;

    public enum LLKBInput : uint
    { 
        LAlt = 56,

        F1 = 59,
        F2 = 60,
        F3 = 61,
        F4 = 62,
        F5 = 63,

        Insert = 82,
        Home = 71,
        End = 79,
        PageUp = 73,
        PageDown = 81,

        Tab = 15
    }

    public enum  LLMBInput : uint
    {
        Moved = 0x0200,

        LPressed = 0x0201,
        LReleased = 0x0202,

        RPressed = 0x0204,
        RReleased = 0x0205,

        MPressed = 0x0207,
        MReleased = 0x0208,
    }

    public enum LLKBEvent : uint
    { 
        KeyDown = 256,
        KeyUp = 257,
        SystemKeyDown = 0x104,
        SystemKeyUp = 0x0105
    }

    public enum InputState
    {
        Idle,
        Pressed,
        Hold,
        Released
    }

    List<uint> list_allowedInputs = new List<uint>();
    public static Dictionary<uint, InputState> dict_inputStates = new Dictionary<uint, InputState>();

    public static int KbHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        Win32API.KBDLLHOOKSTRUCT kb = Marshal.PtrToStructure<Win32API.KBDLLHOOKSTRUCT>(lParam);

#if UNITY_EDITOR
        Debug.Log($"{kb.scanCode}, {wParam}, {lParam}");
#endif

        var llKeyCode = kb.scanCode;
        if ((LLKBEvent)wParam == LLKBEvent.KeyDown || (LLKBEvent)wParam == LLKBEvent.SystemKeyDown)
        {
            if (dict_inputStates[llKeyCode] == InputState.Pressed || dict_inputStates[llKeyCode] == InputState.Hold)
                dict_inputStates[llKeyCode] = InputState.Hold;
            else
                dict_inputStates[llKeyCode] = InputState.Pressed;
        }
        else if ((LLKBEvent)wParam == LLKBEvent.KeyUp || (LLKBEvent)wParam == LLKBEvent.SystemKeyUp)
        {
            if (dict_inputStates[llKeyCode] == InputState.Released)
                dict_inputStates[llKeyCode] = InputState.Idle;
            else
                dict_inputStates[llKeyCode] = InputState.Released;
        }

        /*
        if (llKeyCode)
        {
            Win32API.BringWindowToTop(ApplicationSetup.proc.MainWindowHandle);
            Win32API.SetActiveWindow(ApplicationSetup.proc.MainWindowHandle);
            Win32API.AllowSetForegroundWindow(ApplicationSetup.proc.Id);
            Win32API.SetForegroundWindow(ApplicationSetup.proc.MainWindowHandle);
            Win32API.SetFocus(ApplicationSetup.proc.MainWindowHandle);
        }*/
        //Debug.Log($"{(LLKBInput)kb.scanCode}: {dict_inputStates[key]} / {dict_inputStates[Key.LeftAlt]}");

        return Win32API.CallNextHookEx(ApplicationSetup.hhook_kbinput, nCode, wParam, lParam);
    }

    public static int MHookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        Win32API.MSLLHOOKSTRUCT mb = Marshal.PtrToStructure<Win32API.MSLLHOOKSTRUCT>(lParam);

#if UNITY_EDITOR
        Debug.Log($"({mb.pt.x}, {mb.pt.y}), {wParam}, {lParam}");
#endif

        return Win32API.CallNextHookEx(ApplicationSetup.hhook_mbinput, nCode, wParam, lParam);
    }

    private void Start()
    {
        gameObject_screenCanvas = GameObject.Find("Canvas/Root/ScreenCanvas");
        screenCanvas = gameObject_screenCanvas.GetComponent<ScreenCanvas>();

        //gameObject_minimapCanvas = GameObject.Find("Canvas/Root/ScreenCanvas");
        //minimapCanvas = gameObject_minimapCanvas.GetComponent<ScreenCanvas>();

        coverScreen = GameObject.Find("Canvas/Root/CoverScreen").GetComponent<CoverScreen>();

        list_allowedInputs = new List<uint>()
        {
            (uint)LLMBInput.LPressed
        };

        for (int i = 0; i < list_allowedInputs.Count; i++)
        {
            dict_inputStates[list_allowedInputs[i]] = InputState.Idle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isFocused) 
            return;

        foreach (UtilityAppBase app in UtilityApps)
        {
            app.ProcessInput(ref this);
        }
    }
}
