using UnityEngine;
using static GlobalInputSystem;
using DeviceType = GlobalInputSystem.DeviceType;
using KeyCode = SharpHook.Native.KeyCode;

public class GlobalAppConstroller : UtilityAppBase
{
    public ScreenCanvas App_ScreenCanvas;
    public CardTable App_CardTable;
    public MinimapCanvas App_MinimapCanvas;
    public ScreenCanvas App_PlayersPanel;
    public ScreenVeil App_ScreenVeil;

    public void ToggleApp_ScreenCanvas()
    {
        var isActive = App_ScreenCanvas.gameObject.activeSelf;
        App_ScreenCanvas.gameObject.SetActive(!isActive);
    }
    public void ToggleApp_CardTable()
    {
        var isActive = App_CardTable.gameObject.activeSelf;
        App_CardTable.gameObject.SetActive(!isActive);
    }
    public void ToggleApp_MinimapCanvas()
    {
        var isActive = App_MinimapCanvas.gameObject.activeSelf;
        App_MinimapCanvas.gameObject.SetActive(!isActive);
    }
    public void ToggleApp_PlayersPanel()
    {
        var isActive = App_PlayersPanel.gameObject.activeSelf;
        App_PlayersPanel.gameObject.SetActive(!isActive);
    }
    public void ToggleApp_ScreenVeil()
    {
        var isActive = App_ScreenCanvas.gameObject.activeSelf;
        App_ScreenVeil.gameObject.SetActive(!isActive);
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void InitializeInputs()
    {
        AddInputCmd(
            DeviceType.Keyboard, (uint)KeyCode.VcF1,
            InputState.Pressed,
            (self) =>
            {
                ToggleApp_ScreenCanvas();
                App_ScreenCanvas.Clear();
            }
        );
        AddInputCmd(
            DeviceType.Keyboard, (uint)KeyCode.VcF2,
            InputState.Pressed,
            (self) =>
            {
                ToggleApp_ScreenVeil();
            }
        );
    }
}
