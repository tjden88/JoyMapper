using System;
using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Models.JoyBindings;

/// <summary>
/// Биндинг оси джойстика
/// </summary>
internal class AxisJoyBinding : JoyBindingBase
{
    public enum Axises
    {
        X,
        Y,
        Z,
        Rx,
        Ry,
        Rz,
        Slider1,
        Slider2,
    }

    public Axises Axis { get; set; }

    public int StartValue { get; set; }

    public int EndValue { get; set; }

    protected override bool IsPressed(JoyState joyState)
    {
        var value = Axis switch
        {
            Axises.X => joyState.AxisValues.X,
            Axises.Y => joyState.AxisValues.Y,
            Axises.Z => joyState.AxisValues.Z,
            Axises.Rx => joyState.AxisValues.Rx,
            Axises.Ry => joyState.AxisValues.Ry,
            Axises.Rz => joyState.AxisValues.Rz,
            Axises.Slider1 => joyState.AxisValues.Slider1,
            Axises.Slider2 => joyState.AxisValues.Slider2,
            _ => throw new ArgumentOutOfRangeException(nameof(Axis))
        };

        return value >= StartValue && value <= EndValue;
    }

    public override string Description => $"Ось {Axis}";
}