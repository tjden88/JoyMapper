using System.Collections.Generic;
using Newtonsoft.Json;

namespace JoyMapper.Models.Legacy.v1_3.Models;

internal abstract class JoyActionBase
{
    /// <summary> Описание дествия </summary>
    [JsonIgnore]
    public abstract string Description { get; }
}


internal class SimpleButtonJoyAction : JoyActionBase
{
    public JoyButton Button { get; set; }

    public List<KeyboardKeyBinding> PressKeyBindings { get; set; } = new();

    public List<KeyboardKeyBinding> ReleaseKeyBindings { get; set; } = new();

    public override string Description => Button.ToString();

}


internal class ExtendedButtonJoyAction : JoyActionBase
{
    public JoyButton Button { get; set; }

    public List<KeyboardKeyBinding> SinglePressKeyBindings { get; set; } = new();
    public List<KeyboardKeyBinding> DoublePressKeyBindings { get; set; } = new();
    public List<KeyboardKeyBinding> LongPressKeyBindings { get; set; } = new();

    public override string Description => Button.ToString();

}


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

    public List<KeyboardKeyBinding> OnRangeKeyBindings { get; set; } = new();

    public List<KeyboardKeyBinding> OutOfRangeKeyBindings { get; set; } = new();

    public override string Description => "Ось " + Axis;

}