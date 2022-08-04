using System.Collections.Generic;

namespace JoyMapper.Models.JoyActions;

/// <summary>
/// Действие джойстика, привязанное к интервалу оси контроллера
/// </summary>
internal class AxisJoyAction : JoyActionBase
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

    public List<KeyboardKeyBinding> OnRangeKeyBindings { get; set; }

    public List<KeyboardKeyBinding> OutOfRangeKeyBindings { get; set; }

    public override string Description => "Ось " + Axis;

}