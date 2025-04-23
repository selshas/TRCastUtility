using SharpHook.Native;
using System.Collections.Generic;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public abstract class UtilityAppBase : MonoBehaviour
{
    public List<InputCommand> inputCmds = new List<InputCommand>();

    protected virtual void Start()
    {
        InitializeInputs();
    }

    public void AddInputCmd(GlobalInputSystem.DeviceType deviceType, uint inputCode, GlobalInputSystem.InputState stateCondition, GlobalInputSystem.InputCallback action)
    {
        InputCommand inputCmd = new InputCommand(deviceType, inputCode, stateCondition, action);
        inputCmds.Add(inputCmd);

        var inputSys = GlobalInputSystem.Instance;
        if (!inputSys.RegisteredInput.TryGetValue(deviceType, out var inputs))
        {
            inputs = new List<uint>();
            inputSys.RegisteredInput.Add(deviceType, inputs);
            inputSys.InputStates.Add(deviceType, new Dictionary<uint, GlobalInputSystem.InputState>());
            inputSys.InputStates_prev.Add(deviceType, new Dictionary<uint, GlobalInputSystem.InputState>());

#if UNITY_EDITOR
            Debug.Log($"Device Added: {deviceType}");
#endif
        }

        if (!inputs.Contains(inputCode))
        {
            inputs.Add(inputCode);

            inputSys.InputStates[deviceType].Add(inputCode, GlobalInputSystem.InputState.Idle);
            inputSys.InputStates_prev[deviceType].Add(inputCode, GlobalInputSystem.InputState.Idle);
        }

#if UNITY_EDITOR
        Debug.Log($"{this}.AddInputCmd: {deviceType} {inputCode} {stateCondition}");
#endif
    }

    protected virtual void Update()
    {
        if (!gameObject.activeSelf)
            return;

        ProcessInput();
    }

    private void ProcessInput()
    {
        var inputSys = GlobalInputSystem.Instance;

        foreach (var inputCommand in inputCmds)
        {
            var keyCode = inputCommand.inputCode;
            var inputStates = inputSys.InputStates[inputCommand.DeviceType];
            if (!inputStates.TryGetValue(keyCode, out var state))
                continue;

            if (state != inputCommand.StateCondition)
                continue;

            inputCommand.Callback(this);
        }
    }

    public abstract void InitializeInputs();
}