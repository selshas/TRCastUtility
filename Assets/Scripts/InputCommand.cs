using System;
using UnityEngine.InputSystem;

public class InputCommand
{
    public InputControl.InputState state = InputControl.InputState.Idle;
    public Key key = 0;

    public Action cb_trigger;
}
