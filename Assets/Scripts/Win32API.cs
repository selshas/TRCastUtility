using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Drawing;
using UnityEngine.LightTransport;

public static class Win32API
{
    public struct Margin
    {
        public int cx_left;
        public int cx_right;
        public int cy_top;
        public int cy_bottom;
    }

    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    [DllImport("user32.dll")]
    public static extern IntPtr GetActiveWindow();
    [DllImport("Dwmapi.dll")]
    public static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margin margins);

    [DllImport("user32.dll")]
    public static extern uint SetWindowLongA(IntPtr hwnd, int index, long attr);

    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint flags);


    public struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;

        public UIntPtr dwExtraInfo;
    }

    public struct POINT
    {
        public int x;
        public int y;
    }

    public struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public int mouseData;
        public int flags;
        public int time;
        public IntPtr dwExtraInfo;
    }

    public delegate int HOOKPROC(int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowsHookEx(int idHook, HOOKPROC lpfn, IntPtr hmod, int dwThreadID);

    [DllImport("user32.dll")]
    public static extern bool UnhookWindowsHookEx(IntPtr hhook);

    [DllImport("user32.dll")]
    public static extern int CallNextHookEx(IntPtr hhook, int nCode, IntPtr wParam, IntPtr lParam);


    [DllImport("user32.dll")]
    public static extern bool SetActiveWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")]
    public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    public static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("user32.dll")]
    public static extern bool AllowSetForegroundWindow(int procId);

    [DllImport("user32.dll")]
    public static extern bool SetFocus(IntPtr hWnd);

    [DllImport("Kernel32.dll")]
    public static extern uint GetCurrentThreadId();
}
