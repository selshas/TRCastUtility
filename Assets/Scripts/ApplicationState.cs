using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

class ApplicationState
{
    public List<InputCommand> list_inputCmds = new List<InputCommand>();
    public List<InputCommand> list_inputCmds_alt = new List<InputCommand>();

    public void AddInputCmd(Key key, bool alt, InputControl.InputState state, Action action)
    {
        InputCommand inputCmd = new InputCommand();
        inputCmd.key = key;
        inputCmd.state = state;
        inputCmd.cb_trigger = action;

        if (alt)
            list_inputCmds_alt.Add(inputCmd);
        else
            list_inputCmds.Add(inputCmd);
    }

    public void Update()
    {
        if (InputControl.dict_inputStates[Key.LeftAlt] == InputControl.InputState.Hold || InputControl.dict_inputStates[Key.LeftAlt] == InputControl.InputState.Pressed)
        {
            for (int i = 0; i < list_inputCmds_alt.Count; i++)
            {
                InputCommand inputCommand = list_inputCmds_alt[i];
                Key key = inputCommand.key;
                
                if (InputControl.dict_inputStates[key] != inputCommand.state) continue;

                inputCommand.cb_trigger();
            }
        }
        else
        {
            for (int i = 0; i < list_inputCmds.Count; i++)
            {
                InputCommand inputCommand = list_inputCmds[i];
                Key key = inputCommand.key;

                if (InputControl.dict_inputStates[key] != inputCommand.state) continue;

                inputCommand.cb_trigger();
            }
        }
    }
}
