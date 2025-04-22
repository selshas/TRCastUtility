using static GlobalInputSystem;

public class InputCommand
{
    public DeviceType DeviceType = DeviceType.None;
    public InputState StateCondition = InputState.Idle;
    public uint LowLevelKeyCode = 0;

    public InputCallback Callback;

    public InputCommand(DeviceType deviceType, uint llKeyCode, InputState stateCondition, InputCallback callback)
    {
        DeviceType = deviceType;
        LowLevelKeyCode = llKeyCode;

        StateCondition = stateCondition;

        Callback = callback;
    }
}
