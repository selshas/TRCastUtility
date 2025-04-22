using System;
using UnityEngine.InputSystem;

public class InputCommand
{
    public LowLevelInputSystem.InputState state = LowLevelInputSystem.InputState.Idle;
    public uint lowLevelKeyCode = 0;

    public Action cb_trigger;
}
