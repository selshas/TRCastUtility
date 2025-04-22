using System;
using UnityEngine.InputSystem;

public class InputCommand
{
    public LoweLevelInputSystem.InputState state = LoweLevelInputSystem.InputState.Idle;
    public uint lowLevelKeyCode = 0;

    public Action cb_trigger;
}
