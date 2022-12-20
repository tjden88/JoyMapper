using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Models.JoyBindings;

/// <summary>
/// Биндинг обычной кнопки джойстика
/// </summary>
public class ButtonJoyBinding : JoyBindingBase
{

    #region ButtonNumber : int - Номер кнопки джойстика, начиная с 1

    /// <summary>Номер кнопки джойстика, начиная с 1</summary>
    private int _ButtonNumber;

    /// <summary>Номер кнопки джойстика, начиная с 1</summary>
    public int ButtonNumber
    {
        get => _ButtonNumber;
        set => IfSet(ref _ButtonNumber, value).CallPropertyChanged(nameof(Description));
    }

    #endregion

    

    protected override bool IsPressed(JoyState joyState) => joyState.Buttons[ButtonNumber - 1];

    public override string Description => $"Кнопка {ButtonNumber}";
}