using System;
using System.Diagnostics;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;

namespace JoyMapper.Services.ActionWatchers;

internal class AxisActionWatcher : ActionWatcherBase
{
    private readonly AxisJoyAction _AxisJoyAction;

    public AxisActionWatcher(AxisJoyAction axisJoyAction)
    {
        _AxisJoyAction = axisJoyAction;
    }

    public override JoyActionBase JoyAction => _AxisJoyAction;

    public override void Poll(JoyState joyState, bool SendCommands)
    {
        var axisState = _AxisJoyAction.Axis switch
        {
            AxisJoyAction.Axises.X => joyState.AxisValues.X >= _AxisJoyAction.StartValue &&
                                      joyState.AxisValues.X <= _AxisJoyAction.EndValue,
            AxisJoyAction.Axises.Y => joyState.AxisValues.Y >= _AxisJoyAction.StartValue &&
                                      joyState.AxisValues.Y <= _AxisJoyAction.EndValue,
            AxisJoyAction.Axises.Z => joyState.AxisValues.Z >= _AxisJoyAction.StartValue &&
                                      joyState.AxisValues.Z <= _AxisJoyAction.EndValue,
            AxisJoyAction.Axises.Rx => joyState.AxisValues.Rx >= _AxisJoyAction.StartValue &&
                                       joyState.AxisValues.Rx <= _AxisJoyAction.EndValue,
            AxisJoyAction.Axises.Ry => joyState.AxisValues.Ry >= _AxisJoyAction.StartValue &&
                                       joyState.AxisValues.Ry <= _AxisJoyAction.EndValue,
            AxisJoyAction.Axises.Rz => joyState.AxisValues.Rz >= _AxisJoyAction.StartValue &&
                                       joyState.AxisValues.Rz <= _AxisJoyAction.EndValue,
            AxisJoyAction.Axises.Slider1 => joyState.AxisValues.Slider1 >= _AxisJoyAction.StartValue &&
                                            joyState.AxisValues.Slider1 <= _AxisJoyAction.EndValue,
            AxisJoyAction.Axises.Slider2 => joyState.AxisValues.Slider2 >= _AxisJoyAction.StartValue &&
                                            joyState.AxisValues.Slider2 <= _AxisJoyAction.EndValue,
            _ => throw new ArgumentOutOfRangeException()
        };

        if (IsActive == axisState) return;

        IsActive = axisState;

        if(!SendCommands) return;

        Debug.WriteLine(axisState ? "SendingOnRangeKeyBindings" : "SendingOutOfRangeKeyBindings");

        SendKeyboardCommands(axisState
            ? _AxisJoyAction.OnRangeKeyBindings
            : _AxisJoyAction.OutOfRangeKeyBindings);
    }
}