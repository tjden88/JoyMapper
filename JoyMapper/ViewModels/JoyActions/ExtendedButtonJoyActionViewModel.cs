using JoyMapper.Models;

namespace JoyMapper.ViewModels.JoyActions;

internal class ExtendedButtonJoyActionViewModel : JoyActionViewModelBase
{

    #region Button : JoyButton - Кнопка или указатель вида джойстика

    /// <summary>Кнопка или указатель вида джойстика</summary>
    private JoyButton _Button;

    /// <summary>Кнопка или указатель вида джойстика</summary>
    public JoyButton Button
    {
        get => _Button;
        set => Set(ref _Button, value);
    }

    #endregion


    #region SinglePressKeys : ActionKeysBindingViewModel - Команды при нажатии кнопки

    /// <summary>Команды при нажатии кнопки</summary>
    private ActionKeysBindingViewModel _SinglePressKeys = new() {Name = "Команды при однократном нажатии кнопки"};

    /// <summary>Команды при нажатии кнопки</summary>
    public ActionKeysBindingViewModel SinglePressKeys
    {
        get => _SinglePressKeys;
        set => Set(ref _SinglePressKeys, value);
    }

    #endregion

    #region DoublePressKeys : ActionKeysBindingViewModel - Команды двойного нажатия кнопки

    /// <summary>Команды двойного нажатия кнопки</summary>
    private ActionKeysBindingViewModel _DoublePressKeys = new() {Name = "Команды двойного нажатия кнопки"};

    /// <summary>Команды двойного нажатия кнопки</summary>
    public ActionKeysBindingViewModel DoublePressKeys
    {
        get => _DoublePressKeys;
        set => Set(ref _DoublePressKeys, value);
    }

    #endregion

    #region LongPressKeys : ActionKeysBindingViewModel - Команды долгого нажатия кнопки

    /// <summary>Команды долгого нажатия кнопки</summary>
    private ActionKeysBindingViewModel _LongPressKeys = new() { Name = "Команды при долгом нажатии кнопки" };

    /// <summary>Команды долгого нажатия кнопки</summary>
    public ActionKeysBindingViewModel LongPressKeys
    {
        get => _LongPressKeys;
        set => Set(ref _LongPressKeys, value);
    }

    #endregion


    public override string Description => Button.ToString();
}