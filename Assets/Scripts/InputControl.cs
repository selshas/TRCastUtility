using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.InputSystem;


public class InputControl : MonoBehaviour
{
    static ApplicationState state;
    public enum eApplicationState : int
    {
        Noone = 0,
        CardTable = 1,
        DrawCanvas = 2
    }

    ApplicationState[] states = new ApplicationState[3];

    static GameObject gameObject_cardTable;
    static GameObject gameObject_cardSpawner;

    static GameObject gameObject_drawCanvas;
    static DrawCanvas drawCanvas;

    static CoverScreen coverScreen;

    public enum LLKBInput
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

    public enum LLKBEvent
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

    public static Dictionary<LLKBInput, Key> dict_lli2UnityKeyCode = new Dictionary<LLKBInput, Key>();
    List<Key> list_allowedInputs = new List<Key>();
    public static Dictionary<Key, InputState> dict_inputStates = new Dictionary<Key, InputState>();

    public static int HookProc(int nCode, IntPtr wParam, IntPtr lParam)
    {
        Win32API.KBDLLHOOKSTRUCT kb = Marshal.PtrToStructure<Win32API.KBDLLHOOKSTRUCT>(lParam);
        Debug.Log($"{kb.scanCode}, {wParam}, {lParam}");

        Key key;
        if (dict_lli2UnityKeyCode.TryGetValue((LLKBInput)kb.scanCode, out key))
        {
            if ((LLKBEvent)wParam == LLKBEvent.KeyDown || (LLKBEvent)wParam == LLKBEvent.SystemKeyDown)
            {
                if (dict_inputStates[key] == InputState.Pressed || dict_inputStates[key] == InputState.Hold)
                    dict_inputStates[key] = InputState.Hold;
                else
                    dict_inputStates[key] = InputState.Pressed;
            }
            else if ((LLKBEvent)wParam == LLKBEvent.KeyUp || (LLKBEvent)wParam == LLKBEvent.SystemKeyUp)
            {
                if (dict_inputStates[key] == InputState.Released)
                    dict_inputStates[key] = InputState.Idle;
                else
                    dict_inputStates[key] = InputState.Released;
            }
        }

        /*
        if (kb.scanCode == 30)
        {
            Win32API.BringWindowToTop(ApplicationSetup.proc.MainWindowHandle);
            Win32API.SetActiveWindow(ApplicationSetup.proc.MainWindowHandle);
            Win32API.AllowSetForegroundWindow(ApplicationSetup.proc.Id);
            Win32API.SetForegroundWindow(ApplicationSetup.proc.MainWindowHandle);
            Win32API.SetFocus(ApplicationSetup.proc.MainWindowHandle);
        }*/
        //Debug.Log($"{(LLKBInput)kb.scanCode}: {dict_inputStates[key]} / {dict_inputStates[Key.LeftAlt]}");
        state.Update();

        return Win32API.CallNextHookEx(ApplicationSetup.hhook_kbinput, nCode, wParam, lParam);
    }

