using System;
using System.Diagnostics;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;

namespace JoyMapper.Services.ActionWatchers;

internal class AxisActionWatcher : ActionWatcherBase
{
    private readonly AxisJoyAction _AxisJoyAction;

    /// <summary> Текущее значение </summary>
    public int CurrentValue { get; private set; }

    public AxisActionWatcher(AxisJoyAction axisJoyAction)
    {
        _AxisJoyAction = axisJoyAction;
    }

    public override JoyActionBase JoyAction => _AxisJoyAction;

    public override void Poll(JoyState joyState, bool SendCommands)
    {
        int axisValue;
        bool axisState;
        switch (_AxisJoyAction.Axis)
        {
            case AxisJoyAction.Axises.X:
                axisValue = joyState.AxisValues.X;
                axisState = axisValue >= _AxisJoyAction.StartValue &&
                            axisValue <= _AxisJoyAction.EndValue;
                break;
            case AxisJoyAction.Axises.Y:
                axisValue = joyState.AxisValues.Y;
                axisState = axisValue>= _AxisJoyAction.StartValue &&
                            axisValue<= _AxisJoyAction.EndValue;
                break;
            case AxisJoyAction.Axises.Z:
                axisValue = joyState.AxisValues.Z;
                axisState = axisValue>= _AxisJoyAction.StartValue &&
                            axisValue<= _AxisJoyAction.EndValue;
                break;
            case AxisJoyAction.Axises.Rx:
                axisValue = joyState.AxisValues.Rx;
                axisState = axisValue >= _AxisJoyAction.StartValue &&
                            axisValue <= _AxisJoyAction.EndValue;
                break;
            case AxisJoyAction.Axises.Ry:
                axisValue = joyState.AxisValues.Ry;
                axisState = axisValue >= _AxisJoyAction.StartValue &&
                            axisValue <= _AxisJoyAction.EndValue;
                break;
            case AxisJoyAction.Axises.Rz:
                axisValue = joyState.AxisValues.Rz;
                axisState = axisValue >= _AxisJoyAction.StartValue &&
                            axisValue <= _AxisJoyAction.EndValue;
                break;
            case AxisJoyAction.Axises.Slider1:
                axisValue = joyState.AxisValues.Slider1;
                axisState = axisValue >= _AxisJoyAction.StartValue &&
                            axisValue <= _AxisJoyAction.EndValue;
                break;
            case AxisJoyAction.Axises.Slider2:
                axisValue = joyState.AxisValues.Slider2;
                axisState = axisValue >= _AxisJoyAction.StartValue &&
                            axisValue <= _AxisJoyAction.EndValue;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        CurrentValue = axisValue;

        var isActive = IsActive;
        IsActive = axisState;
        if (isActive == axisState)
        {
            return;
        }

        OnActionHandled?.Invoke(axisState ? "Вход в диапазон оси" : "Выход из диапазона оси");

        if (!SendCommands)
        {
            return;
        }


        Debug.WriteLine(axisState ? "SendingOnRangeKeyBindings" : "SendingOutOfRangeKeyBindings");

        SendKeyboardCommands(axisState
            ? _AxisJoyAction.OnRangeKeyBindings
            : _AxisJoyAction.OutOfRangeKeyBindings);
    }
}