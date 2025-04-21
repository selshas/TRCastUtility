using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DrawCanvas : MonoBehaviour
{
    RawImage rawImg_screenCanvas = null;
    RawImage rawImg_minimapCanvas = null;

    Transform transform_screenCanvas;
    Transform transform_minimapCanvas;

    public enum CanvasMode
    {
        Screen, Minimap
    }

    public CanvasMode canvasMode
    {
        get => _canvasMode;
        set
        {
            _canvasMode = value;

            switch (value)
            {
                case CanvasMode.Screen:
                    {
                        transform_screenCanvas.gameObject.SetActive(true);
                        transform_minimapCanvas.gameObject.SetActive(false);

                        rectTransform_targetCanvas = transform_screenCanvas.GetComponent<RectTransform>();

                        renderTex_canvasTexture = (RenderTexture)rectTransform_targetCanvas.Find("DrawableArea").GetComponent<RawImage>().texture;

                        Clear();
                        break;
                    }
                case CanvasMode.Minimap:
                    {
                        transform_screenCanvas.gameObject.SetActive(false);
                        transform_minimapCanvas.gameObject.SetActive(true);
                        rectTransform_targetCanvas = transform_minimapCanvas.GetComponent<RectTransform>();

                        renderTex_canvasTexture = (RenderTexture)rectTransform_targetCanvas.Find("DrawableArea").GetComponent<RawImage>().texture;
                        break;
                    }
            }
        }
    }
    private CanvasMode _canvasMode;


    public enum CursorMode
    {
        Pen, Eraser
    }

    public CursorMode cursorMode 
    {
        get => _cursorMode;
        set
        {
            _cursorMode = value;

            switch (value)
            {
                case CursorMode.Pen:
                    {
                        rawImage_emulatedCursor.color   = color;
                        break;
                    }
                case CursorMode.Eraser:
                    {
                        rawImage_emulatedCursor.color   = Color.white;
                        break;
                    }
            }
        }
    }
    CursorMode _cursorMode = CursorMode.Pen;

    private Vector2 cursorPos_curr;
    private Vector2 cursorPos_prev;

    RectTransform rectTransform_targetCanvas;

    private Transform transform_emulatedCursor;
    public RawImage rawImage_emulatedCursor;

    RenderTexture renderTex_canvasTexture;
    public Material mat_drawSpot;
    public Material mat_eraseSpot;

    GameObject gameObject_icon_drawCanvas;
    enum PenColor
    {
        Black = 0,
        Red = 1,
        Green = 2,
        Blue = 3,
        White = 4
    }

    public Color color
    {
        get => _color;
        set
        {
            _color = value;
            rawImage_emulatedCursor.color = value;
        }
    }
    private Color _color = new Color(1,1,1,1);

    private void Awake()
    {
        transform_emulatedCursor = GameObject.Find("EmulatedCursor").transform;
        rawImage_emulatedCursor = transform_emulatedCursor.GetComponent<RawImage>();
        gameObject_icon_drawCanvas = GameObject.Find("Canvas/Root/ToolStatusIcons/Icon_DrawCanvas");

        transform_screenCanvas = transform.Find("ScreenCanvas");
        transform_minimapCanvas = transform.Find("MinimapCanvas");

        canvasMode = CanvasMode.Screen;
    }

    private void OnEnable()
    {
        gameObject_icon_drawCanvas.SetActive(true);
        if (canvasMode == CanvasMode.Screen)
            Clear();
    }
    private void OnDisable()
    {
        gameObject_icon_drawCanvas.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        rawImg_screenCanvas  = rawImg_screenCanvas  ?? transform.Find("ScreenCanvas/DrawableArea").GetComponent<RawImage>();
        rawImg_minimapCanvas = rawImg_minimapCanvas ?? transform.Find("MinimapCanvas/DrawableArea").GetComponent<RawImage>();

        cursorPos_prev = cursorPos_curr = Pointer.current.position.ReadValue();

        Clear();

        RenderTexture.active = (RenderTexture)rawImg_minimapCanvas.texture;
        GL.Clear(true, true, new Color(0, 0, 0, 0));
        RenderTexture.active = null;

        mat_drawSpot.SetVector("_Color", color);
    }

    // Update is called once per frame
    void Update()
    {
        cursorPos_prev = cursorPos_curr;
        cursorPos_curr = Pointer.current.position.ReadValue();
        transform_emulatedCursor.position = cursorPos_curr;

        if (
            rectTransform_targetCanvas.anchoredPosition.x < cursorPos_curr.x && cursorPos_curr.x < rectTransform_targetCanvas.anchoredPosition.x + rectTransform_targetCanvas.rect.width
        && rectTransform_targetCanvas.anchoredPosition.y < cursorPos_curr.y && cursorPos_curr.y < rectTransform_targetCanvas.anchoredPosition.y + rectTransform_targetCanvas.rect.height
        )
        {
            transform_emulatedCursor.gameObject.SetActive(true);
        }
        else
        {
            transform_emulatedCursor.gameObject.SetActive(false);
        }

        if (Mouse.current.leftButton.wasPressedThisFrame || Pen.current.press.wasPressedThisFrame)
        {
            cursorMode = CursorMode.Pen;
            cursorPos_prev = cursorPos_curr;
        }
        else if (Mouse.current.leftButton.isPressed || Pen.current.press.isPressed)
        {
            Vector2 cursorPos_delta = cursorPos_curr - cursorPos_prev;
            if (cursorPos_delta.magnitude > 2.0f)
            {
                int steps = Mathf.RoundToInt(cursorPos_delta.magnitude / 2.0f);
                float dt = 1.0f / steps;

                for (float t = 0; t <= 1.0f; t += dt)
                    DrawSpot(cursorPos_delta * t + cursorPos_prev);
            }
            else DrawSpot(cursorPos_curr);
        }
        else if (Mouse.current.rightButton.wasPressedThisFrame)
            cursorMode = CursorMode.Eraser;
        else if (Mouse.current.rightButton.wasReleasedThisFrame)
            cursorMode = CursorMode.Pen;
        else if (Mouse.current.rightButton.isPressed)
        {
            Vector2 cursorPos_delta = cursorPos_curr - cursorPos_prev;
            if (cursorPos_delta.magnitude > 2.0f)
            {
                int steps = Mathf.RoundToInt(cursorPos_delta.magnitude / 2.0f);
                float dt = 1.0f / steps;

                for (float t = 0; t <= 1.0f; t += dt)
                    EraseSpot(cursorPos_delta * t + cursorPos_prev);
            }
            else EraseSpot(cursorPos_curr);
        }
    }

    public void SetColor1P()
    {
        color = new Color(1, 0, 0, 1);
    }
    public void SetColor2P()
    {
        color = new Color(0.8420299f, 1, 0.04245281f, 1);
    }
    public void SetColor3P()
    {
        color = new Color(0.1843137f, 0.5562598f, 1, 1);
    }
    public void SetColor4P()
    {
        color = new Color(0.4374777f, 0.4524106f, 0.8207547f, 1);
    }
    public void SetColor5P()
    {
        color = new Color(0, 1, 0.8221006f, 1);
    }
    public void SetColor6P()
    {
        color = new Color(1, 0.8207309f, 0, 1);
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
        mat_drawSpot.SetVector("_CursorPos", pos);
        mat_drawSpot.SetVector("_Color", color);
        Graphics.Blit(null, renderTex_canvasTexture, mat_drawSpot);
    }

    public void EraseSpot(Vector2 pos)
    {
        pos -= rectTransform_targetCanvas.anchoredPosition;
        mat_eraseSpot.SetVector("_CursorPos", pos);
        Graphics.Blit(null, renderTex_canvasTexture, mat_eraseSpot);
    }
}
