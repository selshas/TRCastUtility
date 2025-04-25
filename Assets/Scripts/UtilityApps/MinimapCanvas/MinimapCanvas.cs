using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.Rendering;
using SharpHook.Native;
using UnityEngine.InputSystem;

using static GlobalInputSystem;
using DeviceType = GlobalInputSystem.DeviceType;
using KeyCode = SharpHook.Native.KeyCode;
using UnityEditor;

public class MinimapCanvas : UtilityAppBase
{
    public MinimapCanvasHelper Helper;

    public RawImage RawImg_MinimapCanvas_Background;
    public RawImage RawImg_MinimapCanvas;
    public Transform Transform_MinimapCanvas;

    public Texture2D Tex_NotFound;

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


    public int CurrentPlayerCount = 2;

    public List<int> PlayerCounts = new List<int>()
    { 
        2,
        4,
        6,
    };

    public Dictionary<string, Texture2D> TexturesLookup = new Dictionary<string, Texture2D>();
    public List<Texture2D> Textures_minimap = new List<Texture2D>();

    public Dictionary<int, TMP_Dropdown> dropdowns_maps = new Dictionary<int, TMP_Dropdown>();

    public Transform Transform_DropdownGroup_Maps;

    void Awake()
    {
        #region Load and Init Minimap Textures
        foreach (var texture in Textures_minimap)
        {
            var mapName = texture.name.Trim();
            if (TexturesLookup.ContainsKey(mapName))
                continue;

#if UNITY_EDITOR
            Debug.Log($"Load Minimap Texture for: {mapName}");
#endif

            TexturesLookup.Add(mapName, texture);
        }

        foreach (var dropdown in Transform_DropdownGroup_Maps.GetComponentsInChildren<TMP_Dropdown>())
        {
            if (!int.TryParse(dropdown.gameObject.name, out var playerCount))
                continue;
            
            if (!PlayerCounts.Contains(playerCount))
                continue;

            dropdowns_maps.Add(playerCount, dropdown);
        }
        #endregion Load and Init Minimap Textures

        ChangePlayerCount(0);


        currentPenColor = PenColors["white"];

        transform_emulatedCursor = transform.Find("EmulatedCursor").transform;
        RawImage_EmulatedCursor = transform_emulatedCursor.GetComponent<RawImage>();

        Transform_MinimapCanvas = transform;
        rectTransform_targetCanvas = Transform_MinimapCanvas.GetComponent<RectTransform>();

        var renderTexture = new RenderTexture(Screen.width, Screen.height, 1, GraphicsFormat.R8G8B8A8_UNorm, 0);
        renderTex_canvasTexture = renderTexture;

        RawImg_MinimapCanvas ??= transform.Find("Mask/DrawableArea").GetComponent<RawImage>();
        RawImg_MinimapCanvas.texture = renderTex_canvasTexture;
    }

    protected override void Start()
    {
        base.Start();

        cursorPos_prev = cursorPos_curr = Pointer.current.position.ReadValue();

        Clear();

        RenderTexture.active = (RenderTexture)RawImg_MinimapCanvas.texture;
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        RenderTexture.active = null;

        Mat_DrawSpot.SetVector("_Color", CurrentPenColor);
    }

    private bool IsCursorInsideCanvas()
    {
        Vector2 cursorPos = cursorPos_curr;

        return (
            rectTransform_targetCanvas.anchoredPosition.x < cursorPos.x && cursorPos.x < rectTransform_targetCanvas.anchoredPosition.x + rectTransform_targetCanvas.rect.width
            && rectTransform_targetCanvas.anchoredPosition.y < cursorPos.y && cursorPos.y < rectTransform_targetCanvas.anchoredPosition.y + rectTransform_targetCanvas.rect.height
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
                GlobalAppController.Instance.ToggleApp_MinimapCanvas();
                GlobalAppController.Instance.ToggleApp_ScreenCanvas();
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

    public void ChangePlayerCount(int i)
    {
        var playerCount = PlayerCounts[i];

        CurrentPlayerCount = playerCount;

        foreach ((var key, var dropdown) in dropdowns_maps)
        {
            dropdown.gameObject.SetActive(key == playerCount);
        }

        ChangeMap(0);
    }

    public void ChangeMap(int i)
    {
        Clear();

        var texture_minimap = Tex_NotFound;

        var dropdown = dropdowns_maps[CurrentPlayerCount];
        if (dropdown.options.Count != 0 && dropdown.options[i].text != "")
        {
            var mapName = dropdown.options[i].text;

            texture_minimap = TexturesLookup[mapName];

#if UNITY_EDITOR
            Debug.Log($"ChangeMap: {mapName}");
#endif
        }

        RawImg_MinimapCanvas_Background.texture = texture_minimap;
    }
}
