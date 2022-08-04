using System.Linq;
using System.Windows.Input;
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
    public ActionKeysBindingViewModel PressKeys { get; } = new("Команды при нажатии кнопки");

    #endregion


    #region ReleaseKeys : ActionKeysBindingViewModel - Команды при отпускании кнопки

    /// <summary>Команды при отпускании кнопки</summary>
    public ActionKeysBindingViewModel ReleaseKeys { get; } = new("Команды при отпускании кнопки");

    #endregion


    public override string Description => Button.ToString();

    public override bool HasKeyBindings => PressKeys.KeyBindings.Any() || ReleaseKeys.KeyBindings.Any();

    public override bool IsRecording => PressKeys.IsRecorded || ReleaseKeys.IsRecorded;
    public override void AddKeyBinding(Key key, bool isPress)
    {
        if(PressKeys.IsRecorded)
            PressKeys.KeyBindings.Add(new KeyboardKeyBinding {IsPress = isPress, KeyCode = key});
        if (ReleaseKeys.IsRecorded)
            ReleaseKeys.KeyBindings.Add(new KeyboardKeyBinding { IsPress = isPress, KeyCode = key });
    }
}