using JoyMapper.Interfaces;

namespace JoyMapper.Models.JoyBindings;

/// <summary>
/// Биндинг обычной кнопки джойстика
/// </summary>
internal class ButtonJoyBinding : IJoyBinding
{
    public string JoyName { get; set; }

    /// <summary>
    /// Номер кнопки джойстика, начиная с 1
    /// </summary>
    public int ButtonNumber { get; set; }


    public bool IsActive(JoyState joyState) => joyState.Buttons[ButtonNumber + 1];


    public string Description => $"Кнопка {ButtonNumber}";
}

/// <summary>
/// Биндинг указателя вида
/// </summary>
internal class PowJoyBinding : IJoyBinding
{
    public string JoyName { get; set; }

    /// <summary>
    /// Номер кнопки джойстика, начиная с 1
    /// </summary>
    public int ButtonNumber { get; set; }


    public bool IsActive(JoyState joyState) => joyState.Buttons[ButtonNumber + 1];


    public string Description => $"Кнопка {ButtonNumber}";
}