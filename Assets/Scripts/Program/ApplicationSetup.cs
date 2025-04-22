using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ApplicationSetup : MonoBehaviour
{
    public static ApplicationSetup instance = null;

    public static bool raycastDetected = false;

    public static bool interactionMode
    {
        get => _interactionMode; 
        set 
        {
            _interactionMode = value;

            if (!value) 
                Win32API.SetWindowLongA(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
    }
    private static bool _interactionMode = true;

    const int GWL_EXSTYLE = -20;

    const long WS_EX_LAYERED = 0x00080000;
    const long WS_EX_TRANSPARENT = 0x00000020L;

    public static IntPtr hhook_kbinput = IntPtr.Zero;
    public static IntPtr hhook_mbinput = IntPtr.Zero;
    public static IntPtr hWnd { get; private set; }

    public static System.Diagnostics.Process proc { get; private set; }

    GraphicRaycaster raycaster;
    public static List<RaycastResult> list_raycastResults = new List<RaycastResult>();

    // Start is called before the first frame update
    void Start()
    {
        instance ??= this;

        proc = System.Diagnostics.Process.GetCurrentProcess();

        Application.runInBackground = true;
        Application.targetFrameRate = 240;
        raycaster = GetComponent<GraphicRaycaster>();

#if !UNITY_EDITOR

        hWnd = Win32API.GetActiveWindow();

        Win32API.Margin margin = new Win32API.Margin { cx_left = -20 };
        Win32API.DwmExtendFrameIntoClientArea(hWnd, ref margin);

        Win32API.SetWindowLongA(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

        Win32API.SetWindowPos(hWnd, new IntPtr(-1), 0, 0, 0, 0, 0);
#endif
        InstallInputHooker();
    }

    public void Wakeup()
    {
        Win32API.BringWindowToTop(proc.MainWindowHandle);
        Win32API.SetActiveWindow(proc.MainWindowHandle);
        Win32API.AllowSetForegroundWindow(proc.Id);
        Win32API.SetForegroundWindow(proc.MainWindowHandle);
        Win32API.SetFocus(proc.MainWindowHandle);
    }

    // Update is called once per frame
    void Update()
    {
        if (!interactionMode) 
            return;

        Cursor.lockState = (Application.isFocused) 
            ? CursorLockMode.Confined
            : CursorLockMode.None;

        PointerEventData ped = new PointerEventData(null);
        ped.position = Mouse.current.position.ReadValue();

        raycaster.Raycast(ped, list_raycastResults);

        if (list_raycastResults.Count > 0 || raycastDetected)
        {
            // Bring window focus to the top
            Win32API.SetWindowLongA(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        }
        else
        {
            Win32API.SetWindowLongA(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
        list_raycastResults.Clear();
    }

    void InstallInputHooker()
    {
        if (hhook_kbinput == IntPtr.Zero)
        {
            hhook_kbinput = Win32API.SetWindowsHookEx(
                13, // WH_KEYBOARD_LL
                LoweLevelInputSystem.KbHookProc,
                IntPtr.Zero,
                0
            );
        }

        if (hhook_mbinput == IntPtr.Zero)
        {
            hhook_mbinput = Win32API.SetWindowsHookEx(
                14, // WH_MOUSE_LL
                LoweLevelInputSystem.MHookProc,
                IntPtr.Zero,
                0
            );
        }

        Debug.Log($"hhook_kbinput: {hhook_kbinput}");
        Debug.Log($"hhook_mbinput: {hhook_mbinput}");
    }

    void UninstallInputHooker()
    {
        Win32API.UnhookWindowsHookEx(hhook_kbinput);
        hhook_kbinput = IntPtr.Zero;

        Win32API.UnhookWindowsHookEx(hhook_mbinput);
        hhook_mbinput = IntPtr.Zero;
    }

    private void OnDestroy()
    {
        UninstallInputHooker();
    }
    ~ApplicationSetup()
    {
        UninstallInputHooker();
    }
}