    private void Awake()
    {
        dict_lli2UnityKeyCode.Add(LLKBInput.F1, Key.F1);
        dict_lli2UnityKeyCode.Add(LLKBInput.F2, Key.F2);
        dict_lli2UnityKeyCode.Add(LLKBInput.F3, Key.F3);
        dict_lli2UnityKeyCode.Add(LLKBInput.F4, Key.F4);
        dict_lli2UnityKeyCode.Add(LLKBInput.F5, Key.F5);
        dict_lli2UnityKeyCode.Add(LLKBInput.LAlt, Key.LeftAlt);
        dict_lli2UnityKeyCode.Add(LLKBInput.Home, Key.Home);
        dict_lli2UnityKeyCode.Add(LLKBInput.End, Key.End);
        dict_lli2UnityKeyCode.Add(LLKBInput.Insert, Key.Insert);
        dict_lli2UnityKeyCode.Add(LLKBInput.PageDown, Key.PageDown);
        dict_lli2UnityKeyCode.Add(LLKBInput.PageUp, Key.PageUp);
        dict_lli2UnityKeyCode.Add(LLKBInput.Tab, Key.Tab);


        states[(int)eApplicationState.Noone] = new ApplicationState();
        
        states[(int)eApplicationState.Noone].AddInputCmd(Key.F1, true, InputState.Pressed, () => {
            ApplicationSetup.interactionMode = !ApplicationSetup.interactionMode;
        });
        states[(int)eApplicationState.Noone].AddInputCmd(Key.F2, true, InputState.Pressed, ToggleCardTable);
        states[(int)eApplicationState.Noone].AddInputCmd(Key.F3, true, InputState.Pressed, ToggleCanvas);
        
        states[(int)eApplicationState.Noone].AddInputCmd(Key.Home, false, InputState.Pressed, () => {
            coverScreen.ShowTitleScreen();
        });
        states[(int)eApplicationState.Noone].AddInputCmd(Key.End, false, InputState.Pressed, () => {
            coverScreen.Disappear();
        });
        states[(int)eApplicationState.Noone].AddInputCmd(Key.Insert, false, InputState.Pressed, () => {
            coverScreen.ShowCyclicImages();
        });
        states[(int)eApplicationState.Noone].AddInputCmd(Key.PageUp, false, InputState.Pressed, () => {
            coverScreen.ShowWhiteScreen();
        });
        states[(int)eApplicationState.Noone].AddInputCmd(Key.PageDown, false, InputState.Pressed, () => {
            coverScreen.ShowBlackScreen();
        });
        

        states[(int)eApplicationState.CardTable] = new ApplicationState();
        states[(int)eApplicationState.CardTable].AddInputCmd(Key.F1, true, InputState.Pressed, () => {
            ApplicationSetup.interactionMode = !ApplicationSetup.interactionMode;
        });
        states[(int)eApplicationState.CardTable].AddInputCmd(Key.F2, true, InputState.Pressed, ToggleCardTable);
        states[(int)eApplicationState.CardTable].AddInputCmd(Key.F3, true, InputState.Pressed, ToggleCanvas);
        states[(int)eApplicationState.CardTable].AddInputCmd(Key.Home, false, InputState.Pressed, () => {
            coverScreen.ShowTitleScreen();
        });
        states[(int)eApplicationState.CardTable].AddInputCmd(Key.End, false, InputState.Pressed, () => {
            coverScreen.Disappear();
        });
        states[(int)eApplicationState.CardTable].AddInputCmd(Key.Insert, false, InputState.Pressed, () => {
            coverScreen.ShowCyclicImages();
        });
        states[(int)eApplicationState.CardTable].AddInputCmd(Key.PageUp, false, InputState.Pressed, () => {
            coverScreen.ShowWhiteScreen();
        });
        states[(int)eApplicationState.CardTable].AddInputCmd(Key.PageDown, false, InputState.Pressed, () => {
            coverScreen.ShowBlackScreen();
        });


        states[(int)eApplicationState.DrawCanvas] = new ApplicationState();
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.F1, true, InputState.Pressed, () => {
            ApplicationSetup.interactionMode = !ApplicationSetup.interactionMode;
        });
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.F2, true, InputState.Pressed, ToggleCardTable);
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.F3, true, InputState.Pressed, ToggleCanvas);
        
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.F1, false, InputState.Pressed, () => {
            drawCanvas.cursorMode = DrawCanvas.CursorMode.Pen;
            drawCanvas.color = Color.red;
        });
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.F2, false, InputState.Pressed, () => {
            drawCanvas.cursorMode = DrawCanvas.CursorMode.Pen;
            drawCanvas.color = Color.blue;
        });
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.F3, false, InputState.Pressed, () => {
            drawCanvas.cursorMode = DrawCanvas.CursorMode.Pen;
            drawCanvas.color = Color.yellow;
        });
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.F4, false, InputState.Pressed, () => {
            drawCanvas.cursorMode = DrawCanvas.CursorMode.Pen;
            drawCanvas.color = Color.white;
        });
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.F5, false, InputState.Pressed, () => {
          drawCanvas.Clear();
        });

        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.Tab, false, InputState.Pressed, () => {
            switch (drawCanvas.canvasMode)
            {
                case DrawCanvas.CanvasMode.Screen: drawCanvas.canvasMode = DrawCanvas.CanvasMode.Minimap; break;
                case DrawCanvas.CanvasMode.Minimap: drawCanvas.canvasMode = DrawCanvas.CanvasMode.Screen; break;
            }
        });

        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.Home, false, InputState.Pressed, () => {
            coverScreen.ShowTitleScreen();
        });
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.End, false, InputState.Pressed, () => {
            coverScreen.Disappear();
        });
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.Insert, false, InputState.Pressed, () => {
            coverScreen.ShowCyclicImages();
        });
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.PageUp, false, InputState.Pressed, () => {
            coverScreen.ShowWhiteScreen();
        });
        states[(int)eApplicationState.DrawCanvas].AddInputCmd(Key.PageDown, false, InputState.Pressed, () => {
            coverScreen.ShowBlackScreen();
        });
    }

    private void Start()
    {
        gameObject_cardTable   = GameObject.Find("CardTable");
        gameObject_cardSpawner = GameObject.Find("Canvas/Root/CardSpawner");
        
        gameObject_drawCanvas  = GameObject.Find("Canvas/Root/DrawCanvas");
        drawCanvas = gameObject_drawCanvas.GetComponent<DrawCanvas>();
        coverScreen = GameObject.Find("Canvas/Root/CoverScreen").GetComponent<CoverScreen>();

        list_allowedInputs.Add(Key.LeftAlt);
        list_allowedInputs.Add(Key.F1);
        list_allowedInputs.Add(Key.F2);
        list_allowedInputs.Add(Key.F3);
        list_allowedInputs.Add(Key.F4);
        list_allowedInputs.Add(Key.F5);
        list_allowedInputs.Add(Key.Home);
        list_allowedInputs.Add(Key.End);
        list_allowedInputs.Add(Key.PageDown);
        list_allowedInputs.Add(Key.PageUp);
        list_allowedInputs.Add(Key.Insert);
        list_allowedInputs.Add(Key.Tab);

        for (int i = 0; i < list_allowedInputs.Count; i++)
        {
            dict_inputStates[list_allowedInputs[i]] = InputState.Idle;
        }

        UpdateState();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Application.isFocused) return;

        for (int i = 0; i< list_allowedInputs.Count; i++)
        {
            Key key = list_allowedInputs[i];
            if (Keyboard.current[key].wasPressedThisFrame)
            {
                dict_inputStates[key] = InputState.Pressed;
            }
            else if (Keyboard.current[key].isPressed)
            {
                dict_inputStates[key] = InputState.Hold;
            }
            else if (Keyboard.current[key].wasReleasedThisFrame)
            {
                dict_inputStates[key] = InputState.Released;
            }
            else
            {
                dict_inputStates[key] = InputState.Idle;
            }
        }
        UpdateState();
        state.Update();
    }

    void ToggleCardTable()
    {
        gameObject_cardTable.SetActive(!gameObject_cardTable.activeSelf);
        gameObject_cardSpawner.SetActive(!gameObject_cardSpawner.activeSelf);

        UpdateState();
    }
    void ToggleCanvas()
    {
        gameObject_drawCanvas.SetActive(!gameObject_drawCanvas.activeSelf);

        UpdateState();
    }

    void UpdateState()
    {
        if (gameObject_drawCanvas.activeSelf)
            state = states[(int)eApplicationState.DrawCanvas];
        else if (gameObject_cardTable.activeSelf)
            state = states[(int)eApplicationState.CardTable];
        else
            state = states[(int)eApplicationState.Noone];
    }
}
