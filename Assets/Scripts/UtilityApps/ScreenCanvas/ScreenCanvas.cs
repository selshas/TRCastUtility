using SharpHook.Native;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering;

using static GlobalInputSystem;
using DeviceType = GlobalInputSystem.DeviceType;
using KeyCode = SharpHook.Native.KeyCode;

public partial class ScreenCanvas : UtilityAppBase
{
    public ScreenCanvasHelper Helper;

    public RawImage RawImg_ScreenCanvas = null;
    public Transform Transform_ScreenCanvas;

    private Vector2 cursorPos_curr;
    private Vector2 cursorPos_prev;

    private RectTransform rectTransform_targetCanvas;

    private Transform transform_emulatedCursor;
    public RawImage RawImage_EmulatedCursor;

    private RenderTexture renderTex_canvasTexture;
    public Material Mat_DrawSpot;
    public Material Mat_EraseSpot;

    public Dictionary<string, Color> PenColors = new Dictionary<string, Color>()
    {
        { "white",  new Color(1, 1, 1, 1) },
        { "red",  new Color(1, 0, 0, 1) },
        { "green",  new Color(0, 1, 0, 1) },
        { "blue",  new Color(0, 0, 1, 1) },
        { "yellow",  new Color(1, 1, 0, 1) },
    };

    public Color CurrentPenColor
    {
        get => currentPenColor;
        set
        {
            currentPenColor = value;
            RawImage_EmulatedCursor.color = value;
        }
    }
    private Color currentPenColor;

    protected void Awake()
    {
        currentPenColor = PenColors["white"];

        transform_emulatedCursor = transform.Find("EmulatedCursor").transform;
        RawImage_EmulatedCursor = transform_emulatedCursor.GetComponent<RawImage>();
         
        Transform_ScreenCanvas = transform;
        rectTransform_targetCanvas = Transform_ScreenCanvas.GetComponent<RectTransform>();

        var renderTexture = new RenderTexture(Screen.width, Screen.height, 1, GraphicsFormat.R8G8B8A8_UNorm, 0);
        renderTex_canvasTexture = renderTexture;

        RawImg_ScreenCanvas ??= transform.Find("DrawableArea").GetComponent<RawImage>();
        RawImg_ScreenCanvas.texture = renderTex_canvasTexture;
    }

    private void OnEnable()
    {
        Clear();
    }

    private void OnDisable()
    {
    }

    protected override void Start()
    {
        base.Start();

        cursorPos_prev = cursorPos_curr = Pointer.current.position.ReadValue();

        Clear();

        RenderTexture.active = (RenderTexture)RawImg_ScreenCanvas.texture;
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        RenderTexture.active = null;

        Mat_DrawSpot.SetVector("_Color", CurrentPenColor);
    }

    private bool IsCursorInsideCanvas()
    {
        Vector2 cursorPos = cursorPos_curr;

        return (
            rectTransform_targetCanvas.anchoredPosition.x < cursorPos.x && cursorPos.x < rectTransform_targetCanvas.anchoredPosition.x + rectTransform_targetCanvas.rect.width * 0.5f
            && rectTransform_targetCanvas.anchoredPosition.y < cursorPos.y && cursorPos.y < rectTransform_targetCanvas.anchoredPosition.y + rectTransform_targetCanvas.rect.height * 0.5f
        );
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        cursorPos_prev = cursorPos_curr;
        cursorPos_curr = Pointer.current.position.ReadValue();
        transform_emulatedCursor.position = cursorPos_curr;
    }

    public void Clear()
    {
        RenderTexture.active = renderTex_canvasTexture;
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        RenderTexture.active = null;
    }

    public void DrawSpot(Vector2 pos)
    {
        pos -= rectTransform_targetCanvas.anchoredPosition;

        Mat_DrawSpot.SetVector("_CursorPos", pos);
        Mat_DrawSpot.SetVector("_Color", CurrentPenColor);

        Graphics.Blit(null, renderTex_canvasTexture, Mat_DrawSpot);
    }

    public void EraseSpot(Vector2 pos)
    {
        pos -= rectTransform_targetCanvas.anchoredPosition;

        Mat_EraseSpot.SetVector("_CursorPos", pos);

        Graphics.Blit(null, renderTex_canvasTexture, Mat_EraseSpot);
    }

    public override void InitializeInputs()
    {
        AddInputCmd(
            DeviceType.Mouse, (uint)MouseButton.Button1,
            InputState.Pressed,
            (self) =>
            {
                DrawSpot(cursorPos_curr);
            }
        );
        AddInputCmd(
            DeviceType.Mouse, (uint)MouseButton.Button1,
            InputState.Hold,
            (self) =>
            {
                Vector2 cursorPos_delta = (cursorPos_curr - cursorPos_prev);

                int steps = Mathf.RoundToInt(cursorPos_delta.magnitude / 4.0f);
                float dt = 1.0f / steps;

                for (float t = 0; t <= 1.0f; t += dt)
                    DrawSpot(cursorPos_delta * t + cursorPos_prev);
            }
        );

        AddInputCmd(
            DeviceType.Mouse, (uint)MouseButton.Button2,
            InputState.Pressed,
            (self) =>
            {
                Vector2 cursorPos_delta = (cursorPos_curr - cursorPos_prev);

                int steps = Mathf.RoundToInt(cursorPos_delta.magnitude / 4.0f);
                float dt = 1.0f / steps;

                for (float t = 0; t <= 1.0f; t += dt)
                    EraseSpot(cursorPos_delta * t + cursorPos_prev);
            }
        );
        AddInputCmd(
            DeviceType.Mouse, (uint)MouseButton.Button2,
            InputState.Hold,
            (self) =>
            {
                EraseSpot(cursorPos_curr);
            }
        );

        AddInputCmd(
            DeviceType.Mouse, (uint)MouseButton.Button3,
            InputState.Pressed,
            (self) =>
            {
                Clear();
            }
        );

        AddInputCmd(
            DeviceType.Keyboard, (uint)KeyCode.VcTab,
            InputState.Pressed,
            (self) =>
            {
                GlobalAppController.Instance.ToggleApp_ScreenCanvas();
                GlobalAppController.Instance.ToggleApp_MinimapCanvas();
            }
        );

        AddInputCmd(
            DeviceType.Keyboard, (uint)KeyCode.VcF7,
            InputState.Pressed,
            (self) =>
            {
                ToggleHelper();
            }
        );

        AddInputCmd(
            DeviceType.Keyboard, (uint)KeyCode.Vc1,
            InputState.Pressed,
            (self) =>
            {
                CurrentPenColor = PenColors["white"];
            }
        );
        AddInputCmd(
            DeviceType.Keyboard, (uint)KeyCode.Vc2,
            InputState.Pressed,
            (self) =>
            {
                CurrentPenColor = PenColors["red"];
            }
        );
        AddInputCmd(
            DeviceType.Keyboard, (uint)KeyCode.Vc3,
            InputState.Pressed,
            (self) =>
            {
                CurrentPenColor = PenColors["green"];
            }
        );
        AddInputCmd(
            DeviceType.Keyboard, (uint)KeyCode.Vc4,
            InputState.Pressed,
            (self) =>
            {
                CurrentPenColor = PenColors["blue"];
            }
        );
        AddInputCmd(
            DeviceType.Keyboard, (uint)KeyCode.Vc5,
            InputState.Pressed,
            (self) =>
            {
                CurrentPenColor = PenColors["yellow"];
            }
        );
    }

    private void ToggleHelper()
    {
        Helper.gameObject.SetActive(!Helper.gameObject.activeSelf);
    }
}
