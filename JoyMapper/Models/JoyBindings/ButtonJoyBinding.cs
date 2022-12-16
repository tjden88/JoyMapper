using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Models.JoyBindings;

/// <summary>
/// Биндинг обычной кнопки джойстика
/// </summary>
public class ButtonJoyBinding : JoyBindingBase
{

    /// <summary>
    /// Номер кнопки джойстика, начиная с 1
    /// </summary>
    public int ButtonNumber { get; set; }

    protected override bool IsPressed(JoyState joyState) => joyState.Buttons[ButtonNumber - 1];

    public override string Description => $"Кнопка {ButtonNumber}";
}