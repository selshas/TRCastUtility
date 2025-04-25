using UnityEngine;
using UnityEngine.UI;
using static GlobalInputSystem;
using DeviceType = GlobalInputSystem.DeviceType;
using KeyCode = SharpHook.Native.KeyCode;

public class MinimapOverlay : UtilityAppBase
{
    public MinimapCanvas MinimapCanvas;
    public RawImage RawImg_MinimapOverlay;

    public override void InitializeInputs()
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Update()
    {
        base.Update();

        if (MinimapCanvas.RawImg_MinimapCanvas == null)
            return;

        if (RawImg_MinimapOverlay.texture == MinimapCanvas.RawImg_MinimapCanvas.texture)
            return;

        RawImg_MinimapOverlay.texture = MinimapCanvas.RawImg_MinimapCanvas.texture;
    }
}
