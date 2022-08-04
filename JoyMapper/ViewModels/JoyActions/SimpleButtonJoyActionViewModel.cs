using JoyMapper.Models;

namespace JoyMapper.ViewModels.JoyActions;

internal class SimpleButtonJoyActionViewModel : JoyActionViewModelBase
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


    #region PressKeys : ActionKeysBindingViewModel - Команды при нажатии кнопки

    /// <summary>Команды при нажатии кнопки</summary>
    private ActionKeysBindingViewModel _PressKeys = new() {Name = "Команды при нажатии кнопки"};

    /// <summary>Команды при нажатии кнопки</summary>
    public ActionKeysBindingViewModel PressKeys
    {
        get => _PressKeys;
        set => Set(ref _PressKeys, value);
    }

    #endregion


    #region ReleaseKeys : ActionKeysBindingViewModel - Команды при отпускании кнопки

    /// <summary>Команды при отпускании кнопки</summary>
    private ActionKeysBindingViewModel _ReleaseKeys = new() {Name = "Команды при отпускании кнопки"};

    /// <summary>Команды при отпускании кнопки</summary>
    public ActionKeysBindingViewModel ReleaseKeys
    {
        get => _ReleaseKeys;
        set => Set(ref _ReleaseKeys, value);
    }

    #endregion


    public override string Description => Button.ToString();
}