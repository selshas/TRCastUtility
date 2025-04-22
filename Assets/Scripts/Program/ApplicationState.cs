using System.Collections.Generic;
using System;

class ApplicationState
{
    public List<InputCommand> list_inputCmds = new List<InputCommand>();
    public List<InputCommand> list_inputCmds_alt = new List<InputCommand>();

    public void AddInputCmd(uint lowLevelKeyCode, bool alt, LoweLevelInputSystem.InputState state, Action action)
    {
        InputCommand inputCmd = new InputCommand();
        inputCmd.lowLevelKeyCode = lowLevelKeyCode;
        inputCmd.state = state;
        inputCmd.cb_trigger = action;

        if (alt)
            list_inputCmds_alt.Add(inputCmd);
        else
            list_inputCmds.Add(inputCmd);
    }

    public void Update()
    {
        if (
            LoweLevelInputSystem.dict_inputStates[(uint)LoweLevelInputSystem.LLKBInput.LAlt] == LoweLevelInputSystem.InputState.Hold 
            || LoweLevelInputSystem.dict_inputStates[(uint)LoweLevelInputSystem.LLKBInput.LAlt] == LoweLevelInputSystem.InputState.Pressed
        )
        {
            foreach (var inputCommand in list_inputCmds_alt)
            {
                uint keyCode = inputCommand.lowLevelKeyCode;

                if (LoweLevelInputSystem.dict_inputStates[keyCode] != inputCommand.state) 
                    continue;

                inputCommand.cb_trigger();
            }
        }
        else
        {
            foreach (var inputCommand in list_inputCmds)
            {
                uint keyCode = inputCommand.lowLevelKeyCode;

                if (LoweLevelInputSystem.dict_inputStates[keyCode] != inputCommand.state)
                    continue;

                inputCommand.cb_trigger();
            }
        }
    }
}
